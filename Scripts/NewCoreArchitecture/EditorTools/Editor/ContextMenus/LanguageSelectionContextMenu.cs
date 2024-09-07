using I2.Loc;
using SeganX;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.ContextMenus
{
    public class LanguageSelectionContextMenu
    {
        [MenuItem("CONTEXT/Transform/Set Language To Persian")]
        private static void SetLanguageToPersian(MenuCommand command)
        {
            SetLanguage(command, languageName: "Persian");
        }

        [MenuItem("CONTEXT/Transform/Set Language To English")]
        private static void SetLanguageToEnglish(MenuCommand command)
        {
            SetLanguage(command, languageName: "English");
        }

        [MenuItem("CONTEXT/Transform/Set Language To Turkish")]
        private static void SetLanguageToTurkish(MenuCommand command)
        {
            SetLanguage(command, languageName: "Turkish");
        }

        private static void SetLanguage(MenuCommand command, string languageName)
        {
            GameObject targetGameObject = ((Transform) command.context).gameObject;
            SetLocalizeComponentsLanguage();
            UpdateLocalTexts();

            void SetLocalizeComponentsLanguage()
            {
                foreach (Localize localizeComponent in targetGameObject.GetComponentsInChildren<Localize>())
                {
                    localizeComponent.SetGlobalLanguage(languageName);
                    localizeComponent.OnLocalize(Force: true);
                }
            }

            void UpdateLocalTexts()
            {
                foreach (LocalText localTextComponent in targetGameObject.GetComponentsInChildren<LocalText>())
                    localTextComponent.SetFormatedText();
            }
        }
    }
}