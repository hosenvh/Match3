using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Tiles;
using System.Linq;
using System.Collections.Generic;
using System;
using Match3.Utility.GolmoradLogging;
using static Match3.Game.Gameplay.QueryUtilities;


namespace Match3.Game.Gameplay.Initialization
{

    // NOTES: For multi cellular items it is assumed that their direction is always to Right or Down. 
    // This assumption affects board iteration direction and artifact slave creation.
    public class GameBoardCreator 
    {
        CellFactory cellFactory;
        TileFactory tileFactory;
        CellAttachmentFactory cellAttachmentFactory;
        CellStackBoard cellStackBoard;

        Dictionary<int, SlaveCell> storedSlaveCells = new Dictionary<int, SlaveCell>();
        Dictionary<int, SlaveTile> storedSlaveTiles = new Dictionary<int, SlaveTile>();

        List<KeyValuePair<CellConfig, RiverCell>> riverCellsToProcess = new List<KeyValuePair<CellConfig, RiverCell>>();
        List<KeyValuePair<CellConfig, int>> portals = new List<KeyValuePair<CellConfig, int>>();


        VacuumCleanerBoardSpecificConfigFactory vacuumCleanerBoardSpecificConfigFactory;
        GasCylinderBoardSpecificConfigFactory gasCylinderBoardSpecificConfigFactory;
        TableClothBoardSpecificConfigFactory tableClothBoardSpecificConfigFactory;
        GrassSackBoardSpecificConfigFactory grassSackBoardSpecificConfigFactory;
        TileSourceCreatorSpecificConfigFactory tileSourceCreatorSpecificConfigFactory;
        BalloonBoardSpecificConfigFactory balloonBoardSpecificConfigFactory;
        DuckBoardSpecificConfigFactory duckBoardSpecificConfigFactory;

        public GameBoardCreator()
        {
            this.cellFactory = ServiceLocator.Find<CellFactory>();
            this.tileFactory = ServiceLocator.Find<TileFactory>();
            this.cellAttachmentFactory = ServiceLocator.Find<CellAttachmentFactory>();
        }

        public GameBoard CreateFrom(BoardConfig config)
        {
            int width = config.Width();
            int height = config.Height();

            InitializeWith(new CellStackBoard(width, height), config);
            var iterator = new LeftToRightTopDownGridIterator<CellStack>(cellStackBoard);

            foreach(var element in iterator)
            {
                SetupCellStack(element, cellStackBoard, config);
                SetupInitialTileStack(element, cellStackBoard, config);
            }

            // NOTE: It is always assumed that multi cellular tiles (and therefore their slaves) are on top.
            SetupSlaveTiles();
            ProcessRiverCells();
            ProcessCatPathCells(config, cellStackBoard);
            ProcessPortals(cellStackBoard);
            ProcessBalloons(cellStackBoard);
            return new GameBoard(cellStackBoard);
        }

        public void InitializeWith(CellStackBoard cellStackBoard, BoardConfig config)
        {
            this.cellStackBoard = cellStackBoard;
            vacuumCleanerBoardSpecificConfigFactory = new VacuumCleanerBoardSpecificConfigFactory(config, tileFactory, config.vacuumCleanerBoardSpecificConfigs);
            gasCylinderBoardSpecificConfigFactory = new GasCylinderBoardSpecificConfigFactory(config, tileFactory, config.gasCylinderBoardSpecificConfigs);
            tableClothBoardSpecificConfigFactory = new TableClothBoardSpecificConfigFactory(config, tileFactory, config.tableClothBoardSpecificConfigs);
            grassSackBoardSpecificConfigFactory = new GrassSackBoardSpecificConfigFactory(config, tileFactory, config.grassSackBoardSpecificConfigs);
            tileSourceCreatorSpecificConfigFactory = new TileSourceCreatorSpecificConfigFactory(config, cellAttachmentFactory, config.tileSourceCreatorSpecificConfigs);
            balloonBoardSpecificConfigFactory = new BalloonBoardSpecificConfigFactory(config, tileFactory, config.balloonBoardSpecificConfigs);
            duckBoardSpecificConfigFactory = new DuckBoardSpecificConfigFactory(config, tileFactory);
        }


