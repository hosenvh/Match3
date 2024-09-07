using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Initialization;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.TileGeneration;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;
using Match3.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Match3.Utility.GolmoradLogging;
using UnityEditor;
using UnityEngine;
using Match3.Game.Gameplay.SubSystems.LevelEnding;
using static Match3.Game.Gameplay.Initialization.GoalInfoExtaractor;

namespace Match3.EditorTools.Editor.Menus.Verification
{
    public class LevelsConfigVerificationWindow : EditorWindow
    {
        private struct VerificationInfo
        {
            public readonly string name;
            public readonly Action<BoardConfig, GameBoard> verificationAction;

            public VerificationInfo(string name, Action<BoardConfig, GameBoard> verificationAction)
            {
                this.name = name;
                this.verificationAction = verificationAction;
            }
        }

        private string levelsPath = "Assets/Resources/Boards";
        private int minLevelIndexToInclude;
        private int maxLevelIndexToInclude;
        private List<SpecialTileGenerationInfo> tileGenerationLimits;

        private readonly List<VerificationInfo> verificationInfos = new List<VerificationInfo>();

        [MenuItem("Golmorad/Verification/Levels Config Verifier", priority = 100)]
        public static void ShowWindow()
        {
            GetWindow(typeof(LevelsConfigVerificationWindow));
        }

        public LevelsConfigVerificationWindow()
        {
            verificationInfos.Add(new VerificationInfo(name: "Verify TableCloth", verificationAction: VerifyTableCloth));
            verificationInfos.Add(new VerificationInfo(name: "Verify Garden", verificationAction: VerifyGardens));
            verificationInfos.Add(new VerificationInfo(name: "Verify Color Limits", verificationAction: VerifyColorLimits));
            verificationInfos.Add(new VerificationInfo(name: "Verify Lemonades", verificationAction: VerifyLemonades));
            verificationInfos.Add(new VerificationInfo(name: "Verify Nuts", verificationAction: VerifyNuts));
            verificationInfos.Add(new VerificationInfo(name: "Verify Artifacts", verificationAction: VerifyArtifacts));
            verificationInfos.Add(new VerificationInfo(name: "Verify Gem goals", verificationAction: VerifyGameGoals));
            verificationInfos.Add(new VerificationInfo(name: "Verify Rivers", verificationAction: VerifyRivers));
            verificationInfos.Add(new VerificationInfo(name: "Verify Ghandoons", verificationAction: VerifyFireflyJars));
            verificationInfos.Add(new VerificationInfo(name: "Verify Invisible Grasses", verificationAction: VerifyInvisibleGrasses));
            verificationInfos.Add(new VerificationInfo(name: "Check multiple colors in cell", verificationAction: VerifyMultipleColors));
            verificationInfos.Add(new VerificationInfo(name: "Check multiple type in cell", verificationAction: VerifyMultipleItemTypeInOnePlace));
            verificationInfos.Add(new VerificationInfo(name: "Check Mover Count", verificationAction: VerifyMoveCount));
            verificationInfos.Add(new VerificationInfo(name: "Check ExplosionFactor", verificationAction: VerifyExplosionFactor));
            verificationInfos.Add(new VerificationInfo(name: "Check Goals which Are Not Created", verificationAction: VerifyGoalsWhichAreNotCreated));
            verificationInfos.Add(new VerificationInfo(name: "Check Goal Tiles to Have Creator", verificationAction: VerifyGoalsWhichAreCreated));
            verificationInfos.Add(new VerificationInfo(name: "Check the ColoredBead Goals", verificationAction: VerifyColoredBeadGoals));
            verificationInfos.Add(new VerificationInfo(name: "Verify Rocket Boxes", verificationAction: VerifyRocketBoxes));
            verificationInfos.Add(new VerificationInfo(name: "Verify Rocket Pockets", verificationAction: VerifyRocketPockets));
            verificationInfos.Add(new VerificationInfo(name: "Verify Gas Cylinders", verificationAction: VerifyGasCylinders));
            verificationInfos.Add(new VerificationInfo(name: "Verify ColorBeads", verificationAction: VerifyColorBeads));
            verificationInfos.Add(new VerificationInfo(name: "Verify Cells Overlay", verificationAction: VerifyCellsOverlay));
            verificationInfos.Add(new VerificationInfo(name: "Verify Tiles Overlay", verificationAction: VerifyTilesOverlay));
            verificationInfos.Add(new VerificationInfo(name: "Verify Hardened Honies", verificationAction: VerifyHardenedHoney));
            verificationInfos.Add(new VerificationInfo(name: "Verify Mandatory Goal Tiles", verificationAction: VerifyMandatoryGoalTiles));
            verificationInfos.Add(new VerificationInfo(name: "Verify Tile Generators", verificationAction: VerifyTileGenerators));
            verificationInfos.Add(new VerificationInfo(name: "Verify TileSource Creators", verificationAction: VerifyTileSourceVerifier));
            verificationInfos.Add(new VerificationInfo(name: "Verify Challenge Levels", verificationAction: VerifyChallengeLevels));
        }

        private void OnGUI()
        {
            GUILayout.Label(levelsPath);
            if (GUILayout.Button("Select levels folder"))
                levelsPath = AssetEditorUtilities.RelativeAssetPath(EditorUtility.OpenFolderPanel("Select", levelsPath, ""));

            minLevelIndexToInclude = EditorGUILayout.IntField("Min LevelIndex To Include", minLevelIndexToInclude);
            maxLevelIndexToInclude = EditorGUILayout.IntField("Max LevelIndex To Include", maxLevelIndexToInclude);

            foreach (var info in verificationInfos)
                if (GUILayout.Button(info.name))
                    CheckLevelsFor(info);


            if (GUILayout.Button("Verify All"))
                VerifyAll();
        }

        private void VerifyAll()
        {
            foreach (var info in verificationInfos)
                CheckLevelsFor(info);
        }

