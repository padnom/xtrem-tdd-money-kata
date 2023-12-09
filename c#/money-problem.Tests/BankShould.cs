using System;
using FluentAssertions;
using money_problem.Domain;
using Xunit;
using static money_problem.Domain.Currency;

namespace money_problem.Tests
{
    public class BankShould
    {
        private readonly Bank _bank = Bank.WithExchangeRate(EUR, USD, 1.2);

        [Fact(DisplayName = "10 EUR -> USD = 12 USD")]
        public void ConvertEuroToUsd()
        {
            _bank.Convert(10, EUR, USD)
                .Should()
                .Be(12);
        }

        [Fact(DisplayName = "10 EUR -> EUR = 10 EUR")]
        public void ConvertMoneyInSameCurrency()
        {
            _bank.Convert(10, EUR, EUR)
                .Should()
                .Be(10);
        }

        [Fact(DisplayName = "Throws a MissingExchangeRateException in case of missing exchange rates")]
        public void ConvertWithMissingExchangeRateShouldThrowException()
        {
            MissingExchangeRateException expectedException = new MissingExchangeRateException(EUR, KRW);

            Action act = () => _bank.Convert(10, EUR, KRW);

            act.Should().ThrowExactly<MissingExchangeRateException>()
               .WithMessage(expectedException.Message);
        }

        [Fact(DisplayName = "Conversion with different exchange rates EUR -> USD")]
        public void ConvertWithDifferentExchangeRates()
        {
            _bank.Convert(10, EUR, USD)
                .Should()
                .Be(12);

            _bank.AddExchangeRate(EUR, USD, 1.3);
            
            _bank.Convert(10, EUR, USD)
                .Should()
                .Be(13);
        }
    }
}