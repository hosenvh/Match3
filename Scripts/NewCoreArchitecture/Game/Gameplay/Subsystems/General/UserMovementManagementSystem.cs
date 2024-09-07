

using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;

namespace Match3.Game.Gameplay.SubSystems.General
{
    public struct UserMovedEvent : GameEvent
    {
    }

    public class UserMovementManagementSystem : GameplaySystem
    {
        public UserMovementManagementSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public override void Update(float dt)
        {
            var moves = 0;
            moves += GetFrameData<SuccessfulUserActivationData>().targets.Count;
            moves += GetFrameData<SuccessfulSwapsData>().data.Count;

            if (moves > 0)
            {
                GetFrameData<UserMovementData>().moves = moves;
                ServiceLocator.Find<EventManager>().Propagate(new UserMovedEvent(), this);
            }
        }
    }
}