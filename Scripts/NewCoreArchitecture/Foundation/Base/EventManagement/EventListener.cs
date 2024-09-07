
namespace Match3.Foundation.Base.EventManagement
{
    public interface EventListener
    {
        void OnEvent(GameEvent evt, object sender);
    }
}