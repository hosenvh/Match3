using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.SubSystems.TileRandomPlacement;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.LevelContinuing
{
    public class LevelResumeWithExtraMoveEvent : GameEvent
    {
        public readonly int extraMoveCount;
        public readonly int cost;

        public LevelResumeWithExtraMoveEvent(int cost, int extraMoveCount)
        {
            this.cost = cost;
            this.extraMoveCount = extraMoveCount;
        }
    }

    public class LevelResumeWithAdsExtraMoveEvent : GameEvent { }


    public class LevelContinuingStage
    {
        public readonly int neededCoins;

        public readonly int additionalMoves;

        public readonly List<Type> tilesToCreate = new List<Type>();

        public LevelContinuingStage(int neededCoins, int additionalMoves, List<Type> tilesToCreate)
        {
            this.neededCoins = neededCoins;
            this.additionalMoves = additionalMoves;
            this.tilesToCreate = tilesToCreate;
        }
    }

    public class LevelContinuingSystem : GameplaySystem
    {

        List<LevelContinuingStage> stages = new List<LevelContinuingStage>();
        int currentStageIndex;

        public LevelContinuingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public override void Start()
        {
            currentStageIndex = 0;
        }

        public override void Update(float dt)
        {
            
        }

        public void TryContinueLevel(Action onSuccess, Action onNotEnoughCoin)
        {
            var currentStage = CurrentStage();

            if (Base.gameManager.profiler.CoinCount < currentStage.neededCoins)
            {
                onNotEnoughCoin();
            }
            else
            {
                Base.gameManager.profiler.ChangeCoin(-currentStage.neededCoins, "resume game");
                ContinueLevel(currentStage);
                onSuccess();
            }
        }

        private void ContinueLevel(LevelContinuingStage currentStage)
        {
            IncrementStage();

            AddMoves(currentStage.additionalMoves);

            AddTilesToCreate(currentStage.tilesToCreate);

            gameplayController.ResetGame();

            ServiceLocator.Find<EventManager>().Propagate(new LevelResumeWithExtraMoveEvent(currentStage.neededCoins, currentStage.additionalMoves), this);
        }

        private void AddMoves(int additionalMoves)
        {
            gameplayController.GetSystem<LevelStoppingSystem>().FindStoppingCondition<MovementStopCondition>().AddExtraMove(additionalMoves);

            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTile<GasCylinder>(cellStack))
                    FindTile<GasCylinder>(cellStack).AddCountdown(additionalMoves);
        }

        public void ContinueLevelWithExtraMoves(int extraMoves)
        {
            AddMoves(extraMoves);
            gameplayController.ResetGame();
        }

        private void AddTilesToCreate(List<Type> tilesToCreate)
        {
            var tileRandomPlacer = gameplayController.GetSystem<TileRandomPlacementSystem>().GetTilePlacer<DefaultTileRandomPlacer>();
            foreach (var type in tilesToCreate)
                tileRandomPlacer.AddTilesToPlace(type);
        }

        private void IncrementStage()
        {
            currentStageIndex++;
        }

        public LevelContinuingStage CurrentStage()
        {
            return stages[Math.Min(currentStageIndex, stages.Count - 1)];
        }

        public void AddStage(LevelContinuingStage stage)
        {
            stages.Add(stage);
        }
    }
}