        public void SetupCellStack(GridElement<CellStack> element,CellStackBoard cellStackBoard, BoardConfig config)
        {
            var x = element.x;
            var y = element.y;
            var cellStack = cellFactory.CreateCellStack(x,y);
            var linearIndex = cellStackBoard.PositionToLinearIndex(x, y);
            var cellConfig = config.cells[linearIndex];

            foreach (var cItem in cellConfig.cItemsArr)
            {
                if(cItem == null)
                {
                    DebugPro.LogError<CoreGameplayLogTag>("Null item in " + linearIndex);
                    continue;
                }
                var itemData = cItem.GetItemData();

                if (itemData is PrimaryItemData)
                {
                    switch (itemData.As<PrimaryItemData>().PrimaryType)
                    {
                        case PrimaryItemType.Block:
                            cellStack.Push(cellFactory.CreateEmptyCell(false));
                            break;
                        case PrimaryItemType.Block_Crossable:
                            cellStack.Push(cellFactory.CreateEmptyCell(true));
                            break;
                        case PrimaryItemType.Ground:
                            cellStack.Push(cellFactory.CreateGroundCell());
                            break;
                        case PrimaryItemType.River:
                            CreateRiverCell(cellStack, cellConfig, allowsFallThrough: false);
                            break;
                        case PrimaryItemType.River_Crossable:
                            CreateRiverCell(cellStack, cellConfig, allowsFallThrough: true);
                            break;
                        default:
                            DebugPro.LogError<CoreGameplayLogTag>(message: $"Cell Type {itemData.As<PrimaryItemData>().PrimaryType} is not supported yet");
                            break;
                    }
                    if (HasArtiface(linearIndex, config, ref config.statues))
                    {
                        var artifactConfig = ArtifactConfig(linearIndex, config, ref config.statues);

                        if (artifactConfig.horizontal)
                            cellStack.Push(cellFactory.CreateMainArtifactCell(Direction.Right, artifactConfig.size));
                        else
                            cellStack.Push(cellFactory.CreateMainArtifactCell(Direction.Down, artifactConfig.size));

                        StoreArtifactSlaves(artifactConfig, x, y, cellStack.Top() as AbstractArtifactMainCell);

                    }
                    if (HasArtiface(linearIndex, config, ref config.statuesWithRocket))
                    {
                        var artifactConfig = ArtifactConfig(linearIndex, config, ref config.statuesWithRocket);

                        if (artifactConfig.horizontal)
                            cellStack.Push(cellFactory.CreateMainArtifactWithRocketCell(Direction.Right, artifactConfig.size));
                        else
                            cellStack.Push(cellFactory.CreateMainArtifactWithRocketCell(Direction.Down, artifactConfig.size));

                        StoreArtifactSlaves(artifactConfig, x, y, cellStack.Top() as AbstractArtifactMainCell);

                    }
                    if (storedSlaveCells.ContainsKey(linearIndex))
                    {
                        cellStack.Push(storedSlaveCells[linearIndex]);
                    }

                }

                else if (itemData is GrassItemData)
                {
                    cellStack.Push(cellFactory.CreateGrassCell(itemData.As<GrassItemData>().Level));
                }
                else if (cItem is IvyRootConfig)
                    cellStack.Push(cellFactory.CreateIvyRootCell());
                else if (cItem is HedgeConfig hedgeConfig)
                    cellStack.Push(cellFactory.CreateHedge(hedgeConfig.level));
                else if (cItem is CatPathCellConfig catPathCellCOnfig)
                    cellStack.Push(cellFactory.CreateCatPathCell());
                else if (cItem is TileSourceCreatorConfig tileSourceCreatorConfig)
                {
                    var tileSourceCreator =(TileSourceCreator) tileSourceCreatorSpecificConfigFactory.CreateFrom(cellStack, tileSourceCreatorConfig, linearIndex);
                    cellStack.Attach(tileSourceCreator);

                    foreach (TileSource attachment in tileSourceCreator.CreateTileSources())
                        cellStack.Attach(attachment);
                }
                else if (itemData is EntranceItemData)
                {
                    // NOTE: EntranceItemData is considered an attachment.
                    cellStack.Attach(cellAttachmentFactory.CreateColoredBeadTileSource());
                    if (itemData.As<EntranceItemData>().CanMugEnter)
                        cellStack.Attach(cellAttachmentFactory.CreateLemonadeTileSource());

                    if (ShouldEnableAutoGeneratation<Nut>(config))
                        cellStack.Attach(cellAttachmentFactory.CreateNutTileSource());

                    if (ShouldEnableAutoGeneratation<JamJar>(config))
                        cellStack.Attach(cellAttachmentFactory.CreateJamJarTileSource());

                    if (ShouldEnableAutoGeneratation<CatColoredBead>(config))
                        cellStack.Attach(cellAttachmentFactory.CreateCatColoredBeadTileSource());


                }
                else if (cItem is EntranceConfig)
                    foreach (var source in cItem.As<EntranceConfig>().TileSources(cellAttachmentFactory, config))
                        cellStack.Attach(source);
                else if (itemData is MugExitItemData)
                    cellStack.Attach(cellAttachmentFactory.CreateLemonadeSink());
                else if (itemData is Forbidden_RainbowItemData)
                    cellStack.Attach(cellAttachmentFactory.CreateRainbowPreventer());
                else if (itemData is PortalEnterItemData)
                    portals.Add(new KeyValuePair<CellConfig, int>(cellConfig, linearIndex));
                else if (itemData is PortalExitItemData)
                {
                    // Ignore for now. It is considered in PortalEnterItemData;
                }

                // NOTE: The original placement definition of Wall was inverted. So here the correct placement is fed to Runtime logic.
                else if (itemData is Wall_HorizontalItemData)
                    cellStack.Attach(cellAttachmentFactory.CreateWall(GridPlacement.Left));
                else if (itemData is Wall_VerticalItemData)
                    cellStack.Attach(cellAttachmentFactory.CreateWall(GridPlacement.Down));

                else if (cItem is RopeConfig ropeConfig)
                    cellStack.Attach(cellAttachmentFactory.CreateRope(ropeConfig.placement, ropeConfig.level));
                else if (cItem is LilyPadBudConfig lilyPadBudConfig)
                    cellStack.Attach(cellAttachmentFactory.CreateLilyPadBud(lilyPadBudConfig.initalGrowthLevel));
                else
                    DebugPro.LogError<CoreGameplayLogTag>(message: $"Cell Type {itemData} is not supported yet");

            }

            cellStackBoard[x, y] = cellStack;

        }

