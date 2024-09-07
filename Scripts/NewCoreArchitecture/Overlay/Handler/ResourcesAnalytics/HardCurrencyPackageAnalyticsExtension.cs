using System;
using Match3.Game.ShopManagement;


namespace Match3.Overlay.Analytics
{
    public static class HardCurrencyPackageAnalyticsExtension
    {

        public static string GetCamelCaseSku(this HardCurrencyPackage package)
        {
            return UnderscoreToCamelCase(package.SKU());
        }

        private static string UnderscoreToCamelCase(string text)
        {
            if (string.IsNullOrEmpty(text) || !text.Contains("_"))
            {
                return text;
            }
            string[] array = text.Split('_');
            for (int i = 0; i < array.Length; i++)
            {
                string s = array[i];
                string first = string.Empty;
                string rest = string.Empty;
                if (s.Length > 0)
                {
                    first = Char.ToUpperInvariant(s[0]).ToString();
                }
                if (s.Length > 1)
                {
                    rest = s.Substring(1).ToLowerInvariant();
                }
                array[i] = first + rest;
            }
            return string.Join("", array);
        }
    }
}