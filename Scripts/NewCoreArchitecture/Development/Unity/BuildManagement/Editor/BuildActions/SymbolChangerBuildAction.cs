using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Symbol Changer")]
    public class SymbolChangerBuildAction : ScriptableBuildAction
    {

        public string[] symbolsToAdd;
        public string[] symbolsToRemove;

        public override void Execute()
        {
            foreach (var symbol in symbolsToRemove)
            {
                RemoveSymbolFromSelectedPlatform(symbol);
            }
            
            foreach (var symbol in symbolsToAdd)
            {
                AddSymbolToSelectedPlatform(symbol);
            }
        }

        public override void Revert()
        {
            foreach (var symbol in symbolsToAdd)
            {
                RemoveSymbolFromSelectedPlatform(symbol);
            }
            
            foreach (var symbol in symbolsToRemove)
            {
                AddSymbolToSelectedPlatform(symbol);
            }
        }
        
        
        private void AddSymbolToSelectedPlatform(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string definedSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        
            if (definedSymbols.Contains(symbol))
                return;
        
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, (definedSymbols + ";" + symbol));
        }

        private void RemoveSymbolFromSelectedPlatform(string symbol)
        {
            BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            List<string> definedSymbols =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';').ToList();
        
            if (!definedSymbols.Contains(symbol))
                return;
        
            definedSymbols.Remove(symbol);
            var symbolsString = string.Join(";", definedSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbolsString);
        }
        
        
    }

}