        private bool ShouldEnableAutoGeneratation<T>(BoardConfig boardConfig) where T : Tile
        {
            bool isTileSourceCreatorOfThisTypeAvailable = boardConfig.tileSourceCreatorSpecificConfigs.Any(tileSourceCreatorSpecificConfig => tileSourceCreatorSpecificConfig.ContainsTileSourceCreatorOfType<T>());
            return !isTileSourceCreatorOfThisTypeAvailable && boardConfig.levelConfig.tileGenerationLimits.Count(c => (Type.GetType(c.type)).Equals(typeof(T))) > 0;
        }

        private void CreateRiverCell(CellStack cellStack ,CellConfig cellConfig, bool allowsFallThrough)
        {
            var cell = cellFactory.CreateRiverCell(allowsFallThrough);
            riverCellsToProcess.Add(new KeyValuePair<CellConfig, RiverCell>(cellConfig, cell as RiverCell));
            cellStack.Push(cell);
            if (cellConfig.haveBoat)
                cellStack.Attach(cellAttachmentFactory.CreateLilyPad());
        }

        private void ProcessRiverCells()
        {
            foreach(var entry in riverCellsToProcess)
            {

                if (entry.Key.riverExitIndex < 0)
                    entry.Value.SetNextRiverCell(null);
                else
                    entry.Value.SetNextRiverCell(FindRiverCellIn(entry.Key.riverExitIndex));
            }
        }

        
        private void ProcessCatPathCells(BoardConfig boardConfig, CellStackBoard cellStackBoard)
        {
            foreach(var config in boardConfig.catPathBoardSpecificConfigs)
            {
                var cellStack = cellStackBoard[cellStackBoard.LinearIndexToPosition(config.cellIndex)];
                var pathCell = QueryUtilities.FindCell<CatPathCell>(cellStack);

                var nextCellStack = cellStackBoard[cellStackBoard.LinearIndexToPosition(config.nextCellIndex)];
                var nextPathCell = QueryUtilities.FindCell<CatPathCell>(nextCellStack);

                pathCell.SetNextCell(nextPathCell);
            }
        }


