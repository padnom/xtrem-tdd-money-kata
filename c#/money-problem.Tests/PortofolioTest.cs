using System;
using System.Collections.Generic;
using FluentAssertions;
using money_problem.Domain;
using Xunit;

namespace money_problem.Tests;
public class PortfolioTest
{

  [Fact(DisplayName = "5 USD + 10 EUR = 17 USD")]
  public void Add_ShouldAddMoneyInDollarAndEuro()
  {
    // Arrange
    Bank bank = Bank.WithExchangeRate(Currency.EUR, Currency.USD, 1.2);
    Portfolio portfolio = new Portfolio();
    portfolio.Add(5, Currency.USD);
    portfolio.Add(10, Currency.EUR);

    // Act
    var evaluation = portfolio.Evaluate(bank, Currency.USD);

    // Assert
    evaluation.Should().Be(17);
  }
  
  [Fact(DisplayName = "1 USD + 1100 KRW = 2200 KRW")]
  public void Add_ShouldAddMoneyInDollarAndKoreanWons()
  {
    // Arrange
    Bank bank = Bank.WithExchangeRate(Currency.USD, Currency.KRW, 1100);
    Portfolio portfolio = new Portfolio();
    portfolio.Add(1, Currency.USD);
    portfolio.Add(1100, Currency.KRW);
    
    // Act 
    double evaluation = portfolio.Evaluate(bank, Currency.KRW);

    // Assert
    evaluation.Should().Be(2200);
  }
  
  [Fact(DisplayName = "5 USD + 10 EUR + 4 EUR = 21.8 USD")]
  public void Add_ShouldAddMoneyInDollarAndEuroAndEuro()
  {
    // Arrange
    Bank bank = Bank.WithExchangeRate(Currency.EUR, Currency.USD, 1.2);
    Portfolio portfolio = new();
    portfolio.Add(5, Currency.USD);
    portfolio.Add(10, Currency.EUR);
    portfolio.Add(4, Currency.EUR);
    
    // Act
    double evaluation =  portfolio.Evaluate(bank, Currency.USD);

    // Assert
    evaluation.Should().Be(21.8);
    
  }
  
  [Fact(DisplayName = "1 EUR + 1 USD + 1 KRW = EUR")]
  public void Add_ShouldThrowMissingExchangeRatesException()
  {
    // Arrange
    Bank bank = Bank.WithExchangeRate(Currency.USD, Currency.KRW, 1100);
    bank.AddExchangeRate(Currency.EUR, Currency.USD, 1.2);
    var portfolio = new Portfolio();
    portfolio.Add(1, Currency.USD);
    portfolio.Add(1, Currency.EUR);
    portfolio.Add(1, Currency.KRW);
    
    // Act
    Action act = ()=> portfolio.Evaluate(bank, Currency.EUR);
    
    // Assert
    act.Should().Throw<MissingExchangeRateException>()
       .WithMessage("Missing exchange rate(s): [USD->EUR],[KRW->EUR]");
  }
  
  public class Portfolio
  {
    private IDictionary<Currency,Double> moneys = new Dictionary<Currency, double>();
    public void Add(double amount, Currency currency)
    {
      if (moneys.ContainsKey(currency))
      {
        moneys[currency] += amount;
      }
      else
      {
        moneys.Add(currency, amount);
      }
      
    }

    public double Evaluate(Bank bank, Currency currency)
    {
      double result = 0;
      
      foreach (var money in moneys)
      {
        result += bank.Convert(money.Value, money.Key, currency);
      }

      return result;
    }
  }
}