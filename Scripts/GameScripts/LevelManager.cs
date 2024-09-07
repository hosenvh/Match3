using System;
using Match3.Foundation.Unity;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private ResourceBoardConfigAsset[] levelConfigs;
    private int lastPublishedLevelIndex = 0;

    public void SetLevels(ResourceBoardConfigAsset[] levelConfigs)
    {
        this.levelConfigs = levelConfigs;
    }

    public void SetLastPublishedLevelIndex(int index)
    {
        this.lastPublishedLevelIndex = index;
    }

    public BoardConfig GetLevelConfig(int index)
    {
        return levelConfigs[index].Load();
    }

    public ResourceBoardConfigAsset[] GetAllLevelConfigs()
    {
        return levelConfigs;
    }

    public int TotalLevels()
    {
        return levelConfigs.Length;
    }

    public int LastPublishedLevelIndex()
    {
        return lastPublishedLevelIndex;
    }
}
