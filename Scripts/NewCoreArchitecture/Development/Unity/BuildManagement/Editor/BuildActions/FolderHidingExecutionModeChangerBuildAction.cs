using UnityEngine;


namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Folder Hiding Execution Mode Changer")]
    public class FolderHidingExecutionModeChangerBuildAction : ScriptableBuildAction
    {
        public FolderHidingBuildAction.ExecutionAction executionMode;
        public FolderHidingBuildAction target;

        public override void Execute()
        {
            target.executionAction = executionMode;
            UnityEditor.EditorUtility.SetDirty(target);
        }

        public override void Revert()
        {
            
        }
    }
}