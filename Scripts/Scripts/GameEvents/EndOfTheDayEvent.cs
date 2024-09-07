using Match3.Foundation.Base.EventManagement;

namespace Match3
{
    public class EndOfTheDayEvent : GameEvent
    {
        public DayConfig dayConfig = null;
        public int currentDay = 0;

    }
}