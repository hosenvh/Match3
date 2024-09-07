using I2.Loc;
using UnityEditor;
using UnityEngine;


namespace Match3.EditorTools.Editor.Menus.Localization
{

    public class I2TermMovingWindow : EditorWindow
    {
        [MenuItem("Golmorad/Localization/I2 Term Moving Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(I2TermMovingWindow));
        }

        LanguageSourceAsset sourceLanguageSource;
        LanguageSourceAsset destinationLanguageSource;

        string termPrefixToMove;
        bool errorIfPersianTranslateIsDifferent;

        void OnGUI()
        {
            sourceLanguageSource = (LanguageSourceAsset)EditorGUILayout.ObjectField("Source", sourceLanguageSource, typeof(LanguageSourceAsset), allowSceneObjects : false);
            destinationLanguageSource = (LanguageSourceAsset)EditorGUILayout.ObjectField("Destination", destinationLanguageSource, typeof(LanguageSourceAsset), allowSceneObjects : false);

            termPrefixToMove = EditorGUILayout.TextField("Term Prefix", termPrefixToMove);


            errorIfPersianTranslateIsDifferent = EditorGUILayout.Toggle("Error if Persian different", errorIfPersianTranslateIsDifferent);

            if (GUILayout.Button("Move"))
                MoveTerms(termPrefixToMove, sourceLanguageSource, destinationLanguageSource, errorIfPersianTranslateIsDifferent);
        }

        private void MoveTerms(string termPrefixToMove, LanguageSourceAsset sourceLanguageSource, LanguageSourceAsset destinationLanguageSource, bool errorIfPersianTranslateIsDifferent)
        {
            var terms = sourceLanguageSource.mSource.mTerms;
            for (int i = terms.Count - 1; i >= 0; --i)
            {
                TermData termData = terms[i];
                if (termData.Term.StartsWith(termPrefixToMove))
                {
                    if(errorIfPersianTranslateIsDifferent && destinationLanguageSource.mSource.ContainsTerm(termData.Term))
                    {
                        var destinationPersianTranslate = destinationLanguageSource.mSource.GetTranslation(termData.Term, overrideLanguage: "Persian", skipDisabled: false);
                        var sourcePersianTranslate = sourceLanguageSource.mSource.GetTranslation(termData.Term, overrideLanguage: "Persian", skipDisabled: false);
                        if (destinationPersianTranslate.Equals(sourcePersianTranslate) == false)
                        {
                            Debug.LogError($"Term {termData.Term} has different persian translation in source and destination");
                            continue;
                        }
                    }

                    sourceLanguageSource.mSource.mTerms.RemoveAt(i);
                    sourceLanguageSource.mSource.mDictionary.Remove(termData.Term);

                    if(destinationLanguageSource.mSource.ContainsTerm(termData.Term))
                    {
                        destinationLanguageSource.mSource.RemoveTerm(termData.Term);
                    }

                    destinationLanguageSource.mSource.mTerms.Add(termData);
                    destinationLanguageSource.mSource.mDictionary.Add(termData.Term, termData);
                }
            }

            EditorUtility.SetDirty(sourceLanguageSource);
            EditorUtility.SetDirty(destinationLanguageSource);
        }
    }

}