        private RiverCell FindRiverCellIn(int linearIndex)
        {
            var cellStack = cellStackBoard[cellStackBoard.LinearIndexToPosition(linearIndex)];

            foreach (var cell in cellStack.Stack())
                if (cell is RiverCell)
                    return cell as RiverCell;
            return null;
        }

        private void StoreArtifactSlaves(StatueConfig artifactConfig, int x, int y, AbstractArtifactMainCell master)
        {
            var size = master.ArtifactSize();
            var direction = master.Direction();

            var maxY = 0;
            var maxX = 0;

            if(direction == Direction.Down)
            {
                maxY = size.y + y;
                maxX = size.x + x;
            }
            else if (direction == Direction.Right)
            {
                maxY = size.x + y;
                maxX = size.y + x;
            }
            for(int i = x; i< maxX; ++i)
                for(int j = y; j< maxY; ++j)
                {
                    if (i != x || j != y)
                        storedSlaveCells[cellStackBoard.PositionToLinearIndex(i, j)] = cellFactory.CreateSlaveCell(master) as SlaveCell;
                }
        }

        public void ProcessPortals(CellStackBoard cellStackBoard)
        {
            foreach (var pair in portals)
            {
                var exitIndex = pair.Key.portalExitIndex;
                var entrancePos = cellStackBoard.LinearIndexToPosition(pair.Value);
                if (exitIndex < 0)
                {

                    var portalEntrance = cellAttachmentFactory.CreatePortalEntrance(null);
                    cellStackBoard[entrancePos].Attach(portalEntrance);
                }
                else
                {
                    var exitPos = cellStackBoard.LinearIndexToPosition(pair.Key.portalExitIndex);

                    var portalEntrance = cellAttachmentFactory.CreatePortalEntrance(cellStackBoard[exitPos]);
                    var portalExit = cellAttachmentFactory.CreatePortalExit(cellStackBoard[entrancePos]);

                    cellStackBoard[entrancePos].Attach(portalEntrance);
                    cellStackBoard[exitPos].Attach(portalExit);
                }

            }
        }

        public void ProcessBalloons(CellStackBoard cellStackBoard)
        {
            for (int i = 0; i < cellStackBoard.Width(); i++)
            {
                for (int j = 0; j < cellStackBoard.Height(); j++)
                {
                    CellStack cellStack = cellStackBoard[i, j];
                    if (HasTile<BalloonMainTile>(cellStack))
                        balloonBoardSpecificConfigFactory.CreateTiedTilesFor(FindTile<BalloonMainTile>(cellStack), cellStackBoard);
                }
            }
        }

        private StatueConfig ArtifactConfig(int index, BoardConfig config, ref List<StatueConfig> source)
        {
            foreach (var c in source)
                if (c.index == index)
                    return c;

            return default(StatueConfig);
        }

        private bool HasArtiface(int index, BoardConfig config, ref List<StatueConfig> source)
        {
            foreach (var c in source)
                if (c.index == index)
                    return true;
            return false;
        }

