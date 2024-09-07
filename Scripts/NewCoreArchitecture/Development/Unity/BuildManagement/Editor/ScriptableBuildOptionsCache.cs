using Medrick.Development.Base.BuildManagement;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Medrick.Development.Unity.BuildManagement.ScriptableBuildOptionsPreset;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Options Cache")]
    public class ScriptableBuildOptionsCache : ScriptableObject
    {
        [ScriptableBuildOptionsGroupEntryAttribute]
        public List<GroupOptionEntry> groupOptionsEntry;

        public void Save(IEnumerable<ScriptableBuildOptionGroup> groups)
        {
            groupOptionsEntry.Clear();

            foreach(var group in groups)
                groupOptionsEntry.Add(new GroupOptionEntry(group, group.CurretSelectedOptionIndex()));
            

            UnityEditor.EditorUtility.SetDirty(this);
        }

        public void Restore()
        {
            foreach (var entry in groupOptionsEntry)
                entry.group.SetSelectedOption(entry.group.buildOptions[entry.optionIndex]);
        }
    }
}