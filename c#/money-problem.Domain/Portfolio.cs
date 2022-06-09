using System.Collections.Immutable;

namespace money_problem.Domain;

public class Portfolio
{
    private readonly ICollection<Money> moneys;

    public Portfolio()
    {
        this.moneys = new List<Money>();
    }

    private Portfolio(IEnumerable<Money> moneys)
    {
        this.moneys = moneys.ToImmutableList();
    }

    private static bool ContainsFailure(IEnumerable<ConversionResult<MissingExchangeRateException>> results) =>
        results.Any(result => result.IsFailure());

    private List<ConversionResult<MissingExchangeRateException>> GetConvertedMoneys(Bank bank, Currency currency) =>
        this.moneys
            .Select(money => ConvertMoney(bank, currency, money))
            .ToList();

    private static ConversionResult<MissingExchangeRateException> ConvertMoney(Bank bank, Currency currency,
        Money money)
    {
        try
        {
            return new ConversionResult<MissingExchangeRateException>(bank.Convert(money, currency));
        }
        catch (MissingExchangeRateException exception)
        {
            return new ConversionResult<MissingExchangeRateException>(exception);
        }
    }

    public Portfolio Add(Money money)
    {
        List<Money> updatedMoneys = this.moneys.ToList();
        updatedMoneys.Add(money);
        return new Portfolio(updatedMoneys);
    }

    private static string GetMissingRates(IEnumerable<MissingExchangeRateException> missingRates) => missingRates
        .Select(exception => $"[{exception.Message}]")
        .Aggregate((r1, r2) => $"{r1},{r2}");
    
    public ConversionResult<string> Evaluate(Bank bank, Currency currency)
    {
        List<ConversionResult<MissingExchangeRateException>> results = this.GetConvertedMoneys(bank, currency);
        return ContainsFailure(results) ? ConversionResult<string>.FromFailure(this.ToFailure(results)) : ConversionResult<string>.FromMoney(this.ToSuccess(results, currency));
    }
    
    private string ToFailure(IEnumerable<ConversionResult<MissingExchangeRateException>> results) =>
        $"Missing exchange rate(s): {GetMissingRates(results.Where(result => result.IsFailure()).Select(result => result.Failure!))}";
    
    private Money ToSuccess(IEnumerable<ConversionResult<MissingExchangeRateException>> results, Currency currency) =>
        new Money(results.Where(result => result.IsSuccess()).Sum(result => result.Money!.Amount), currency);
}