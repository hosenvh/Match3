using System;
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using Random = UnityEngine.Random;
using Match3;
using Match3.Game.NeighborhoodChallenge;
using Match3.Utility.GolmoradLogging;


public class NeighborhoodChallengeLevelSelector
{

    private int selectionLevelOffset;
    private float totalWeights;
    private float normalDifficulty;
    private float[] levelsDifficulty;
    private float defaultLevelDifficulty;

    private float[] levelsWeight;

    private int selectedLevelIndex;

    private int SelectedLevelIndex
    {
        get => selectedLevelIndex;
        set
        {
            Base.gameManager.profiler.NeighborhoodChallengeSelectedLevel = value;
            selectedLevelIndex = value;
        }
    }


    public NeighborhoodChallengeLevelSelector()
    {
        ServiceLocator.Find<ConfigurationManager>().Configure(this);
    }

    public void InitializeSelection()
    {
        var currentSelectedLevel = Base.gameManager.profiler.NeighborhoodChallengeSelectedLevel;
        SelectedLevelIndex = currentSelectedLevel == -1 ? GetNewLevelIndex() : currentSelectedLevel;
    }

    public void ForceSelectNewLevel()
    {
        SelectedLevelIndex = GetNewLevelIndex();
    }

    public int SelectedLevel()
    {
        return SelectedLevelIndex;
    }

    public void SetSelectedLevel(int index)
    {
        SelectedLevelIndex = index;
    }
    
    private int GetNewLevelIndex()
    {
        CalculateLevelsWeight();
        return WeightedRandom(levelsWeight, totalWeights) + selectionLevelOffset;
    }

    public void SetSelectionLevelOffset(int offset)
    {
        selectionLevelOffset = offset;
    }
    
    public void SetLevelsDifficulty(float[] difficulties)
    {
        levelsDifficulty = difficulties;
    }

    public void SetDifficultyData(float normalDifficulty, float defaultDifficulty)
    {
        this.normalDifficulty = normalDifficulty;
        this.defaultLevelDifficulty = defaultDifficulty;
    }
    
    private void CalculateLevelsWeight()
    {
        try
        {
            totalWeights = 0;

            var lastUnlockedLevel = Base.gameManager.profiler.LastUnlockedLevel;
            levelsWeight = new float[lastUnlockedLevel - selectionLevelOffset];

            for (var i = selectionLevelOffset; i < lastUnlockedLevel; ++i)
            {
                var weight = 1 - Mathf.Abs(normalDifficulty - LevelDifficultyOf(i));
                totalWeights += weight;
                levelsWeight[i-selectionLevelOffset] = weight;
            }
        }
        catch (Exception e)
        {
            DebugPro.LogException<NeighborHoodChallengeLogTag>(
                $"{e.Message} \n Last Unlocked Level : {Base.gameManager.profiler.LastUnlockedLevel}  levelsDifficulty Length : {levelsDifficulty.Length}");
        }
    }

    private float LevelDifficultyOf(int levelIndex)
    {
        if (levelIndex < levelsDifficulty.Length)
            return levelsDifficulty[levelIndex];
        else
            return defaultLevelDifficulty;
        
    }

    private int WeightedRandom(float[] wCollection, float totalWeight)
    {
        Random.InitState((int)DateTime.UtcNow.Ticks);
        float rand = Random.Range(0, totalWeight);
        for (var i = 0; i < wCollection.Length; ++i)
        {
            rand -= wCollection[i];
            if (rand <= 0) return i;
        }

        return 0;
    }
}
