using Match3.Foundation.Unity;
using System.Linq;


public partial class TaskManager
{
    private class DayConfigsContainer
    {
        private readonly ResourceDayConfigAsset[] resourceDayConfigs;

        public int Length { get => resourceDayConfigs.Length; }

        public DayConfigsContainer(ResourceDayConfigAsset[] dayConfigs)
        {
            this.resourceDayConfigs = dayConfigs;
        }

        public DayConfig this[int index]
        {
            get => resourceDayConfigs[index].Load();
        }

        public DayConfig[] GetAllConfigs()
        {
            return resourceDayConfigs.Select(r => r.Load()).ToArray();
        }
    }
}