using I2.Loc;
using Match3.Development.Base.DevelopmentConsole;
using SeganX;
using UnityEngine;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Localization", priority: 29)]
    public class LocalizationDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Change To Persian", shouldAutoClose: true)]
        public static void ChangeLanguageToPersian()
        {
            ChangeLanguage(languageName: "Persian");
        }

        [DevOption(commandName: "Change To English", shouldAutoClose: true)]
        public static void ChangeLanguageToEnglish()
        {
            ChangeLanguage(languageName: "English");
        }

        [DevOption(commandName: "Change To Turkish", shouldAutoClose: true)]
        public static void ChangeLanguageToTurkish()
        {
            ChangeLanguage(languageName: "Turkish");
        }

        private static void ChangeLanguage(string languageName)
        {
            UpdateCurrentActiveLanguage();
            SetLocalizeComponentsLanguage();
            UpdateLocalTexts();

            void UpdateCurrentActiveLanguage()
            {
                LocalizationManager.CurrentLanguage = languageName;
            }

            void SetLocalizeComponentsLanguage()
            {
                foreach (Localize localizeComponent in Object.FindObjectsOfType<Localize>())
                {
                    localizeComponent.SetGlobalLanguage(languageName);
                    localizeComponent.OnLocalize(Force: true);
                }
            }

            void UpdateLocalTexts()
            {
                foreach (LocalText localTextComponent in Object.FindObjectsOfType<LocalText>())
                    localTextComponent.SetFormatedText();
            }
        }
    }
}