        private void CheckLevelsFor(VerificationInfo info)
        {
            List<BoardConfig> boardsConfigs = AssetEditorUtilities.FindAssetsByType<BoardConfig>(path: levelsPath);


            if (ServiceLocator.IsInited() == false)
                ServiceLocator.Init();

            ServiceLocator.Replace<CellFactory>(new MainCellFactory());
            ServiceLocator.Replace<TileFactory>(new MainTileFactory());
            ServiceLocator.Replace<CellAttachmentFactory>(new MainCellAtttachmentFactory());

            for (int i = minLevelIndexToInclude; i < boardsConfigs.Count && i <= maxLevelIndexToInclude; i++)
            {
                var config = boardsConfigs[i];
                try
                {
                    var gameBoard = new GameBoardCreator().CreateFrom(config);
                    tileGenerationLimits = new TileGenerationInfoExtractor().ExtractFrom(config);
                    info.verificationAction(config, gameBoard);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Could not create/verify {config.name}. Exception: {e}. StackTrace: {e.StackTrace}", config);
                }
            }

            Debug.Log($"Check Ended For {info.name}.");
        }

        private void VerifyNuts(BoardConfig config, GameBoard gameBoard)
        {
            var nutGoal = new GoalInfoExtaractor().Extract(config).Find(x => x.goalType.GoalObjectType() == typeof(Nut));
            if (nutGoal.goalType == null)
                return;

            int nutGoalAmount = nutGoal.goalAmount;
            List<Nut> nutTiles = GetTilesOfTypeInGameBoard<Nut>(gameBoard);
            List<NutTileSource> nutTileSource = GetTileSourcesOfTypeInGameBoard<NutTileSource>(gameBoard);
            BoardConfig.TileGenerationInfo nutTileGeneration = config.levelConfig.tileGenerationLimits.Find(x => GetType(x.type) == typeof(Nut));

            LogErrorIfWeHaveNotEnoughNut(config, nutGoalAmount, nutTileGeneration, nutTiles, nutTileSource);
        }

        private void LogErrorIfWeHaveNotEnoughNut(BoardConfig config, int nutGoalAmount, BoardConfig.TileGenerationInfo nutTileGeneration, List<Nut> nutTiles, List<NutTileSource> nutTileSource)
        {
            if (nutTileSource.Count >= 0)
            {
                if (nutGoalAmount > nutTileGeneration.inLevelLimit)
                    ErrorOn(config, "the Level Limit generation of TileSource \"Nut\" is less than goal amount.");
            }
            else
            {
                if (nutGoalAmount > nutTiles.Count)
                    ErrorOn(config, "there is not enough \"Nut\" tile to reach the goal.");
            }
        }

        private void VerifyLemonades(BoardConfig config, GameBoard gameBoard)
        {
            var lemonades = new List<Lemonade>();
            var lemonadeSources = new List<LemonadeTileSource>();
            var lemonadeSinks = new List<LemonadeSink>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                    {
                        if (tile is Lemonade lemonade)
                            lemonades.Add(lemonade);
                    }
                if (cellStack.HasAttachment<LemonadeTileSource>())
                    lemonadeSources.Add(cellStack.GetAttachment<LemonadeTileSource>());
                if (cellStack.HasAttachment<LemonadeSink>())
                    lemonadeSinks.Add(cellStack.GetAttachment<LemonadeSink>());
            }

            if (lemonades.Count == 0 && lemonadeSources.Count == 0 && lemonadeSinks.Count == 0)
                return;

            List<GoalInfo> goals = new GoalInfoExtaractor().Extract(config);

            var lemonadeTemp = ServiceLocator.Find<TileFactory>().CreateLemonadeTile() as Lemonade;
            bool hasLemonadeGoal = goals.Count(g => g.goalType.Includes(lemonadeTemp)) > 0;

            if (hasLemonadeGoal == false)
            {
                if (lemonadeSinks.Count > 0)
                    Debug.LogErrorFormat(config, "Error in board config {0}. Has lemonade sink but no lemonade goal", config.name);
                else
                    Debug.LogErrorFormat(config, "Error in board config {0}. Has lemonade in board but no lemonade goal", config.name);
                return;
            }
            if (lemonadeSinks.Count == 0)
            {
                Debug.LogErrorFormat(config, "Error in board config {0}. No lemonade sink", config.name);
                return;
            }

            var lemonadeGoal = goals.Find(g => g.goalType.Includes(lemonadeTemp));

            if (lemonadeGoal.goalAmount > lemonades.Count && lemonadeSources.Count == 0)
                Debug.LogErrorFormat(config, "Error in board config {0}. Needs lemonade source", config.name);

            if (lemonadeGoal.goalAmount != FindGenerationInfoFor<Lemonade>().inLevelLimit && lemonadeSources.Count > 0)
                ErrorOn(config, "Lemonade's goal count and its in level limit are not equal.");
        }

        private SpecialTileGenerationInfo FindGenerationInfoFor<T>() where T : Tile
        {
            return tileGenerationLimits.Find(generationInfo => generationInfo.tileType == typeof(T));
        }

