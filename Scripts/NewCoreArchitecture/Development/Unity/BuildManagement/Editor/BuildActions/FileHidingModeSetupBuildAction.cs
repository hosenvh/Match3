using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{

    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/File Hider Mode Setup")]
    public class FileHidingModeSetupBuildAction : ScriptableBuildAction
    {
        public FileHidingBuildAction.ExecutionAction executionMode;
        public FileHidingBuildAction fileHidingTarget;

        public override void Execute()
        {
            fileHidingTarget.executionAction = executionMode;
            UnityEditor.EditorUtility.SetDirty(fileHidingTarget);
        }

        public override void Revert()
        {
            
        }
    }

}