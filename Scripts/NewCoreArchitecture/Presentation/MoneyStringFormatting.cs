using I2.Loc;
using Match3.Foundation.Base.MoneyHandling;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Match3.Presentation
{
    public static class MoneyStringFormatting
    {
        private static readonly Dictionary<string, string> currencyMap;
        static MoneyStringFormatting()
        {
            currencyMap = CultureInfo
                .GetCultures(CultureTypes.AllCultures)
                .Where(c => !c.IsNeutralCulture)
                .Select(culture => string.IsNullOrWhiteSpace(culture.Name) ? null : new RegionInfo(culture.Name))
                .Where(ri => ri != null)
                .GroupBy(ri => ri.ISOCurrencySymbol)
                .ToDictionary(x => x.Key, x => x.First().CurrencySymbol);
        }

        // TODO: Refactor this.
        public static string FormatMoneyToString(this Money money)
        {
            switch(money.CurrencyCode())
            {
                case "IRR":
                    return $"{(money.Amount()/10).ToString("N0")} تومان";
                case "USD":
                case "EUR":
                    return $"{GetCurrencySymbol(money.CurrencyCode())}{money.Amount()} ";
                case "TRY":
                    return $"{money.Amount()} TL";
                default:
                    return $"{money.Amount()} {money.CurrencyCode()}";
            }
        }

        static string GetCurrencySymbol(string ISOCurrencyCoode)
        {
            if (currencyMap.TryGetValue(ISOCurrencyCoode, out var symbol))
                return symbol;
            else
                return "NA";
        }
    }
}