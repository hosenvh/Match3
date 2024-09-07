using Medrick.Development.Base.BuildManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Medrick.Development.Unity.BuildManagement
{
    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Option Group")]
    public class ScriptableBuildOptionGroup : ScriptableObject, BuildOptionGroup
    {
        public string groupName;
        public List<SerializableBuildOption> buildOptions;

        int currentSelectedOption;

        public void Apply()
        {
            foreach (var option in BuildOptions())
                if(option != CurrentSelectedOption())
                    option.Reset();

            CurrentSelectedOption().Apply();   
        }

        public IEnumerable<BuildOption> BuildOptions()
        {
            return buildOptions;
        }

        public bool IsOptionSelected(BuildOption buildOption)
        {
            return CurrentSelectedOption() == buildOption;
        }

        private void ValidateCurrentSelectedOption()
        {
            currentSelectedOption = Mathf.Clamp(currentSelectedOption, 0, buildOptions.Count - 1);
        }

        private BuildOption CurrentSelectedOption()
        {
            ValidateCurrentSelectedOption();
            return buildOptions[currentSelectedOption];
        }

        public string Name()
        {
            return groupName;
        }

        public void SetSelectedOption(BuildOption buildOption)
        {
            currentSelectedOption = buildOptions.IndexOf(buildOption as SerializableBuildOption);
        }

        public int CurretSelectedOptionIndex()
        {
            return currentSelectedOption;
        }
    }
}