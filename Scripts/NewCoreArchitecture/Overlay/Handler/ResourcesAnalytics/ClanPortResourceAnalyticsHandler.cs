using System;
using Match3.Clan.Presentation.Popups;
using Match3.Clan.Utilities;
using Match3.Foundation.Base.EventManagement;
using SeganX;
using static GameAnalyticsDataProvider;


namespace Match3.Overlay.Analytics.ResourcesAnalytics
{
    public class ClanPortResourceAnalyticsHandler : ResourcesBaseAnalyticsHandler
    {
        private const string CLAN_JOIN_REWARD_ITEM_ID = "Join_Reward";
        private string currentOpenItemId;

        public ClanPortResourceAnalyticsHandler(ResourceAnalyticsHandlerActions actions) : base(ResourcesItemType.Clan, actions)
        {
        }

        protected override void Handle(GameEvent evt)
        {
            switch (evt)
            {
                case Popup_ClanMain.OnClanTabSwitched clanTabSwitchedEvent:
                    HandleClanTabOpening(clanTabSwitchedEvent.CurrentTab);
                    break;
                case ClanJoiningHandler.OnClanJoinRewardGivingStartedEvent:
                    OpenPort(CLAN_JOIN_REWARD_ITEM_ID);
                    break;
                case ClanJoiningHandler.OnClanJoinRewardGivingFinishedEvent:
                    ClosePort(CLAN_JOIN_REWARD_ITEM_ID);
                    break;
            }
        }

        private void HandleClanTabOpening(Type currentTabType)
        {
            if (currentOpenItemId.IsNullOrEmpty() == false)
                ClosePort(currentOpenItemId);
            currentOpenItemId = currentTabType.Name;
            OpenPort(itemId: currentOpenItemId);
        }

        public override void HandleGameStatesClosings(GameState gameState)
        {
            if (gameState is Popup_ClanMain)
                HandleClanClosing();
        }

        private void HandleClanClosing()
        {
            ClosePort(currentOpenItemId);
            currentOpenItemId = null;
        }

        public override void HandleGameStatesOpenings(GameState gameState)
        {
        }
    }
}