using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeganX;
using Match3.Game.Gameplay.Tiles;
using PandasCanPlay.HexaWord.Utility;
using Match3.Game.Gameplay.Core;
using System;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using Match3.Data;
using Match3.Data.Configuration;
using Match3;
using Match3.Data.Unity.PersistentTypes;
using Match3.Game;
using UnityEngine.Scripting;
using UnityEngine.Serialization;
using static BoardConfig;
using Match3.Game.Gameplay.Initialization;

public enum EditType { None, MainBorder, RiverBorder, Items }

public enum EditorItems
{
    Ground_Cell, Block_Cell, River_Cell, Block_Crossable_Cell, River_Crossable_Cell,
    EntranceCell, EntranceMugCell, Mug_ExitCell, Vase, Grass, Portal_Enter, Portal_Exit, Rock,
    Waste, Mug_Object, ExtraMove, MatchItem, Ice, Chain, Wood, Wall_Horizontal, Wall_Vertical, Explosive_Rocket, Explosive_Bomb, Explosive_Dynamite, Explosive_TNT, Rainbow, Forbidden_Rainbow
}

public enum ItemColorType { None, Blue, Green, Pink, Purple, Red, Yellow }
public class BoardSpecificItemConfig
{
    public int cellIndex;

    [Preserve]
    public BoardSpecificItemConfig(int cellIndex)
    {
        this.cellIndex = cellIndex;
    }
}

[Serializable]
public class VacuumCleanerBoardSpecificConfig : BoardSpecificItemConfig
{
    public int targetAmount;
    public Direction direction;
    public int priority;

    public VacuumCleanerBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
        this.direction = Direction.Up;
        this.targetAmount = 1;
        this.priority = 1;
    }
}

[Serializable]
public class TableClothBoardSpecificConfig : BoardSpecificItemConfig
{
    [System.Serializable]
    public class TargetData
    {
        public bool isActive;
        [TypeAttribute(typeof(Tile), false)]
        public string tileType;
        public TileColor tileColor;
        public int targetAmount;

        public TargetData(bool isActive, Type tileType, TileColor tileColor, int targetAmount)
        {
            this.isActive = isActive;
            this.tileType = tileType.AssemblyQualifiedName;
            this.tileColor = tileColor;
            this.targetAmount = targetAmount;
        }

        public TableClothMainTile.TargetHandler ToLogical()
        {
            return new TableClothMainTile.TargetHandler(isActive, ExtractTargetType(), targetAmount);
        }

        public GoalTargetType ExtractTargetType()
        {
            var rawType = Type.GetType(tileType);
            if (typeof(ColoredBead).IsAssignableFrom(rawType))
                return new ColoredGoalType(rawType, tileColor);
            else
                return new SimpleGoalType(rawType);
        }
    }

    public TargetData firstTargetData = new TargetData(true, typeof(ColoredBead), TileColor.Green, 1);
    public TargetData secondTargetData = new TargetData(false, typeof(ColoredBead), TileColor.Green, 0);

    public Vector2Int size = new Vector2Int(1,1);

    public TableClothBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
    }
}

[Serializable]
public class BalloonBoardSpecificConfig : BoardSpecificItemConfig
{
    public BalloonBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
    }

    [SerializeField] private List<Vector2Int> tiedTilesPositions = new List<Vector2Int>();

    public HashSet<Vector2Int> GetTiedTilesPositions()
    {
        return new HashSet<Vector2Int>(tiedTilesPositions);
    }

    public void AddTilesPositions(Vector2Int position)
    {
        tiedTilesPositions.Add(position);
    }

    public void RemoveTilesPositions(Vector2Int position)
    {
        tiedTilesPositions.Remove(position);
    }
}

[Serializable]
public class GrassSackBoardSpecificConfig : BoardSpecificItemConfig
{

    public List<GrassSackMainTile.ArtifactData> artifactsData = new List<GrassSackMainTile.ArtifactData>();

