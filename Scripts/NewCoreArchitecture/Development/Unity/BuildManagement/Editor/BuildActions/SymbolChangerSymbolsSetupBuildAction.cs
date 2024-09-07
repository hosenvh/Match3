using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Symbol Changer Symbol Setup")]
    public class SymbolChangerSymbolsSetupBuildAction : ScriptableBuildAction
    {

        public SymbolChangerBuildAction symbolChangerBuildAction;
        public string[] symbolsToAdd;
        public string[] symbolsToRemove;

        public override void Execute()
        {
            symbolChangerBuildAction.symbolsToAdd = symbolsToAdd;
            symbolChangerBuildAction.symbolsToRemove = symbolsToRemove;
        }

        public override void Revert()
        {
            symbolChangerBuildAction.symbolsToAdd = symbolsToRemove;
            symbolChangerBuildAction.symbolsToRemove = symbolsToAdd;
        }
        
    }

}