using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Match3.Foundation.Base.Utility
{
    public static class CodeSyntaxHighlighter
    {
        private class WordStyleCondition
        {
            public string RegexPattern { get; }
            public Color Color { get; }
            public FontStyle FontStyle { get; }
            public RegexOptions Options { get; }
            public bool ShouldDisableOtherTags { get; }

            public WordStyleCondition(string regexPattern, Color color, FontStyle fontStyle = FontStyle.Normal, RegexOptions options = RegexOptions.None, bool shouldDisableOtherTags = false)
            {
                RegexPattern = regexPattern;
                Color = color;
                FontStyle = fontStyle;
                Options = options;
                ShouldDisableOtherTags = shouldDisableOtherTags;
            }
        }

        private static readonly List<WordStyleCondition> WordsStylesConditions;

        static CodeSyntaxHighlighter()
        {
            WordsStylesConditions =
                new List<WordStyleCondition>()
                {
                    new WordStyleCondition(regexPattern: @"[()]", new Color(0.38f, 0.76f, 1f)),
                    new WordStyleCondition(regexPattern: @"[.]", new Color(1f, 1f, 1f), FontStyle.Bold),
                    new WordStyleCondition(regexPattern: @"[;]", Color.gray),
                    new WordStyleCondition(regexPattern: @"\b(GameObject)\b", new Color(1f, 0.58f, 0.01f)),
                    new WordStyleCondition(regexPattern: @"\b(var |int |float |double |string |short |long |byte |List|HashSet|Object |Vector2|Vector3|Vector4)\b", new Color(1f, 0.58f, 0.01f)),
                    new WordStyleCondition(regexPattern: @"\b(public |internal |protected |private |partial |using |readonly )\b", new Color(0.38f, 0.76f, 1f)),
                    new WordStyleCondition(regexPattern: @"\b(namespace |class |static |const |void )\b", new Color(1f, 0.24f, 0.36f)),
                    new WordStyleCondition(regexPattern: @"\b(foreach)\b", new Color(0.73f, 1f, 0.38f)),
                    new WordStyleCondition(regexPattern: @"\b(if|else|else if|while|for)\b", new Color(0.73f, 1f, 0.38f)),
                    new WordStyleCondition(regexPattern: @"\b(true|false)\b", new Color(1f, 0.42f, 0.58f)),
                    new WordStyleCondition(regexPattern: @"\b(Serializable|SerializeField)\b", new Color(0.88f, 0.6f, 1f)),
                    new WordStyleCondition(regexPattern: @"\b(MonoBehaviour)\b", new Color(0.38f, 0.76f, 1f)),
                    new WordStyleCondition(regexPattern: "\"(.*?)\"", new Color(1f, 0.99f, 0.57f), shouldDisableOtherTags: true), // getting string sections
                    new WordStyleCondition(regexPattern: @"(\/\/.+?$|\/\*.+?\*\/)", Color.gray, FontStyle.Italic, RegexOptions.Multiline, shouldDisableOtherTags: true) // getting comments (inline or multiline)
                };
        }

        public static string DoCodeSyntaxHighlighting(string originalText, string codeSectionIdentifier = "```")
        {
            string[] splitted = SplitInToCodeAndNoneCodeParts();
            SetWordsStylesForCodeParts();
            return MergeSplitParts();

            string[] SplitInToCodeAndNoneCodeParts() => originalText.Split(codeSectionIdentifier);

            void SetWordsStylesForCodeParts()
            {
                for (int i = 1; i < splitted.Length; i += 2)
                    splitted[i] = SetWordsStylesInText(splitted[i]);
            }

            string MergeSplitParts() => splitted.Aggregate((part1, part2) => part1 + part2);
        }

        private static string SetWordsStylesInText(string text)
        {
            foreach (WordStyleCondition styleCondition in WordsStylesConditions)
                try
                {
                    SetWordStyleInText(styleCondition);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"Couldn't Apply Regex to find font, wordsRegexMatchPattern: {styleCondition.RegexPattern}, text: {text} | Exception: {exception}");
                }
            return text;

            void SetWordStyleInText(WordStyleCondition styleCondition)
            {
                MatchCollection wordMatches = Regex.Matches(text, styleCondition.RegexPattern, styleCondition.Options);
                foreach (Match wordMatch in wordMatches)
                    text = text.Replace(oldValue: wordMatch.Value, newValue: SetRichTextTagsTo(wordMatch.Value));


                string SetRichTextTagsTo(string originalText)
                {
                    if (styleCondition.ShouldDisableOtherTags)
                        RemoveRichTextTags();

                    RichTextTag colorStyleTag = GetColorRichTextTag();
                    RichTextTag fontStyleTag = GetFontStyleRichTextTag();


                    return $"{fontStyleTag.StartingTag}" +
                           $"{colorStyleTag.StartingTag}" +
                           $"{originalText}" +
                           $"{colorStyleTag.EndingTag}" +
                           $"{fontStyleTag.EndingTag}";

                    void RemoveRichTextTags() => originalText = Regex.Replace(originalText, pattern: @"<[^>]*>", string.Empty);

                    RichTextTag GetColorRichTextTag() => new RichTextTag(startingTag: $"<color=#{ColorUtility.ToHtmlStringRGB(styleCondition.Color)}>", endingTag: "</color>");

                    RichTextTag GetFontStyleRichTextTag()
                    {
                        switch (styleCondition.FontStyle)
                        {
                            case FontStyle.Bold: return new RichTextTag(startingTag: "<b>", endingTag: "</b>");
                            case FontStyle.Italic: return new RichTextTag(startingTag: "<i>", endingTag: "</i>");
                            case FontStyle.BoldAndItalic: return new RichTextTag(startingTag: "<b><i>", endingTag: "</b></i>");
                            default: return new RichTextTag();
                        }
                    }
                }
            }
        }

        private struct RichTextTag
        {
            public string StartingTag { get; }
            public string EndingTag { get; }

            public RichTextTag(string startingTag, string endingTag)
            {
                StartingTag = startingTag;
                EndingTag = endingTag;
            }
        }
    }
}