        private void VerifyColorLimits(BoardConfig config, GameBoard gameBoard)
        {
            var forbiddenColors = new HashSet<TileColor>(new ColorChanceExtractor().ExtractGenerationChancesFrom(config).Where(c => c.Value <= 0).Select(c => c.Key));
            var inBoardColors = new HashSet<TileColor>();
            var notCoveredColors = new HashSet<TileColor>();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                        if (tile is ColoredBead coloredBead)
                            inBoardColors.Add(coloredBead.GetComponent<TileColorComponent>().color);
            }

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                {
                    bool isConvertedByTopCell = false;
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                    {
                        if (isConvertedByTopCell == false && tile is ColoredBead coloredBead)
                            notCoveredColors.Add(coloredBead.GetComponent<TileColorComponent>().color);

                        if (tile is Box || tile is Honey)
                            isConvertedByTopCell = true;
                    }
                }
            }

            foreach (var color in inBoardColors.Intersect(forbiddenColors))
                Debug.LogErrorFormat(config, "Error in board config {0}. Color {1} is in board but is also forbidden", config.name, color);

            foreach (var color in Enum.GetValues(typeof(TileColor)).Cast<TileColor>())
                if (color != TileColor.None && inBoardColors.Contains(color) == false && forbiddenColors.Contains(color) == false)
                    Debug.LogWarningFormat(config, "Warning in board config {0}. Color {1} is not in board and is not forbidden", config.name, color);

            foreach (var color in Enum.GetValues(typeof(TileColor)).Cast<TileColor>())
                if (color != TileColor.None && notCoveredColors.Contains(color) == false && forbiddenColors.Contains(color) == false)
                    Debug.LogWarningFormat(config, "Warning in board config {0}. Color {1} is hidden and is not forbidden", config.name, color);
        }

        private void VerifyTableCloth(BoardConfig config, GameBoard gameBoard)
        {
            foreach (TableClothBoardSpecificConfig specificConfig in config.tableClothBoardSpecificConfigs)
            {
                TableClothBoardSpecificConfig.TargetData firstTarget = specificConfig.firstTargetData;
                TableClothBoardSpecificConfig.TargetData secondTarget = specificConfig.secondTargetData;
                if (firstTarget.isActive == false)
                    Debug.LogErrorFormat(config, "Error in board config {0}. TableCloth FIRST TARGET should be Always ACTIVE", config.name);
                if (IsNotValidTargetForTableCloth(firstTarget) || (secondTarget.isActive && IsNotValidTargetForTableCloth(secondTarget)))
                    Debug.LogErrorFormat(config, "Error in board config {0}. TableCloth ACTIVE Targets should NOT be set as `ColorBead tileTypes with color of NONE`", config.name);
                if (firstTarget.targetAmount <= 0 || (secondTarget.isActive && secondTarget.targetAmount <= 0))
                    Debug.LogErrorFormat(config, "Error in board config {0}. TableCloth ACTIVE Targets AMOUNT should Not 0 Or less", config.name);
            }

            bool IsNotValidTargetForTableCloth(TableClothBoardSpecificConfig.TargetData targetData)
            {
                return targetData.tileType.IsNullOrEmpty() || (GetType(targetData.tileType) == typeof(ColoredBead) && targetData.tileColor == TileColor.None);
            }
        }

        private void VerifyGardens(BoardConfig config, GameBoard gameBoard)
        {
            bool hasGarden = false;

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack() && cellStack.CurrentTileStack().Top() is Garden)
                {
                    hasGarden = true;
                    break;
                }
            }

            if (hasGarden == false)
                return;

            Dictionary<TileColor, float> colorChances = new ColorChanceExtractor().ExtractGenerationChancesFrom(config);

            if (colorChances.ContainsKey(TileColor.Pink) && colorChances[key: TileColor.Pink] > 0)
                Debug.LogErrorFormat(config, "Error in board config {0}. Has Garden and pink item source.", config.name);
        }

        private void VerifyArtifacts(BoardConfig config, GameBoard gameBoard)
        {
            var artifacts = new List<ArtifactMainCell>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                foreach (var cell in cellStack.Stack())
                    if (cell is ArtifactMainCell artifactMainCell)
                        artifacts.Add(artifactMainCell);
            }

            if (artifacts.Count == 0)
                return;

            var goals = new GoalInfoExtaractor().Extract(config);

            var artifactGoal = goals.Find(g => g.goalType.Includes(artifacts[0]));

            if (artifactGoal.goalAmount != artifacts.Count)
                Debug.LogErrorFormat(config, "Error in board config {0}. Mails in board and in goal are not same.", config.name);
        }

        private void VerifyGameGoals(BoardConfig config, GameBoard gameBoard)
        {
            List<GoalInfo> goals = new GoalInfoExtaractor().Extract(config);

            var sandWithGems = new List<SandWithGem>();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                {
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                        if (tile is SandWithGem sandWithGem)
                            sandWithGems.Add(sandWithGem);
                }
            }

            if (sandWithGems.Count > 0)
            {
                foreach (var goal in goals)
                    if (goal.goalType.Includes(sandWithGems[0]))
                    {
                        if (goal.goalAmount != sandWithGems.Count)
                            ErrorOn(config, "Sand with gem goal {0} but there are {1} in board.", goal.goalAmount, sandWithGems.Count);
                        return;
                    }
                ErrorOn(config, "No goal for sand with gems");
            }
        }

        private void VerifyRivers(BoardConfig config, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                bool foundRiver = false;
                foreach (var cell in cellStack.Stack())
                {
                    if (cell is RiverCell)
                        foundRiver = true;
                    else if (foundRiver)
                        ErrorOn(config, "River in {0} is not in root.", gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y));
                }
            }
        }

        private void VerifyFireflyJars(BoardConfig config, GameBoard gameBoard)
        {
            List<GoalInfo> goals = new GoalInfoExtaractor().Extract(config);
            List<FireflyJar> fireflyJars = new List<FireflyJar>();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                {
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                        if (tile is FireflyJar jar)
                            fireflyJars.Add(jar);
                }
            }

            bool infiniteFireflyJarExist = fireflyJars.Find(jar => jar.CurrentLevel() == 99999) != null;
            int totalFireflies = 0;
            foreach (var jar in fireflyJars)
                totalFireflies += jar.CurrentLevel();

            if (fireflyJars.Count > 0)
            {
                foreach (var goal in goals)
                    if (goal.goalType.Includes(fireflyJars[0]))
                    {
                        if ((goal.goalAmount != totalFireflies && infiniteFireflyJarExist == false) || goal.goalAmount > totalFireflies)
                            ErrorOn(config, "Firefly goal is {0} but there are {1} in board.", goal.goalAmount, totalFireflies);
                        return;
                    }
            }
        }

        private void VerifyInvisibleGrasses(BoardConfig config, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                bool isUnder = false;
                foreach (var cell in cellStack.Stack())
                {
                    if (cell is RiverCell)
                    {
                        if (isUnder)
                            ErrorOn(config, "Grass in {0} is under.", gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y));
                    }
                    else
                        isUnder = true;
                }
            }
        }

        private void VerifyMultipleColors(BoardConfig config, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack() == false)
                    continue;

                bool hasColor = false;
                foreach (var tile in cellStack.CurrentTileStack().Stack())
                {
                    if (tile is ColoredBead)
                    {
                        if (hasColor)
                            ErrorOn(config, "Multiple colors are in {0}.", gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y));
                        else
                            hasColor = true;
                    }
                }
            }
        }

        private void VerifyMultipleItemTypeInOnePlace(BoardConfig config, GameBoard gameBoard)
        {
            HashSet<Type> cellTypes = new HashSet<Type>();
            HashSet<Type> tileTypes = new HashSet<Type>();

            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                cellTypes.Clear();
                tileTypes.Clear();

                foreach (var cell in cellStack.Stack())
                {
                    if (cellTypes.Contains(cell.GetType()))
                        ErrorOn(config, "Multiple cell of type {0} are in {1}", cell.GetType().Name, PositionOf(cellStack, gameBoard));
                    cellTypes.Add(cell.GetType());
                }

                if (cellStack.HasTileStack())
                {
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                    {
                        if (tileTypes.Contains(tile.GetType()))
                            ErrorOn(config, "Multiple tile of type {0} are in {1}", tile.GetType().Name, PositionOf(cellStack, gameBoard));
                        tileTypes.Add(tile.GetType());
                    }
                }
            }
        }

        private void ErrorOn(BoardConfig config, string message, params object[] args)
        {
            Debug.LogErrorFormat(config, "Error in board config {0}. {1}", config.name, string.Format(message, args));
        }

        private void WarningOn(BoardConfig config, string message, params object[] args)
        {
            Debug.LogWarningFormat(config, "Warning in board config {0}. {1}", config.name, string.Format(message, args));
        }


        private string PositionOf(CellStack cellStack, GameBoard gameBoard)
        {
            return $"{cellStack.Position()}[{LinearIndex(cellStack, gameBoard)}]";
        }

        private int LinearIndex(CellStack cellStack, GameBoard gameBoard)
        {
            return gameBoard.CellStackBoard().PositionToLinearIndex(cellStack.Position().x, cellStack.Position().y);
        }

        private void VerifyMoveCount(BoardConfig boardConfig, GameBoard gameBoard)
        {
            var levelConfig = boardConfig.levelConfig;
            if (levelConfig.maxMove > 50 || levelConfig.maxMove < 5)
                ErrorOn(boardConfig, "Max Move count is not in desired range of 50 and 5");
        }

        private void VerifyExplosionFactor(BoardConfig boardConfig, GameBoard gameBoard)
        {
            for (int i = 0; i < boardConfig.levelConfig.rainbowAddValues.Length; i++)
            {
                var amount = boardConfig.levelConfig.rainbowAddValues[i];

                if (amount > 0 && amount < 1)
                {
                    for (int j = i + 1; j < boardConfig.levelConfig.rainbowAddValues.Length; j++)
                    {
                        if (amount >= boardConfig.levelConfig.rainbowAddValues[j])
                            Debug.LogErrorFormat(boardConfig, $"Error in board config {boardConfig.name}. Element {i} ExplosionFactor Should be smaller than Element {j} ExplosionFactor");
                    }
                }
                else
                    Debug.LogErrorFormat(boardConfig, $"Error in board config {boardConfig.name}. Element {i} ExplosionFactor is not between 0 and 1");
            }
        }

        private void VerifyGoalsWhichAreNotCreated(BoardConfig boardConfig, GameBoard gameBoard)
        {
            GameBoardCreator gameBoardCreator = new GameBoardCreator();

            List<Tile> tilesWhichAreNotCreated = new List<Tile>() {new CompassTile(), new ColoredBox(0, 0), new FishStatue(0), new FireflyJar(0), new HoneyJar(), new SandWithGem(0)};
            List<Cell> cellsWhichAreNotCreated = new List<Cell>() {new HedgeCell(0), new IvyRootCell()};

            CheckTilesThatAreNotCreated(boardConfig, tilesWhichAreNotCreated, gameBoardCreator);
            CheckCellsThatAreNotCreated(boardConfig, cellsWhichAreNotCreated);
            CheckArtifactMainCellThatAreNotCreated(boardConfig);
        }

        private void CheckTilesThatAreNotCreated(BoardConfig boardConfig, List<Tile> tilesWhichAreNotCreated, GameBoardCreator gameBoardCreator)
        {
            for (int goalIndex = 0; goalIndex < boardConfig.levelConfig.goals.Count; goalIndex++)
            {
                int goalAmount = boardConfig.levelConfig.goals[goalIndex].goalAmount;
                int number = 0;

                foreach (Tile tileWhichIsNotCreated in tilesWhichAreNotCreated)
                {
                    if (GetType(boardConfig.levelConfig.goals[goalIndex].goalType) == tileWhichIsNotCreated.GetType())
                    {
                        foreach (CellConfig cellConfig in boardConfig.cells)
                        {
                            foreach (BaseItemConfig tileConfig in cellConfig.itemsArr)
                            {
                                var tile = gameBoardCreator.Debug_CreatTileForLevelsConfig(tileConfig);

                                if (tile.GetType() == tileWhichIsNotCreated.GetType())
                                    number++;
                            }
                        }

                        if (goalAmount > number)
                            ErrorOn(boardConfig, $"{tileWhichIsNotCreated} is goal but there is not in cells. number {number}");
                    }
                }
            }
        }

        private void CheckCellsThatAreNotCreated(BoardConfig boardConfig, List<Cell> cellsWhichAreNotCreated)
        {
            for (int goalIndex = 0; goalIndex < boardConfig.levelConfig.goals.Count; goalIndex++)
            {
                int goalAmount = boardConfig.levelConfig.goals[goalIndex].goalAmount;
                int number = 0;

                foreach (Cell cellWhichIsNotCreated in cellsWhichAreNotCreated)
                {
                    if (GetType(boardConfig.levelConfig.goals[goalIndex].goalType) == cellWhichIsNotCreated.GetType())
                    {
                        foreach (CellConfig cellConfig in boardConfig.cells)
                        {
                            foreach (BaseItemConfig cellBaseConfig in cellConfig.cItemsArr)
                            {
                                if (cellWhichIsNotCreated is HedgeCell)
                                {
                                    if (cellBaseConfig is HedgeConfig)
                                        number++;
                                }

                                else if (cellWhichIsNotCreated is IvyRootCell)
                                {
                                    if (cellBaseConfig is IvyRootConfig)
                                        number++;
                                }
                            }
                        }

                        if (goalAmount > number)
                            ErrorOn(boardConfig, $"{cellWhichIsNotCreated} is goal but there is not in cells. number {number}");
                    }
                }
            }
        }

        private void CheckArtifactMainCellThatAreNotCreated(BoardConfig boardConfig)
        {
            ArtifactMainCell artifactMainCell = new ArtifactMainCell(Direction.Down, 0);
            for (int goalIndex = 0; goalIndex < boardConfig.levelConfig.goals.Count; goalIndex++)
            {
                int goalAmount = boardConfig.levelConfig.goals[goalIndex].goalAmount;

                if (boardConfig.levelConfig.goals[goalIndex].goalType.Contains(artifactMainCell.ToString()))
                {
                    if (boardConfig.statues.Count < goalAmount)
                        ErrorOn(boardConfig, $"ArtifactMainCell is goal but the goal count is {goalAmount} and status count is {boardConfig.statues.Count} whilst it's not enough");
                }
            }
        }

        private void VerifyGoalsWhichAreCreated(BoardConfig boardConfig, GameBoard gameBoard)
        {
            GameBoardCreator gameBoardCreator = new GameBoardCreator();

            List<Tile> creatableItems = new List<Tile>() {new Butterfly(), new GasCylinder(0), new JamJar(), new Lemonade(), new RocketBox()};
            Buoyant buoyant = new Buoyant(TileColor.Blue);

            for (int goalIndex = 0; goalIndex < boardConfig.levelConfig.goals.Count; goalIndex++)
            {
                int goalAmount = boardConfig.levelConfig.goals[goalIndex].goalAmount;
                int number = 0;

                foreach (Tile creatableTile in creatableItems)
                {
                    if (GetType(boardConfig.levelConfig.goals[goalIndex].goalType) == creatableTile.GetType())
                    {
                        foreach (CellConfig cellConfig in boardConfig.cells)
                        {
                            foreach (BaseItemConfig tileConfig in cellConfig.itemsArr)
                            {
                                var tile = gameBoardCreator.Debug_CreatTileForLevelsConfig(tileConfig);

                                if (tile.GetType() == creatableTile.GetType())
                                    number++;
                            }
                        }
                        if (goalAmount > number)
                        {
                            for (int tileGenerationIndex = 0; tileGenerationIndex < boardConfig.levelConfig.tileGenerationLimits.Count; tileGenerationIndex++)
                            {
                                if (GetType(boardConfig.levelConfig.tileGenerationLimits[tileGenerationIndex].type) == creatableTile.GetType()
                                 && boardConfig.levelConfig.tileGenerationLimits[tileGenerationIndex].inBoardLimit > 0
                                 && boardConfig.levelConfig.tileGenerationLimits[tileGenerationIndex].inLevelLimit > 0)
                                    number = goalAmount;
                            }
                            if (goalAmount != number)
                                ErrorOn(boardConfig, $"{creatableTile} is goal but there is not in cells So check out Tile Generation Limits");
                        }
                    }
                }

                if (GetType(boardConfig.levelConfig.goals[goalIndex].goalType) == buoyant.GetType())
                {
                    foreach (CellConfig cellConfig in boardConfig.cells)
                    {
                        foreach (BaseItemConfig tileConfig in cellConfig.itemsArr)
                        {
                            var tile = gameBoardCreator.Debug_CreatTileForLevelsConfig(tileConfig);

                            if (tile.GetType() == buoyant.GetType())
                                number++;
                        }
                    }
                    if (goalAmount > number)
                    {
                        if (boardConfig.levelConfig.buoyantGenerationData.inBoardMax == 0)
                            ErrorOn(boardConfig, "Buoyant is goal but there is not in cells So check out Buoyant Generation Data");
                    }
                }
            }
        }

        private void VerifyColoredBeadGoals(BoardConfig boardConfig, GameBoard gameBoard)
        {
            ColoredBead coloredBead = new ColoredBead(ColoredBead.DirtinessState.Clean);

            for (int goalIndex = 0; goalIndex < boardConfig.levelConfig.goals.Count; goalIndex++)
            {
                int goalAmount = boardConfig.levelConfig.goals[goalIndex].goalAmount;
                TileColor goalColor = boardConfig.levelConfig.goals[goalIndex].color;
                int currentGoalCount = 0;
                bool isGarden = false;

                if (GetType(boardConfig.levelConfig.goals[goalIndex].goalType) == coloredBead.GetType())
                {
                    foreach (CellConfig cellConfig in boardConfig.cells)
                    {
                        foreach (BaseItemConfig tileConfig in cellConfig.itemsArr)
                        {
                            if (tileConfig is MatchItemConfig matchItemConfig &&
                                matchItemConfig.itemColorType.Equals(goalColor)
                             && matchItemConfig.dirtinessState.Equals(ColoredBead.DirtinessState.Clean)
                             && isGarden == false)

                                currentGoalCount++;

                            if (goalColor is TileColor.Pink && tileConfig is VaseItemConfig)
                            {
                                currentGoalCount = goalAmount;
                                isGarden = true;
                            }
                        }
                    }

                    if (goalAmount > currentGoalCount)
                    {
                        for (int colorChanceIndex = 0; colorChanceIndex < boardConfig.levelConfig.colorChances.Count; colorChanceIndex++)
                        {
                            if (goalColor.Equals(boardConfig.levelConfig.colorChances[colorChanceIndex].color))
                            {
                                if (boardConfig.levelConfig.colorChances[colorChanceIndex].wieght == 0)
                                    ErrorOn(boardConfig, $"ColoredBead {goalColor} is goal, check 'Weight' ColorChances and cells", "ColoredBead ");
                            }
                        }
                    }
                }
            }
        }

        private void VerifyRocketBoxes(BoardConfig boardConfig, GameBoard gameBoard)
        {
            List<RocketBox> rocketBoxTiles = GetTilesOfTypeInGameBoard<RocketBox>(gameBoard);
            List<RocketBoxTileSource> rocketBoxTileSources = GetTileSourcesOfTypeInGameBoard<RocketBoxTileSource>(gameBoard);

            if (rocketBoxTiles.Count == 0 && rocketBoxTileSources.Count == 0)
                return;

            CheckRocketBoxPrioritiesConfig(boardConfig);
        }

        private void CheckRocketBoxPrioritiesConfig(BoardConfig boardConfig)
        {
            var rocketBoxPriorities = boardConfig.levelConfig.rocketBoxPriorities;
            if (rocketBoxPriorities.Count == 0)
                WarningOn(boardConfig, "maybe the Rocket Box Priorities should be set.");
            else
            {
                for (int i = 0; i < rocketBoxPriorities.Count; i++)
                {
                    if (!rocketBoxPriorities[i].targetFinder)
                        ErrorOn(boardConfig, "the Rocket Box Priorities number {0} TargetFinder is not assign.", i);
                    if (!rocketBoxPriorities[i].gameplayCondition)
                        ErrorOn(boardConfig, "the Rocket Box Priorities number {0} GameplayCondition is not assign.", i);
                }
            }
        }

        private void VerifyRocketPockets(BoardConfig boardConfig, GameBoard gameBoard)
        {
            if (boardConfig.statuesWithRocket.Count != 0)
                CheckRocketBoxPrioritiesConfig(boardConfig);
        }

        private void VerifyGasCylinders(BoardConfig boardConfig, GameBoard gameBoard)
        {
            List<GasCylinder> gasCylinderTiles = GetTilesOfTypeInGameBoard<GasCylinder>(gameBoard);
            List<GasCylinderTileSource> gasCylinderTileSources = GetTileSourcesOfTypeInGameBoard<GasCylinderTileSource>(gameBoard);

            if (gasCylinderTiles.Count > 0)
            {
                if (boardConfig.TEMPORARY_InBoardGasCylinderStartCountdown <= 1)
                {
                    ErrorOn(boardConfig, "We have Gas Cylinder in board but StartCountdown is {0}. Please fix it, dear Designer.", boardConfig.TEMPORARY_InBoardGasCylinderStartCountdown);
                }
            }
            if (gasCylinderTileSources.Count > 0)
                if (boardConfig.generationGasCylinderStartCountdown <= 1)
                    ErrorOn(boardConfig, "We have Gas Cylinder source but StartCountdown is {0}. Please fix it, dear Designer.", boardConfig.generationGasCylinderStartCountdown);
        }

        private void VerifyColorBeads(BoardConfig boardConfig, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (cellStack.HasTileStack())
                    CheckColorBeadsInCellStack(boardConfig, cellStack);
        }

        private void CheckColorBeadsInCellStack(BoardConfig boardConfig, CellStack cellStack)
        {
            List<Tile> stackTiles = cellStack.CurrentTileStack().Stack().ToList();
            for (int i = 0; i < stackTiles.Count; i++)
            {
                if (stackTiles[i] is ColoredBead)
                {
                    if (DoesHaveBeneathTileAtIndex(stackTiles, i))
                        LogErrorIfBottomTileOfColorBeadIsNotValid(stackTiles[i + 1], boardConfig);
                    else
                        LogErrorIfBottomCellOfColorBeadIsNotValid(cellStack.Top(), boardConfig);
                }
            }
        }

        private bool DoesHaveBeneathTileAtIndex(List<Tile> tiles, int index)
        {
            return tiles.Count > index + 1;
        }

        private void LogErrorIfBottomTileOfColorBeadIsNotValid(Tile tileToCheck, BoardConfig boardConfig)
        {
            if (tileToCheck is IvyBush == false)
                ErrorOn(boardConfig, "a ColorBead position in {0} is not right and it's top of another tile.", tileToCheck.Parent().Position());
        }

        private void LogErrorIfBottomCellOfColorBeadIsNotValid(Cell cellToCheck, BoardConfig boardConfig)
        {
            if (!cellToCheck.CanContainTile())
                ErrorOn(boardConfig, "a ColorBead in {0} cell, is top of a {1}", cellToCheck.Parent().Position(), cellToCheck.GetType().Name);
        }

        private void VerifyCellsOverlay(BoardConfig boardConfig, GameBoard gameBoard)
        {
            List<Cell> cells = new List<Cell>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                LogErrorIfTopOfCellIsNotValid(boardConfig, cells, cellStack);
        }

        private void LogErrorIfTopOfCellIsNotValid(BoardConfig boardConfig, List<Cell> cells, CellStack cellStack)
        {
            cells = cellStack.Stack().ToList();

            LogErrorIfATileIsOverCellAndItsNotValid(boardConfig, cellStack);
            LogErrorIfACellIsOverCellAndItsNotValid(boardConfig, cells, cellStack);

            cells.Clear();
        }

        private void LogErrorIfATileIsOverCellAndItsNotValid(BoardConfig boardConfig, CellStack cellStack)
        {
            if (cellStack.HasTileStack() && !CanHaveTileOnTop(cellStack))
                ErrorOn(boardConfig, "the cell {0} at position {1} have a tile on top of it, whilst it should not.", cellStack.Top().GetType().Name, cellStack.Position());
        }

        private void LogErrorIfACellIsOverCellAndItsNotValid(BoardConfig boardConfig, List<Cell> cells, CellStack cellStack)
        {
            for (int i = 1; i < cells.Count; i++)
                if (cells[i] is RiverCell || cells[i] is EmptyCell)
                    ErrorOn(boardConfig, "the cell {0} at position {1} have a cell on top of it, whilst it should not.", cells[i].GetType().Name, cellStack.Position());
        }

        private bool CanHaveTileOnTop(CellStack cellStack)
        {
            if (cellStack.Top() is RiverCell && cellStack.CurrentTileStack().Top() is Buoyant)
                return true;

            return cellStack.Top().CanContainTile();
        }

        private void VerifyTilesOverlay(BoardConfig boardConfig, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                {
                    List<Tile> stackTiles = cellStack.CurrentTileStack().Stack().ToList();
                    LogErrorIfATileIsOverTileAndItsNotValid(boardConfig, stackTiles, cellStack);
                }
            }
        }

        private void LogErrorIfATileIsOverTileAndItsNotValid(BoardConfig boardConfig, List<Tile> stackTiles, CellStack cellStack)
        {
            for (int i = 1; i < stackTiles.Count; i++)
            {
                if (!CanHaveTileOnTop(stackTiles[i]))
                {
                    if (IsTheOverTileTableCloth(stackTiles[i - 1]))
                        continue;

                    ErrorOn(boardConfig, "there is a tile On Top of a {0} on position {1}, whilst it should not.", stackTiles[i].GetType().Name, cellStack.Position());
                }
            }
        }

        private bool CanHaveTileOnTop(Tile tile)
        {
            if (tile is Box || tile is ColoredBox || tile is Garden || tile is Padlock || tile is VacuumCleaner || tile is TableClothMainTile || IsSlaveOfType<TableClothMainTile>(tile))
                return false;

            return true;
        }

        private bool IsTheOverTileTableCloth(Tile overTile)
        {
            if (IsSlaveOfType<TableClothMainTile>(overTile) || overTile is TableClothMainTile)
                return true;
            return false;
        }

        private bool IsSlaveOfType<TCompositeTile>(Tile tile) where TCompositeTile : CompositeTile
        {
            if (tile is SlaveTile slaveTile)
                if (slaveTile.Master() is TCompositeTile)
                    return true;
            return false;
        }

        private void VerifyHardenedHoney(BoardConfig boardConfig, GameBoard gameBoard)
        {
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
                if (cellStack.HasTileStack())
                    LogIfThereIsAHardenedHoneyWithoutHoneyBeneathIt(boardConfig, cellStack);
        }

        private void LogIfThereIsAHardenedHoneyWithoutHoneyBeneathIt(BoardConfig boardConfig, CellStack cellStack)
        {
            List<Tile> tileList = cellStack.CurrentTileStack().Stack().ToList();
            for (int i = 0; i < tileList.Count; i++)
                if (tileList[i] is HardenedHoney && tileList.Count > i + 1 && tileList[i + 1] is Honey == false)
                    ErrorOn(boardConfig, "in position {0}, there is a Hardened_Honey without Honey beneath it.", cellStack.Position());
        }

        private void VerifyMandatoryGoalTiles(BoardConfig boardConfig, GameBoard gameBoard)
        {
            var goals = new GoalInfoExtaractor().Extract(boardConfig).Where(x => IsGoalNeedToBeCheck(x)).ToArray();
            List<Tile> tilesNotAtGoals = new List<Tile>();
            List<Tile> allTiles = GetTilesOfTypeInGameBoard<Tile>(gameBoard);
            List<TileSource> allTileSources = GetTileSourcesOfTypeInGameBoard<TileSource>(gameBoard);
            List<Type> tileSourceCreatorsTypes = GetTileSourceCreatorsTypesInBoardConfig(boardConfig);
            List<Type> tileTypes = GetAllTypesOfTileSourcesAndCreators(allTileSources, tileSourceCreatorsTypes);

            CheckColorChancesAndTileGenerationsLimits(boardConfig, goals, tileTypes);
            CountGoalAmountsForAllTilesInBoard(goals, allTiles, ref tilesNotAtGoals);
            LogErrorIfWeHaveNotEnoughTilesOrGoals(boardConfig, goals, tilesNotAtGoals);
        }

        private void CheckColorChancesAndTileGenerationsLimits(BoardConfig boardConfig, GoalInfo[] goals, List<Type> allTileSourcesAndCreatorTypes)
        {
            List<GoalInfo> cantReachableGoals = new List<GoalInfo>();
            for (int i = 0; i < goals.Length; i++)
            {
                if (goals[i].goalType.GoalObjectType() == typeof(ColoredBead))
                {
                    var colorChances = boardConfig.levelConfig.colorChances.Find(x => goals[i].goalType.As<ColoredGoalType>().color == x.color);
                    if (colorChances.color == TileColor.None || colorChances.wieght != 0)
                        goals[i].goalAmount = 0;

                }
                foreach (var type in allTileSourcesAndCreatorTypes)
                    if (type == goals[i].goalType.GoalObjectType())
                        foreach (var generation in boardConfig.levelConfig.tileGenerationLimits)
                        {
                            if (type == GetType(generation.type))
                            {
                                if (generation.inLevelLimit < goals[i].goalAmount)
                                {
                                    ErrorOn(boardConfig, "the Level Limit generation of tile source {0} is less than goal amount.", type.Name);
                                    cantReachableGoals.Add(goals[i]);
                                }
                                else
                                    goals[i].goalAmount -= generation.inLevelLimit;
                            }
                        }
            }
            if (cantReachableGoals.Count > 0)
                goals = goals.Where(x => cantReachableGoals.Any(y => x.goalType.GoalObjectType() == y.goalType.GoalObjectType() == false)).ToArray();
        }

        private void CountGoalAmountsForAllTilesInBoard(GoalInfo[] goals, List<Tile> allTiles, ref List<Tile> tilesNotAtGoals)
        {
            foreach (Tile tile in allTiles)
            {
                bool isTileInGoals = false;
                for (int i = 0; i < goals.Length; i++)
                {
                    if (IsGoalAsSameAsTile(goals[i], tile))
                    {

                        if (goals[i].goalAmount > 0)
                        {
                            goals[i].goalAmount -= GetTileAmount(tile);
                        }
                        isTileInGoals = true;
                        break;
                    }
                }
                if (isTileInGoals == false && IsTileMustHaveGoal(tile) && tilesNotAtGoals.Any(x => x.GetType() == tile.GetType()) == false)
                    tilesNotAtGoals.Add(tile);
            }
        }

        private void LogErrorIfWeHaveNotEnoughTilesOrGoals(BoardConfig boardConfig, GoalInfo[] goals, List<Tile> tilesNotAtGoals)
        {
            foreach (var goal in goals)
            {
                if (goal.goalType.GoalObjectType() == typeof(Buoyant))
                {
                    if (boardConfig.levelConfig.buoyantGenerationData.inBoardMax <= 0)
                        ErrorOn(boardConfig, "we have goal of Buoyant but the Buoyant generation data is 0 whilst it should not.", goal.goalType.GoalObjectType().Name);
                }

                else if (goal.goalAmount > 0)
                {
                    if (goal.goalType.GoalObjectType() == typeof(ColoredBead))
                        ErrorOn(boardConfig, "the goal of {0} have not enough tile on board.", goal.goalType.As<ColoredGoalType>().color.ToString());
                    else
                        ErrorOn(boardConfig, "the goal of {0} have not enough tile on board.", goal.goalType.GoalObjectType().Name);
                }
            }

            foreach (Tile tile in tilesNotAtGoals)
            {
                if (IsTileMustHaveGoal(tile))
                {
                    ErrorOn(boardConfig, "there is no goal for tile {0}.", tile.GetType().Name);
                }
            }
        }

        private bool IsTileMustHaveGoal(Tile tile)
        {
            return tile is BalloonMainTile ||
                   tile is CatFood ||
                   tile is ChickenNest ||
                   tile is CompassTile ||
                   tile is FireflyJar ||
                   tile is FishStatue ||
                   tile is Garden ||
                   tile is SandWithGem ||
                   tile is Buoyant ||
                   tile is Lemonade ||
                   tile is JamJar;
        }

        private bool IsGoalAsSameAsTile(GoalInfo goal, Tile tile)
        {
            if (tile is IceMakerMainTile && goal.goalType.GoalObjectType() == typeof(Ice))
                return true;
            if (tile is ChickenNest && goal.goalType.GoalObjectType() == typeof(Chick))
                return true;
            if (tile is Garden && goal.goalType.GoalObjectType() == typeof(ColoredBead))
                if (goal.goalType.As<ColoredGoalType>().color == TileColor.Pink)
                    return true;
            if (goal.goalType.GoalObjectType() == tile.GetType())
                return true;

            return false;
        }

        private int GetTileAmount(Tile tile)
        {
            if (tile is ChickenNest)
                return 3;
            if (tile is IceMakerMainTile)
                return 9;
            if (tile is Garden)
                return 999;
            return tile.CurrentLevel();
        }

        private bool IsGoalNeedToBeCheck(GoalInfo goal)
        {
            bool isGoalCell = false;
            bool isGoalExplosionTile = false;
            bool isGoalRainbow = false;

            //TODO: why it can't detect the parent interface type?
            if (goal.goalType.GoalObjectType() == typeof(ArtifactMainCell) ||
                goal.goalType.GoalObjectType() == typeof(HedgeCell) ||
                goal.goalType.GoalObjectType() == typeof(IvyRootCell))
                isGoalCell = true;

            else if (goal.goalType.GoalObjectType() == typeof(Rocket) ||
                     goal.goalType.GoalObjectType() == typeof(Bomb) ||
                     goal.goalType.GoalObjectType() == typeof(Dynamite) ||
                     goal.goalType.GoalObjectType() == typeof(TNTBarrel)
                    )
                isGoalExplosionTile = true;

            else if (goal.goalType.GoalObjectType() == typeof(Rainbow))
                isGoalRainbow = true;

            return isGoalCell == false && isGoalExplosionTile == false && isGoalRainbow == false;
        }

        private void VerifyTileGenerators(BoardConfig boardConfig, GameBoard gameBoard)
        {
            List<TileSource> tileSources = GetTileSourcesOfTypeInGameBoard<TileSource>(gameBoard);
            List<Type> tileSourceCreatorsTypes = GetTileSourceCreatorsTypesInBoardConfig(boardConfig);
            List<Type> tileTypes = GetAllTypesOfTileSourcesAndCreators(tileSources, tileSourceCreatorsTypes);

            foreach (var generator in boardConfig.levelConfig.tileGenerationLimits)
            {
                foreach (Type tileType in tileTypes)
                {
                    if (GetType(generator.type) == tileType)
                    {
                        if (generator.inLevelLimit <= 0)
                            ErrorOn(boardConfig, "the Generator limit of {0} inLevel Limit is 0.", GetType(generator.type));
                        if (generator.inBoardLimit <= 0)
                            ErrorOn(boardConfig, "the Generator limit of {0}  inBoard Limit is 0.", GetType(generator.type));

                        tileTypes.Remove(tileType);
                        break;
                    }
                }
            }

            foreach (Type tileType in tileTypes)
                ErrorOn(boardConfig, "there is no Generator Limits for tileSource {0}.", tileType.Name);
        }

        private List<Type> GetTileSourceCreatorsTypesInBoardConfig(BoardConfig boardConfig)
        {
            List<Type> sources = new List<Type>();

            foreach (var tileSourceCreator in boardConfig.tileSourceCreatorSpecificConfigs)
                foreach (var sourceTypes in tileSourceCreator.sourceTypes)
                {
                    if (sources.Contains(GetType(sourceTypes)))
                        continue;
                    sources.Add(GetType(sourceTypes));
                }

            return sources;
        }

        private List<Type> GetAllTypesOfTileSourcesAndCreators(List<TileSource> tileSources, List<Type> tileSourceCreatorsTypes)
        {
            List<Type> types = new List<Type>();
            foreach (var tileSource in tileSources)
            {
                if (types.Any(x => x == tileSource.TileType()) || tileSource.TileType() == typeof(ColoredBead))
                    continue;
                types.Add(tileSource.TileType());
            }
            foreach (var tileSourceCreator in tileSourceCreatorsTypes)
            {
                if (types.Any(x => x == tileSourceCreator) || tileSourceCreator == typeof(ColoredBead))
                    continue;
                types.Add(tileSourceCreator);
            }
            return types;
        }

        private void VerifyTileSourceVerifier(BoardConfig boardConfig, GameBoard gameBoard)
        {
            List<TileSourceCreator> tileSourceCreators = new List<TileSourceCreator>();
            List<ColoredBeadTileSource> colorBeadTileSource = new List<ColoredBeadTileSource>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasAttachment<TileSourceCreator>())
                    cellStack.GetAttachments(container: ref tileSourceCreators);
                if (cellStack.HasAttachment<ColoredBeadTileSource>())
                    cellStack.GetAttachments(container: ref colorBeadTileSource);

                if (tileSourceCreators.Count > 0 && colorBeadTileSource.Count == 0)
                    ErrorOn(boardConfig, "There is a TileSource on position {0} without a ColorBead TileSource beneath it.", cellStack.Position());

                tileSourceCreators.Clear();
                colorBeadTileSource.Clear();
            }
        }

        private List<TTile> GetTilesOfTypeInGameBoard<TTile>(GameBoard gameBoard) where TTile : Tile
        {
            List<TTile> tiles = new List<TTile>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                if (cellStack.HasTileStack())
                    foreach (var tile in cellStack.CurrentTileStack().Stack())
                    {
                        if (tile is TTile thisTile)
                            tiles.Add(thisTile);
                    }
            }
            return tiles;
        }

        private List<TTileSource> GetTileSourcesOfTypeInGameBoard<TTileSource>(GameBoard gameBoard) where TTileSource : TileSource
        {
            List<TTileSource> sources = new List<TTileSource>();
            foreach (var cellStack in gameBoard.ArrbitrayCellStackArray())
            {
                List<TTileSource> attachments = new List<TTileSource>();
                if (cellStack.HasAttachment<TTileSource>())
                    cellStack.GetAttachments(container: ref attachments);
                foreach (var attachment in attachments)
                    if (sources.Any(x => x.GetType() == attachment.GetType()) == false)
                        sources.Add(attachment);
            }
            return sources;
        }

        private void VerifyChallengeLevels(BoardConfig boardConfig, GameBoard gameBoard)
        {
            if (boardConfig.challengeLevelConfig.challengeMoveCount != 0
             && boardConfig.levelConfig.difficultyLevel != DifficultyLevel.Normal)
                ErrorOn(boardConfig, "The Board {0} is a challenge level but it's difficulty is not normal instead it's {1}", boardConfig.name, boardConfig.levelConfig.difficultyLevel);
        }

        private Type GetType(string typeName)
        {
            var type = TryFetchTypeAsItIsQualifiedName();
            if (type == null)
                type = TryFetchTypeAsItIsNotQualifiedName();
            return type;

            Type TryFetchTypeAsItIsQualifiedName()
            {
                return Type.GetType(typeName);
            }

            Type TryFetchTypeAsItIsNotQualifiedName()
            {
                return Assembly.Load("Assembly-CSharp").GetType(typeName);
            }
        }
    }
}