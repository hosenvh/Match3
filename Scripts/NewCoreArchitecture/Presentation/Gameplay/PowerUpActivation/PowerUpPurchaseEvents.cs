using Match3.Foundation.Base.EventManagement;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public abstract class PowerUpPurchaseEvent : GameEvent
    {
        public readonly int powerupIndex;

        public PowerUpPurchaseEvent(int index)
        {
            this.powerupIndex = index;
        }
    }

    public class PowerUpPurchaseSucceededEvent : PowerUpPurchaseEvent
    {
        public PowerUpPurchaseSucceededEvent(int index) : base(index)
        {
        }
    }

    public class PowerUpPurchaseFailedEvent : PowerUpPurchaseEvent
    {
        public PowerUpPurchaseFailedEvent(int index) : base(index)
        {
        }
    }

    public class PowerUpPurchaseOpenedEvent : PowerUpPurchaseEvent
    {
        public PowerUpPurchaseOpenedEvent(int index) : base(index)
        {
        }
    }

    public class PowerUpPurchaseClosedEvent : PowerUpPurchaseEvent
    {
        public PowerUpPurchaseClosedEvent(int index) : base(index)
        {
        }
    }

}