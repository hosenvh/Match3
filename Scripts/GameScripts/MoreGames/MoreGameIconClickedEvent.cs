using Match3.Foundation.Base.EventManagement;


namespace Match3.MoreGames
{
    public class MoreGameIconClickedEvent : GameEvent
    {
        public string GameTitle { get; }

        public MoreGameIconClickedEvent(string gameTitle)
        {
            GameTitle = gameTitle;
        }
    }
}