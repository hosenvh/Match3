using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.EditorTools.Editor.Menus.Verification.MapItemsSpineVerification
{
    public class MapItemsSpineVerifier
    {
        private static readonly string ANIMATION_PATH = "animations.";

        public void Verify(string rootPath)
        {
            List<string> paths = FindSpinesJsonPaths(rootPath);

            foreach (var path in paths)
            {
                Object dataObject = LoadSpineObject();
                JObject jsonObject = LoadSpineJson();
                List<string> animationNames = FindAnimations(jsonObject);
                LogVerification();

                Object LoadSpineObject() => AssetDatabase.LoadAssetAtPath<Object>(path);

                JObject LoadSpineJson() => JObject.Parse(dataObject.ToString());

                void LogVerification()
                {
                    if (IsVerified())
                        Debug.Log($"Map Item Spine Verifier | Item with path: {path} verified.", dataObject);
                    else
                        Debug.LogError($"Map Item Spine Verifier | Item with path: {path} failed verification.", dataObject);
                }

                bool IsVerified() => AreAnimationNamesOk(animationNames);
            }
        }

        private List<string> FindSpinesJsonPaths(string rootPath)
        {
            var spines = AssetDatabase.FindAssets("*", new[] {rootPath});
            List<string> jsonPaths = new List<string>();

            foreach (var guid in spines)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains(".json"))
                {
                    jsonPaths.Add(path);
                }
            }

            return jsonPaths;
        }

        private List<string> FindAnimations (JObject jsonObject)
        {
            List<string> animationNames = new List<string>();

            foreach (var animation in jsonObject["animations"])
                animationNames.Add(animation.Path.Remove(0, ANIMATION_PATH.Length)); // animations.Path e.g.: "animations.1" or "animations.1_2"

            return animationNames;
        }

        private bool AreAnimationNamesOk(List<string> animationNames)
        {
            List<string> animationNumbers = new List<string>();
            List<string> animationStrings = new List<string>();
            List<string> extractedAnimationNumbers = new List<string>();

            foreach (var animationName in animationNames)
            {
                if (IsAnimationNameANumber())
                    animationNumbers.Add(animationName);
                else if(IsAnimationNameAValidString())
                {
                    animationStrings.Add(animationName);
                    ExtractAnimationNumbersFromAnimationString();
                }
                else
                    return false;


                bool IsAnimationNameANumber()
                {
                    int numericValue;
                    return int.TryParse(animationName, out numericValue);
                }

                bool IsAnimationNameAValidString()
                {
                    var mustMatch = new Regex("^\\d*[_]\\d$");
                    return mustMatch.IsMatch(animationName);
                }

                void ExtractAnimationNumbersFromAnimationString()
                {
                    string[] extractedNumbersArr = animationName.Split('_');
                    extractedAnimationNumbers.AddRange(new List<string>(extractedNumbersArr));
                    extractedAnimationNumbers = extractedAnimationNumbers.Distinct().ToList();
                }
            }

            if (!DoAllTheExtractedNumbersExistInNumbers())
                return false;

            bool DoAllTheExtractedNumbersExistInNumbers() => !extractedAnimationNumbers.Except(animationNumbers).Any();

            return true;
        }
    }
}
