using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Match3.Presentation.TextAdapting;
using UnityEngine;
using Random = UnityEngine.Random;


public class LoadingScreenHint : MonoBehaviour
{
    [SerializeField] private RTLTextMeshProAdapter hint;

    private LanguageSourceAsset loadingScreenHintLanguageSource;

    private void Start()
    {
        ShowHint();
    }

    private void OnEnable()
    {
        loadingScreenHintLanguageSource =
            Resources.Load<LanguageSourceAsset>(LocalizationManager.LoadingScreenHintLanguageSource);
    }

    private void OnDisable()
    {
        loadingScreenHintLanguageSource = null;
        Resources.UnloadUnusedAssets();
    }

    private void ShowHint()
    {
        List<TermData> terms = GetExistingTerms();
        TermData term = GetRandomTerm();
        LocalizedStringTerm localizedString = GetTermString();
        string translatedString = GetTranslatedString();

        SetHint(translatedString);

        List<TermData> GetExistingTerms() => loadingScreenHintLanguageSource.mSource.mTerms;
        TermData GetRandomTerm() => terms[Random.Range(0, terms.Count)];
        string GetTermString() => term.Term.ToString();
        string GetTranslatedString() => localizedString.ToString();

        void SetHint(string text) => hint.SetText(text);
    }
}
