using FluentAssertions;
using money_problem.Domain;
using Xunit;
using static money_problem.Domain.Currency;

namespace money_problem.Tests
{
    public class MoneyShould
    {
        [Fact(DisplayName = "5 USD + 10 USD = 15 USD")]
        public void AddInUsd()
        {
            double? result = MoneyCalculator.Add(5, USD, 10);
            result.Should()
                .Be(15);
        }
        
        [Fact(DisplayName = "10 EUR x 2 = 20 EUR")]
        public void MultiplyInEuros()
        {
            MoneyCalculator.Times(10, EUR, 2)
                           .Should()
                           .Be(20d);
        }

        [Fact(DisplayName = "5 USD + 10 EUR = 1000.5 KRW")]
        public void DivideInKoreanWons()
        {
            MoneyCalculator.Divide(4002, KRW, 4)
                           .Should()
                           .Be(1000.5d);
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(1, 2, 3)]
        public void shouydl(int a, int b, int c)
        {
            
        }
    }
   
}