        public void SetupInitialTileStack(GridElement<CellStack> element, CellStackBoard cellStackBoard, BoardConfig config)
        {
            var x = element.x;
            var y = element.y;
            var linearIndex = cellStackBoard.PositionToLinearIndex(x, y);
            var cellConfig = config.cells[linearIndex];

            if (cellConfig.itemsArr.Length > 0)
            {
                var tileStack = tileFactory.CreateTileStack();

                foreach (var tileConfig in cellConfig.itemsArr)
                {
                    var tile = CreateTileFrom(tileConfig, x, y, linearIndex);
                    var attachment = TryCreateTileAttachmentFor(tileConfig);

                    if(attachment != null)
                        cellStackBoard[x, y].Attach(attachment);

                    tileStack.Push(tile);

                    TryCreateSlavesFor(tile, x, y);
                }

                cellStackBoard[x,y].SetCurrnetTileStack(tileStack);
                tileStack.SetPosition(cellStackBoard[x, y].Position());
            }
        }

        private void TryCreateSlavesFor(Tile tile, int xPos, int yPos)
        {
            if (tile is CompositeTile compositeCell)
            {
                var size = compositeCell.Size();

                for(int i = 0; i < size.Witdth; ++i )
                    for(int j = 0; j<size.Height; j++)
                        if(i != 0 || j != 0)
                            storedSlaveTiles.Add(cellStackBoard.PositionToLinearIndex(xPos+ i, yPos+j), tileFactory.CreateSlaveTile(compositeCell) as SlaveTile);
            }
        }

        private CellAttachment TryCreateTileAttachmentFor(BaseItemConfig tileConfig)
        {
            switch (tileConfig)
            {
                case Forbidden_RainbowItemConfig _:
                    return cellAttachmentFactory.CreateRainbowPreventer();
                default:
                    return null;
            }
        }

