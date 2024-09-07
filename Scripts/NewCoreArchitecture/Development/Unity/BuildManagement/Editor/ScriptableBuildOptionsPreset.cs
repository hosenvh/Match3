using ArmanCo.ShapeRunner.Utility;
using Medrick.Development.Base.BuildManagement;
using SeganX;
using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    public class ScriptableBuildOptionsGroupEntryAttribute : PropertyAttribute
    {
    }


    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Options Preset")]
    public class ScriptableBuildOptionsPreset : ScriptableObject, BuildOptionsPreset
    {
        [System.Serializable]
        public class GroupOptionEntry
        {
            public ScriptableBuildOptionGroup group;
            public int optionIndex;

            public GroupOptionEntry(ScriptableBuildOptionGroup group, int optionIndex)
            {
                this.group = group;
                this.optionIndex = optionIndex;
            }
        }

        [ButtonAttribute("Update", nameof(TryAddOptions))]
        public string buttonTemp;

        public new string name;
        public string description;

        [ScriptableBuildOptionsGroupEntryAttribute]
        public List<GroupOptionEntry> groupOptionsEntry;


        public void TryAddOptions()
        {
            AssetEditorUtilities.FindAllAssetsOfType<ScriptableBuildOptionGroup>(TryAddOption);
        }

        private void OnValidate()
        {
            for (int i = groupOptionsEntry.Count -1; i >= 0; --i)
                if (groupOptionsEntry[i].group == null)
                    groupOptionsEntry.RemoveAt(i);
            TryAddOptions();
        }

        private void TryAddOption(ScriptableBuildOptionGroup group)
        {
            foreach (var entry in groupOptionsEntry)
                if (entry.group == group)
                    return;

            var newEntry = new GroupOptionEntry(group, -1);

            groupOptionsEntry.Add(newEntry);
        }

        public void Apply()
        {
            foreach (var entry in groupOptionsEntry)
                if (entry.optionIndex >= 0)
                    entry.group.SetSelectedOption(entry.group.buildOptions[entry.optionIndex]);
        }

        public string Name()
        {
            return name;
        }
    }
}