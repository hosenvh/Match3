using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KitchenParadise.Utiltiy.Base;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3.Main.Localization
{
    
    [System.Serializable]
    public class LanguageBasedString
    {
        
        public static implicit operator string(LanguageBasedString s)
        {
            return s.ToString();
        }
        
        [System.Serializable]
        public class LanguageBasedStringValue
        {
            [LanguageSelectAttribute]
            public string languageCode;
            [TextArea]
            public string value;
        }
        
        public List<LanguageBasedStringValue> localizedStrings = new List<LanguageBasedStringValue>();

        public bool TryGetValue(string languageCode, out string value)
        {
            value = "";
            foreach (var localizedString in localizedStrings)
            {
                if (localizedString.languageCode.Equals(languageCode))
                {
                    value = localizedString.value;
                    return true;
                }
            }

            return false;
        }
        

        public void AddTranslation(string languageCode, string translation)
        {
            foreach (var locStr in localizedStrings)
            {
                if (locStr.languageCode.Equals(languageCode))
                {
                    locStr.value = translation;
                    return;
                }
            }

            localizedStrings.Add(new LanguageBasedStringValue(){ languageCode = languageCode, value = translation});
        }
        
        public override string ToString()
        {
            TryGetValue(LocalizationManager.CurrentLanguageCode, out string localizedText);
            return localizedText;
        }
        
    }
    
    
    
}


