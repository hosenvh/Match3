using Match3.Foundation.Base.ServiceLocating;


namespace Match3.Foundation.Base.EventManagement
{
    public interface EventManager : Service
    {
        void Propagate(GameEvent evt, object sender);
        void Register(EventListener listener);
        void UnRegister(EventListener listener);
        bool Has(EventListener listener);
        void Clear();
    }
}