
using System.Collections.Generic;

namespace Medrick.Development.Base.BuildManagement
{
    // TODO: Maybe change it to an interface later?
    public class BuildOptionsManager 
    {
        List<BuildOptionGroup> buildOptionGroups = new List<BuildOptionGroup>();
        List<BuildOptionsPreset> buildOptionsPreset = new List<BuildOptionsPreset>();


        List<BuildAction> globalBuildActions = new List<BuildAction>();

        // TODO: Try to find a better data structure.
        Dictionary<string, List<BuildAction>> specificBuildActions = new Dictionary<string, List<BuildAction>>();

        public void ApplyAll()
        {
            // NOTE: It can later be devided to post and pre actions
            ApplyBuildActions();

            ApplyOptions();
        }

        protected virtual void OnOptionsApplied()
        {

        }

        public void ApplyOptions()
        {
            foreach (var group in buildOptionGroups)
                group.Apply();

            OnOptionsApplied();
        }

        public void ApplyOption(BuildOptionGroup buildOptionGroup)
        {
            buildOptionGroup.Apply();
            OnOptionsApplied();
        }

        public void ApplyBuildActions()
        {
            foreach (var action in globalBuildActions)
                action.Execute();
        }


        // TODO: Try to find a better name for this concept.
        public void ApplySpecificBuildActions(string tag)
        {
            if (specificBuildActions.ContainsKey(tag) == false)
                return;

            foreach (var action in specificBuildActions[tag])
                action.Execute();
        }

        public void ApplyOptionsPreset(BuildOptionsPreset preset)
        {
            preset.Apply();
            ApplyAll();
        }

        public void RegisterOptionGroup(BuildOptionGroup optionGroup)
        {
            buildOptionGroups.Add(optionGroup);
        }

        public void RegisterOptionsPreset(BuildOptionsPreset optionsPreset)
        {
            buildOptionsPreset.Add(optionsPreset);
        }

        public void RegisterGlobalBuildAction(BuildAction action)
        {
            globalBuildActions.Add(action);
        }

        public void RegisterSpecificBuildAction(string tag, BuildAction action)
        {
            if (specificBuildActions.ContainsKey(tag) == false)
                specificBuildActions[tag] = new List<BuildAction>();

            specificBuildActions[tag].Add(action);
        }

        public IEnumerable<BuildOptionGroup> BuildOptionGroups()
        {
            return buildOptionGroups;
        }

        public IEnumerable<BuildOptionsPreset> BuildOptionsPresets()
        {
            return buildOptionsPreset;
        }

        public Dictionary<string, List<BuildAction>> SpecificBuildActions()
        {
            return specificBuildActions;
        }
    }
}