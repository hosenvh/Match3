using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using UnityEngine;


namespace Match3.Main.Localization
{

    [System.Serializable]
    public class LanguageBasedInt
    {
        
        public static implicit operator int(LanguageBasedInt s)
        {
            return s.GetValue();
        }
        
        [System.Serializable]
        public class LanguageBasedIntValue
        {
            [LanguageSelectAttribute]
            public string languageCode;
            public int value;
        }
        
        public List<LanguageBasedIntValue> localizedInts = new List<LanguageBasedIntValue>();
        
        public void Add(string languageCode, int intValue)
        {
            foreach (var locStr in localizedInts)
            {
                if (locStr.languageCode.Equals(languageCode))
                {
                    locStr.value = intValue;
                    return;
                }
            }

            localizedInts.Add(new LanguageBasedIntValue(){ languageCode = languageCode, value = intValue});
        }
        
        public bool TryGetValue(string languageCode, out int value)
        {
            value = 0;
            foreach (var localizedString in localizedInts)
            {
                if (localizedString.languageCode.Equals(languageCode))
                {
                    value = localizedString.value;
                    return true;
                }
            }

            return false;
        }
        
        public int GetValue()
        {
            TryGetValue(LocalizationManager.CurrentLanguageCode, out int localizedInt);
            return localizedInt;
        }
    }
    
}