    public GrassSackBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
    }
}

[Serializable]
public class GasCylinderBoardSpecificConfig : BoardSpecificItemConfig
{
    public int startCountdown;

    [Preserve]
    public GasCylinderBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
        this.startCountdown = 1;
    }
}

[Serializable]
public class CatPathCellBoardSpecificConfig : BoardSpecificItemConfig
{
    public int nextCellIndex = -1;

    [Preserve]
    public CatPathCellBoardSpecificConfig(int cellIndex) : base(cellIndex)
    {
    }
}

[Serializable]
public class TileSourceCreatorSpecificConfig : BoardSpecificItemConfig
{
    [TypeAttribute(typeof(Tile), false)]
    public List<string> sourceTypes = new List<string>();

    [Preserve]
    public TileSourceCreatorSpecificConfig(int cellIndex) : base(cellIndex)
    {
    }

    public bool ContainsTileSourceCreatorOfType<T>() where T : Tile
    {
        return sourceTypes.Find(types => Type.GetType(types) == typeof(T)) != null;
    }
}


[CreateAssetMenu()]
public class BoardConfig : ScriptableObject
{
    public LevelConfig levelConfig;
    public ChallengeLevelConfig challengeLevelConfig;
    
    [Space(8)]


    public int boardWidth = 10;
    public int boardHeight = 1;
    [InspectorButton(200f, "Recreate board", nameof(RecreateBoard), true)]
    public int v0;

    public List<StatueConfig> statues;
    public List<StatueConfig> statuesWithRocket;
    [FormerlySerializedAs("vacuumCleanerDynamicConfigs")]
    public List<VacuumCleanerBoardSpecificConfig> vacuumCleanerBoardSpecificConfigs;
    [FormerlySerializedAs("tableClothBoardConfigs")]
    public List<TableClothBoardSpecificConfig> tableClothBoardSpecificConfigs;
    public int TEMPORARY_InBoardGasCylinderStartCountdown;
    public int generationGasCylinderStartCountdown;
    [FormerlySerializedAs("gasCylinderDynamicConfigs")]
    public List<GasCylinderBoardSpecificConfig> gasCylinderBoardSpecificConfigs;
    public List<GrassSackBoardSpecificConfig> grassSackBoardSpecificConfigs;
    public List<CatPathCellBoardSpecificConfig> catPathBoardSpecificConfigs;
    public List<TileSourceCreatorSpecificConfig> tileSourceCreatorSpecificConfigs;
    public List<BalloonBoardSpecificConfig> balloonBoardSpecificConfigs;

    public CellConfig[] cells;

    private bool isChallengeLevelActive = false;
    public PersistentBool IsChallengeLevelPlayed;
    
    
    #region public methods

    private void OnEnable()
    {
        isChallengeLevelActive = false;
        IsChallengeLevelPlayed = new PersistentBool($"ChallengeLevelPlayed_{name}");
    }

    public CellConfig GetCellConfig(int column, int row)
    {
        return cells[column * boardWidth + row];
    }


    public int GetLevelMaxMove()
    {
        if (isChallengeLevelActive)
            return challengeLevelConfig.challengeMoveCount;
        
        return levelConfig.maxMove;
    }

    public bool HasChallengeLevel()
    {
        return challengeLevelConfig.challengeMoveCount > 0 && !IsChallengeLevelPlayed.Get();
    }
    
    public void TrySetChallengeLevelActive()
    {
        isChallengeLevelActive = HasChallengeLevel();
    }
    
    public bool IsChallengeLevelActive()
    {
        return isChallengeLevelActive;
    }
    
    public void SetChallengeLevelPlayed()
    {
        IsChallengeLevelPlayed.Set(true);
        isChallengeLevelActive = false;
    }
    

    #endregion

    #region nested classes

    [System.Serializable]
    public struct TileGenerationInfo
    {
        [TypeAttribute(typeof(Tile), false)]
        public string type;
        public int inLevelLimit;
        public int inBoardLimit;
    }


