

using System;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystems.WinSequence;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{


    public interface LevelEndingPresentationHandler : PresentationHandler
    {
        void HandleGameLost(StopConditinon losingCause);
        void HandleGamePreWin(Action onComepleted);
        void HandleGameWin(int finalScore);

        // TODO: The name of the parameter doesn't match well. Try to find a better interface.
        void HandleRewardDoubling(int addedScore);
    }


    public class LevelEndingSystem : GameplaySystem
    {
        /** NOTE: The WaitingOneFrameForFinalResult is needed because in some situations the matching systems waits for physics systems,
         * and physics systems is executed after matching system.
         * */
        enum State { WaitingForStability , WaitingForWinSeqence, WaitingOneFrameForFinalResult, Idle}

        GridIterator<CellStack> cellBoardIterator;

        LevelEndingPresentationHandler presentationHandler;

        State currentState;

        int finalLevelScore;


        public LevelEndingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            cellBoardIterator = gameplayController.GameBoard().DefaultCellBoardIterator();

            presentationHandler = gameplayController.GetPresentationHandler<LevelEndingPresentationHandler>();

            currentState = State.WaitingForStability;
        }

        public override void Reset()
        {
            currentState = State.WaitingForStability;
            finalLevelScore = 0;
        }

        public override void Update(float dt)
        {
            switch (currentState)
            {
                case State.WaitingForStability:
                    ProcessWaitingForStability();
                    break;
                case State.WaitingForWinSeqence:
                    ProcessWaitingForWinSequence();
                    break;
                case State.WaitingOneFrameForFinalResult:
                    ProcessWaitingOneFrameForFinalResult();
                    break;
            }  
        }

        public void DoubleTheLevelReward()
        {
            var addedScore = finalLevelScore;
            finalLevelScore += addedScore;
            presentationHandler.HandleRewardDoubling(addedScore);
        }

        private void ProcessWaitingForWinSequence()
        {
            if(GetFrameData<LevelWinningData>().isWinSequenceEnded)
            {
                gameplayController.DeactiveAllSystems();
                finalLevelScore = gameplayController.GetSystem<LevelFinalScoringSystem>().CurrentScore();
                presentationHandler.HandleGameWin(finalLevelScore);
                ServiceLocator.Find<EventManager>().Propagate(new LevelScoredEvent(finalLevelScore), this);
            }
        }

        private void ProcessWaitingForStability()
        {
            if (IsBoardStable())
                currentState = State.WaitingOneFrameForFinalResult;
            
        }

        private void ProcessWaitingOneFrameForFinalResult()
        {
            if (IsBoardStable())
                HandleLevelResult();
            
            else
                currentState = State.WaitingForStability;
        }

        private void HandleLevelResult()
        {
            var result = gameplayController.GetSystem<LevelStoppingSystem>().GetLevelResult();
            var stoppingCause = gameplayController.GetSystem<LevelStoppingSystem>().StoppingCause();
            gameplayController.DeactiveAllSystems();

            if (result == LevelResult.Win)
            {
                presentationHandler.HandleGamePreWin(() => gameplayController.ActivateWinSequenceSystems());
                currentState = State.WaitingForWinSeqence;
            }
            else
            {
                presentationHandler.HandleGameLost(stoppingCause);
                currentState = State.Idle;
            }
            
            ServiceLocator.Find<EventManager>().Propagate(new LevelEndedEvent(result), this);
        }

        bool IsBoardStable()
        {
            if (GetFrameData<GeneratedTileStacksData>().tileStacks.Count > 0)
                return false;

            foreach (var element in cellBoardIterator)
                if (IsStable(element.value) == false)
                    return false;

            return true;
        }

        private bool IsStable(CellStack cellStack)
        {
            return QueryUtilities.IsFullyFree(cellStack);
        }

        public int FinalLevelScore()
        {
            return finalLevelScore;
        }

    }
}