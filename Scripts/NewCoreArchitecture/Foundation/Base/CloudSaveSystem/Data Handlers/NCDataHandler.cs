using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;



namespace Match3.CloudSave
{
    public class NCDataHandler : ICloudDataHandler
    {

        private const string TicketKey = "NCLife";
        private const string SelectedLevelIndexKey = "NCSelectedLevelIndex";
    
    
    
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var NCManager = ServiceLocator.Find<NeighborhoodChallengeManager>();
        
            cloudStorage.SetInt(SelectedLevelIndexKey, NCManager.LevelSelector.SelectedLevel());
            cloudStorage.SetInt(TicketKey, NCManager.Ticket().CurrentValue());
        }

    
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var NCManager = ServiceLocator.Find<NeighborhoodChallengeManager>();
        
            NCManager.LevelSelector.SetSelectedLevel(cloudStorage.GetInt(SelectedLevelIndexKey, -1));
            NCManager.Ticket().SetValue(cloudStorage.GetInt(TicketKey, NCManager.Ticket().MaxValue));
        }
    
    
    }

}

