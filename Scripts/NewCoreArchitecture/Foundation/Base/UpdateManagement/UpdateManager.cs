

using Match3.Foundation.Base.ServiceLocating;

namespace KitchenParadise.Foundation.Base.TimeManagement
{

    public interface UpdateManager : Service
    {
        void RegisterChannel(Channel channel);
        void RegisterChannelToParent(Channel child, Channel parent);

        void RegisterUpdatable(Updatable updatable, Channel channel);
        void UnRegisterUpdatable(Updatable updatable);

        void Pause(Channel channel);
        void Resume(Channel channel);

        void SetChannelTimeScale(Channel channel, float scale);

        bool Has(Updatable updatable);
        bool IsChannelGloballyPaused(Channel channel);

    }
}