using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.Foundation.Unity;
using UnityEngine;


namespace Match3.Main.Localization
{
    [Serializable]
    public class LanguageBasedResourceAudioClipAsset : LanguageBasedResourceAsset<ResourceAudioClipAsset>
    {
    }

    [Serializable]
    public class LanguageBasedResourceSpriteAsset : LanguageBasedResourceAsset<ResourceSpriteAsset>
    {
    }

    [Serializable]
    public class LanguageBasedResourceGameObjectAsset : LanguageBasedResourceAsset<ResourceGameObjectAsset>
    {
    }

    [Serializable]
    public abstract class LanguageBasedResourceAsset<T> where T : ResourceAsset
    {
        [Serializable]
        public class LanguageBasedValue
        {
            [LanguageSelectAttribute]
            [SerializeField] private string languageCode;
            [SerializeField] private T value;

            public string LanguageCode => languageCode;
            public T Value => value;
        }

        public List<LanguageBasedValue> localizedResourceAssets = new List<LanguageBasedValue>();

        public T GetValue()
        {
            return GetValueFor(LocalizationManager.CurrentLanguageCode);
        }

        private T GetValueFor(string languageCode)
        {
            foreach (var languageBasedAsset in localizedResourceAssets)
                if (languageBasedAsset.LanguageCode.Equals(languageCode))
                    return languageBasedAsset.Value;

            return null;
        }
    }
}