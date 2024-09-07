using I2.Loc;
using UnityEngine;



namespace Match3.Main.Localization
{

    public class GameLanguageHelper
    {
        
        public static void SetGameLanguageAsSystemLanguage(string defaultLanguageCode)
        {
            var systemLanguage = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var allLanguages = LocalizationManager.GetAllLanguagesCode();

            foreach (var language in allLanguages)
            {
                if (language == systemLanguage)
                {
                    LocalizationManager.CurrentLanguageCode = language;
                    return;
                }
            }

            LocalizationManager.CurrentLanguageCode = defaultLanguageCode;
        }
    }
    
}


