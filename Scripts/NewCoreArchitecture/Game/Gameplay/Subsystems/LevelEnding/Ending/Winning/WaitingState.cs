using System;
using Match3.Game.Gameplay.SubSystemsData.FrameData;

namespace Match3.Game.Gameplay.SubSystems.WinSequence
{
    public class IdleState : State
    {
        public override void Update(float dt)
        {
            
        }

        protected override void InternalSetup()
        {
            
        }
    }
    public class WaitingState : State
    {
        float remainingTime;
        StabilityData stabilityData;
        Action onStabilityAction;


        protected override void InternalSetup()
        {
            stabilityData = system.GetFrameData<StabilityData>();
        }

        public void SetData(float waitingDuration , Action action)
        {
            this.onStabilityAction = action;
            this.remainingTime = waitingDuration;
        }


        public override void Update(float dt)
        {
            if (stabilityData.wasStableLastChecked == false)
                return;
            remainingTime -= dt;
            if(remainingTime <= 0)
                onStabilityAction();
            
        }

    }
}