using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class LanguageSelectAttribute : PropertyAttribute { }


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(LanguageSelectAttribute))]
public class LanguageSelectDrawer : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        List<string> languages = LocalizationManager.GetAllLanguages(false);
        List<string> languagesCode = LocalizationManager.GetAllLanguagesCode(false, false);

        GUI.changed = false;
        int index = FindIndex(languagesCode, property.stringValue);

        index = EditorGUI.Popup(position, label.text, index, languages.ToArray(), EditorStyles.toolbarDropDown);

        if (GUI.changed && index >= 0)
            property.stringValue = languagesCode[index];
    }

    private int FindIndex(List<string> allLanguagesCode, string findingLanguageCode)
    {
        for (int i = 0; i < allLanguagesCode.Count; ++i)
        {
            if (allLanguagesCode[i].Equals(findingLanguageCode))
                return i;
        }

        return 0;
    }
    
}

#endif

