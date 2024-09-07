using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using Match3.Game.Gameplay.Tiles;

namespace Match3.Game.Gameplay.SubSystems.WinSequence
{
    public interface WinSequencePresentationHandler : PresentationHandler
    {
        void HandleActivation(Action onReadyForActivation);
        void HandleSkipping();
        void PlaceExplosives(List<Tile> tiles, List<CellStack> targets, Action<int> onPlaced, Action onCompleted);
        void HandleFinishing();
    }

    public interface WinSequenceKeyType : KeyType
    {

    }

    [Before(typeof(ExplosionManagement.ExplosionActivationSystem))]
    [Before(typeof(RainbowMechanic.RainbowActivationSystem))]
    public class LevelWinSequenceSystem : GameplaySystem
    {
        ActivatingState activatingState = new ActivatingState();
        PlacingState placingState = new PlacingState();
        WaitingState waitingForStabilityState = new WaitingState();
        IdleState idleState = new IdleState();


        TileFactory tileFactory;
        State currentState;

        WinSequencePresentationHandler presentationHandler;

        public LevelWinSequenceSystem(GameplayController gameplayController) : base(gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<WinSequencePresentationHandler>();
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        public override void OnActivated()
        {
            activatingState.Setup(this, gameplayController);
            placingState.Setup(this, gameplayController);
            waitingForStabilityState.Setup(this, gameplayController);
            idleState.Setup(this, gameplayController);

            SetState(idleState);
            presentationHandler.HandleActivation(() => SetState(activatingState));
        }

        // TODO: Make this private later;
        public Tile CreateAnExplosive()
        {
            if (UnityEngine.Random.value > 0.66)
                return tileFactory.CreateBombTile();
            else
                return tileFactory.CreateRocketTile();
        }

        private void SetState(State state)
        {
            this.currentState = state;
        }

        public override void Update(float dt)
        {
            currentState.Update(dt);
        }

        public void OnActivationFinished()
        {
            if (placingState.IsDepleted())
                waitingForStabilityState.SetData(1, Finish);
            else
                waitingForStabilityState.SetData(1, () => SetState(placingState));

            SetState(waitingForStabilityState);
        }

        public void OnPlacementFinished()
        {
            if (currentState != placingState)
                return;
            waitingForStabilityState.SetData(2.5f, () => SetState(activatingState));

            SetState(waitingForStabilityState);
        }

        public void Skip()
        {
            presentationHandler.HandleSkipping();

            waitingForStabilityState.SetData(0, Finish);

            SetState(waitingForStabilityState);

            var data = GetFrameData<LevelWinningData>();
            for (int i = 0; i < placingState.RemainingMoves(); ++i)
                data.calculatedScoringTiles.Add(CreateAnExplosive());

            foreach(var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if(cellStack.HasTileStack() && cellStack.CurrentTileStack().IsDepleted() == false)
                {
                    var top = cellStack.CurrentTileStack().Top();
                    if (top is ExplosiveTile || top is Rainbow)
                        data.calculatedScoringTiles.Add(top);
                }
        }

        void Finish()
        {
            presentationHandler.HandleFinishing();
            gameplayController.DeactivateSystem<LevelWinSequenceSystem>();
            GetFrameData<LevelWinningData>().isWinSequenceEnded = true;   
        }
    }
}