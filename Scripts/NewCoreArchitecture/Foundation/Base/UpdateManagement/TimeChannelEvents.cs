

using Match3.Foundation.Base.EventManagement;

namespace KitchenParadise.Foundation.Base.TimeManagement
{
    public class TimeChannelStateChangedEvent : GameEvent
    {
        public readonly Channel channel;
        public readonly bool isPaused;

        public TimeChannelStateChangedEvent(Channel channel, bool isPaused)
        {
            this.channel = channel;
            this.isPaused = isPaused;
        }
    }
}