        private Tile CreateTileFrom(BaseItemConfig tileConfig, int x, int y, int linearIndex)
        {
            switch (tileConfig)
            {
                case WoodItemConfig _:
                    return tileFactory.CreateBoxTile(tileConfig.GetItemData().As<WoodItemData>().Level);
                case ColoredBoxConfig _:
                    return tileFactory.CreateColoredBoxTile(tileConfig.As<ColoredBoxConfig>().level, (tileConfig.As<ColoredBoxConfig>().color));
                case ChainItemConfig _:
                    return tileFactory.CreateChainTile(tileConfig.GetItemData().As<ChainItemData>().Level);
                case MatchItemConfig config:
                    return tileFactory.CreateColoredBeadTile((TileColor)config.itemColorType, config.dirtinessState);
                case MugItemConfig _:
                    return tileFactory.CreateLemonadeTile();
                case WasteItemConfig config:
                    return tileFactory.CreateNutTile(config.level);
                case IceItemConfig _:
                    return tileFactory.CreateIceTile(tileConfig.GetItemData().As<IceItemData>().Level);
                case RainbowItemConfig _:
                    return tileFactory.CreateRainbowTile();
                case ExtraMoveItemConfig _:
                    return tileFactory.CreateExtraMoveTile(tileConfig.GetItemData().As<ExtraMoveItemData>().MoveCount);
                case FireflyJarConfig _:
                    return tileFactory.CreateFireflyJar(tileConfig.GetItemData().As<FireflyJarData>().FireflyCount);
                case ButterflyConfig _:
                    return tileFactory.CreateButterflyTile(tileConfig.As<ButterflyConfig>().color);
                case RocketBoxConfig _:
                    return tileFactory.CreateRocketBoxTile(tileConfig.As<RocketBoxConfig>().color);
                case HoneyJarConfig _:
                    return tileFactory.CreateHoneyJarTile();
                case PadlockConfig padlockConfig:
                    return tileFactory.CreatePadlock(padlockConfig.status);
                case RockItemConfig _:
                    return tileFactory.CreateRockTile(tileConfig.GetItemData().As<RockItemData>().Level);
                case SandItemConfig _:
                    return tileFactory.CreateSandTile(tileConfig.GetItemData().As<SandItemData>().Level);
                case SandWithGemItemConfig _:
                    return tileFactory.CreateSandWithGemTile(tileConfig.GetItemData().As<SandWithGemItemData>().Level);
                case VaseItemConfig _:
                    return tileFactory.CreateGardenTile();
                case ChickenNestItemConfig _:
                    return tileFactory.CreateChickenNestTile();
                case BalloonMainTileItemConfig balloonMainTileConfig:
                    return balloonBoardSpecificConfigFactory.CreateFrom(balloonMainTileConfig, linearIndex);
                case HoneyItemConfig _:
                    return tileFactory.CreateHoneyTile();
                case HardenedHoneyConfig _:
                    return tileFactory.CreateHardenedHoneyTile();
                case VacuumCleanerConfig vacuumConfig:
                    return vacuumCleanerBoardSpecificConfigFactory.CreateFrom(vacuumConfig, linearIndex);
                case GasCylinderConfig gasCylinder:
                    return gasCylinderBoardSpecificConfigFactory.CreateFrom(gasCylinder, linearIndex);
                case IvyBushConfig _:
                    return tileFactory.CreateIvyBushTile();
                case BeachBallConfig beachBallConfig:
                    return tileFactory.CreateBeachBallTile(beachBallConfig.Colors());
                case HoneycombConfiguration _:
                    return tileFactory.CreateHoneycomb();
                case TableClothConfig tableClothConfig:
                    return tableClothBoardSpecificConfigFactory.CreateFrom(tableClothConfig, linearIndex);
                case FishStatueConfig fishStatueConfig:
                    return tileFactory.CreateFishStatueTile(fishStatueConfig.level);
                case BuoyantConfig config:
                    return tileFactory.CreateBuoyantTile(config.color);
                case GrassSackConfig config:
                    return grassSackBoardSpecificConfigFactory.CreateFrom(config, linearIndex);
                case JamJarConfig _:
                    return tileFactory.CreateJamJarTile();
                case LogicalTileCreator tileCreator:
                    return tileCreator.CreateLogicalEntity(tileFactory);
                case ExplosiveItemConfig _:
                    {
                        var explosiveType = tileConfig.GetItemData().As<ExplosiveItemData>().ExplosiveType;
                        switch (explosiveType)
                        {
                            case ExplosiveItemType.Rocket:
                                return tileFactory.CreateRocketTile();
                            case ExplosiveItemType.Bomb:
                                return tileFactory.CreateBombTile();
                            case ExplosiveItemType.Dynamite:
                                return tileFactory.CreateDynamiteTile();
                            case ExplosiveItemType.TNT:
                                return tileFactory.CreateTNTBarrelTile();
                            default:
                                DebugPro.LogError<CoreGameplayLogTag>($"Explosive {explosiveType} Not Supported");
                                return null;
                        }
                    }
                case IceMakerConfig _:
                    return tileFactory.CreateIceMakerTile();
                case CompassConfig _:
                    return tileFactory.CreateCompass();
                case IvySackConfig ivySackConfig:
                    return tileFactory.CreateIvySackTile(ivySackConfig.GetItemData().As<IvySackItemData>().Level);
                case DuckConfig duckConfig:
                    return duckBoardSpecificConfigFactory.CreateDuckTile();
                case LouieConfig louieConfig:
                    return tileFactory.CreateLouieTile(louieConfig.GetItemData().As<LouieItemData>().Level);
                case CoconutConfig coconutConfig:
                    return tileFactory.CreateCoconutTile(coconutConfig.GetItemData().As<CoconutItemData>().Level);
                default:
                    DebugPro.LogError<CoreGameplayLogTag>($"Tile {tileConfig} in {cellStackBoard.PositionToLinearIndex(x, y)} Not Supported");
                    return null;
            }

        }


        public Tile Debug_CreatTileForLevelsConfig(BaseItemConfig tileConfig)
        {
            return CreateTileFrom(tileConfig, 0, 0, 0);
        }

        private void SetupSlaveTiles()
        {
            foreach(var pair in storedSlaveTiles)
            {
                var position = cellStackBoard.LinearIndexToPosition(pair.Key);
                if (cellStackBoard.IsInRange(position.x, position.y))
                {
                    var tileStack = cellStackBoard[position].CurrentTileStack();
                    if (tileStack == null)
                    {
                        tileStack = tileFactory.CreateTileStack();
                        cellStackBoard[position].SetCurrnetTileStack(tileStack);
                        tileStack.SetPosition(cellStackBoard[position].Position());
                    }
                    tileStack.Push(pair.Value);
                }
            }
        }
    }
}
