
using System;
using System.Collections.Generic;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData;
using Match3.Game.Gameplay.SubSystemsData.FrameData.LevelEnding;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;

namespace Match3.Game.Gameplay.SubSystems.LevelEnding
{

    public enum DifficultyLevel
    {
        Normal, Hard, VeryHard
    }


    public interface FinalScorePresentationHandler : PresentationHandler
    {
        void HandleScoreAddition(int number, Tile tile);
    }


    [After(typeof(DestructionManagement.DestructionSystem))]
    public class LevelFinalScoringSystem : GameplaySystem
    {
        int currentScore;
        FinalScorePresentationHandler presentationHandler;
        bool canStartScoring = false;

        DifficultyLevel currentDifficulty;
        Dictionary<DifficultyLevel, int> baseDifficultyScores = new Dictionary<DifficultyLevel, int>();

        public LevelFinalScoringSystem(GameplayController gameplayController, DifficultyLevel currentDifficulty) : base(gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<FinalScorePresentationHandler>();
            this.currentDifficulty = currentDifficulty;

            ServiceLocator.Find<ConfigurationManager>().Configure(this);
        }

        public override void OnActivated()
        {
            canStartScoring = false;
            currentScore = LevelDifficultyBaseScore();

            if (gameplayController.GetSystem<LevelStoppingSystem>().GetLevelResult() == LevelResult.Lose)
                gameplayController.DeactivateSystem<LevelFinalScoringSystem>();
        }

        public override void Update(float dt)
        {
            if(canStartScoring == false)
            {
                canStartScoring = GetFrameData<StabilityData>().wasStableLastChecked;
                return;
            }
            foreach (var tile in GetFrameData<DestroyedObjectsData>().tiles)
                AddScore(ScoreFor(tile), tile);

            var skippedScore = 0f;
            foreach (var tile in GetFrameData<LevelWinningData>().calculatedScoringTiles)
                skippedScore += ScoreFor(tile) * 1.15f;

            currentScore += (int) skippedScore;

        }

        private void AddScore(int n, Tile tile)
        {
            currentScore += n;

            if(n > 0)
                presentationHandler.HandleScoreAddition(n, tile);
        }

        public void SetBaseDifficultyScore(DifficultyLevel difficulty, int baseScore)
        {
            baseDifficultyScores[difficulty] = baseScore;
        }

        private int LevelDifficultyBaseScore()
        {
            return baseDifficultyScores[currentDifficulty];
        }

        private int ScoreFor(Tile tile)
        {
            if (tile is Rocket)
                return 1;
            if (tile is Bomb)
                return 2;
            if (tile is Dynamite)
                return 3;
            if (tile is TNTBarrel)
                return 4;

            if (tile is Rainbow)
                return 2;

            return 0;
        }

        public int CurrentScore()
        {
            return currentScore;
        }
    }
}