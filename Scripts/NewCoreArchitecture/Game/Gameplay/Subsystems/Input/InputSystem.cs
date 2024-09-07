using Match3.Game.Gameplay.SubSystemsData.SessionData;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Input
{
    public class InputSystem : GameplaySystem
    {
        Queue<GameplayInputCommand> inputQueue = new Queue<GameplayInputCommand>();



        public InputSystem(GameplayController gameplayController) : base(gameplayController)
        {
        }

        public void Apply(GameplayInputCommand command)
        {
            inputQueue.Enqueue(command);
        }


        public override void Update(float dt)
        {
            if(GetSessionData<InputControlData>().IsLocked() || GetSessionData<StabilityControlData>().shouldForceStablize)
                inputQueue.Clear();
            
            while (inputQueue.Count != 0)
                Excute(inputQueue.Dequeue());
        }

        private void Excute(GameplayInputCommand inputCommand)
        {
            inputCommand.Execute(gameplayController);
        }
    }
}