    [System.Serializable]
    public struct ColorChanceInfo
    {
        public TileColor color;
        public float wieght;
    }

    // TODO: Integrate GoalInfo && GoalTypeInfo
    [System.Serializable]
    public struct GoalInfo
    {
        [TypeAttribute(type: typeof(GoalObject), includeAbstracts : false, showPartialName : true)]
        public string goalType;

        public TileColor color;

        public int goalAmount;

        public GoalTargetType ExtractGoalType()
        {
            var rawType = Type.GetType(goalType);

            if (typeof(ColoredBead).IsAssignableFrom(rawType))
                return new ColoredGoalType(rawType, color);
            else
                return new SimpleGoalType(rawType);

        }
    }

    [System.Serializable]
    public struct GoalTypeInfo
    {
        [TypeAttribute(new System.Type[] { typeof(Cell), typeof(Tile), typeof(CellAttachment) }, includeAbstracts: false, showPartialName: true)]
        public string goalType;

        public TileColor color;

        public GoalTargetType ExtractGoalType()
        {
            var rawType = Type.GetType(goalType);

            if (typeof(ColoredBead).IsAssignableFrom(rawType))
                return new ColoredGoalType(rawType, color);
            else
                return new SimpleGoalType(rawType);

        }
    }


    [Serializable]
    public class RocketBoxPriorityConfig
    {
        public string inspectorName;
        public int priority;
        public TargetFinderConfig targetFinder;
        public GameplayConditionConfig gameplayCondition;
    }

    [Serializable]
    public class BuoyantGenerationData
    {
        public int inBoardMax;
    }

    [Serializable]
    public class DuckGenerationData
    {
        public int inBoardMax;
        public int inLevelMax;
    }

    [Serializable]
    public class DuckItemData
    {
        [TypeAttribute(typeof(Tile), false)]
        public List<string> childTilesTypes = new List<string>();
    }

    [System.Serializable]
    public class LevelConfig
    {
        public DifficultyLevel difficultyLevel;
        public int maxMove = 30, maxTime = 0;
        public float[] rainbowAddValues;
        public List<GoalInfo> goals;

        public List<ColorChanceInfo> colorChances;
        public List<ColorChanceInfo> colordirtinessChances;

        public List<TileGenerationInfo> tileGenerationLimits;

        public bool enableLemonadeSideFalling;

        public List<RocketBoxPriorityConfig> rocketBoxPriorities;

        public BuoyantGenerationData buoyantGenerationData;

        public DuckGenerationData duckGenerationData;

        public DuckItemData duckItemData;
    }

    [Serializable]
    public class ChallengeLevelConfig
    {
        public int challengeMoveCount;

        // Initialization values comes from assumption of having fix rewards for challenge levels
        public InfiniteLifeReward infiniteLifeReward = new InfiniteLifeReward(900);
        public CoinReward coinReward = new CoinReward(300);
        public InfiniteDoubleBombBoosterReward infiniteDoubleBombBoosterReward1 =
            new InfiniteDoubleBombBoosterReward(900);

        public List<Reward> GetRewards()
        {
            List<Reward> rewards = new List<Reward>();
            rewards.Add(infiniteLifeReward);
            rewards.Add(coinReward);
            rewards.Add(infiniteDoubleBombBoosterReward1);
            return rewards;
        }
    }

    
    
    #endregion

    #region editor
    public void RecreateBoard(object sender)
    {
        var firstCell = cells[0];

        cells = new CellConfig[boardWidth * boardHeight];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new CellConfig();
            cells[i].cItemsArr = new BaseItemConfig[1];
            cells[i].cItemsArr[0] = firstCell.cItemsArr[0];
        }

#if UNITY_EDITOR
	UnityEditor.EditorUtility.SetDirty(this);
#endif

    }
    public int Width()
    {
        return boardWidth;
    }

    public int Height()
    {
        return cells.Length / boardWidth;
    }


    #endregion

    
}