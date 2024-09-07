using System.Collections.Generic;

namespace Medrick.Development.Base.BuildManagement
{
    public interface BuildOptionGroup
    {
        void Apply();

        IEnumerable<BuildOption> BuildOptions();

        bool IsOptionSelected(BuildOption buildOption);

        void SetSelectedOption(BuildOption buildOption);

        string Name();
    }
}