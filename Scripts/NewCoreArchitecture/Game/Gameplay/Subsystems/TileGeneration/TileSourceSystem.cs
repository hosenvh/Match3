using System;
using System.Collections.Generic;
using KitchenParadise.Utiltiy.Base;
using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.General;
using Match3.Game.Gameplay.Tiles;
using Match3.Game.Gameplay.Tiles.Explosives;


namespace Match3.Game.Gameplay.TileGeneration
{

    public interface TileSourceGenerationCondition
    {
        bool IsSatisfied(CellStack cellStack, GameplayController gpc);
    }


    [After(typeof(DestructionManagement.DestructionSystem))]
    public class TileSourceSystem : GameplaySystem
    {

        List<CellStack> sourceCellStacks = new List<CellStack>();
        Dictionary<CellStack, List<TileSource>> cellSpecialSourcesMap = new Dictionary<CellStack, List<TileSource>>();
        Dictionary<CellStack, TileSource> cellDefaultSourceMap = new Dictionary<CellStack, TileSource>();



        CellStackBoard cellStackBoard;

        SpecialTileTacker specialTileTacker = new SpecialTileTacker();
        TileFactory tileFactroy;


        List<TileColor> colors = new List<TileColor>();
        List<float> colorsTotalChances = new List<float>();
        Dictionary<TileColor, float> colorsDirtinessChances = new Dictionary<TileColor, float>();

        Dictionary<Type, TileSourceGenerationCondition> tileSourceGenerationConditions = new Dictionary<Type, TileSourceGenerationCondition>();

        Random random = new Random();

        PhysicsSystem physicsSystem;
        int gasCylinderStartCountDown;

        public TileSourceSystem(GameplayController gameplayController) : base(gameplayController)
        {
            this.cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            this.tileFactroy = ServiceLocator.Find<TileFactory>();

            SetupSources();
 
        }

        public override void Start()
        {
            this.physicsSystem = gameplayController.GetSystem<PhysicsSystem>();
        }

        public void SetColorGenerationChance(TileColor tileColor, float totalChance)
        {
            colors.Add(tileColor);
            colorsTotalChances.Add(totalChance);
        }

        public void SetColorDirtinessChance(TileColor color, float chance)
        {
            colorsDirtinessChances[color] = chance;
        }

        public void AddGenerationCondition<T>(TileSourceGenerationCondition condition) where T : TileSource
        {
            tileSourceGenerationConditions.Add(typeof(T), condition);
        }

        public void SetSpecialTileLimits(List<SpecialTileGenerationInfo> specialTypeLimits)
        {
            specialTileTacker.Setup(specialTypeLimits, cellStackBoard);
        }

        public void SetGasCylinderStartCountDonw(int startCountDown)
        {
            gasCylinderStartCountDown = startCountDown;
        }

        void SetupSources()
        {
            foreach (var elemnet in new LeftToRightTopDownGridIterator<CellStack>(cellStackBoard))
                ExtractTileSources(elemnet.value);
        }

        private void ExtractTileSources(CellStack value)
        {
            var tileSources = new List<TileSource>();
            value.GetAttachments<TileSource>(ref tileSources);

            if (tileSources.Count > 0)
            {
                sourceCellStacks.Add(value);
                cellSpecialSourcesMap[value] = new List<TileSource>();
            }

            foreach (var source in tileSources)
            {
                if (source is ColoredBeadTileSource)
                    cellDefaultSourceMap[value] = source;
                else
                    cellSpecialSourcesMap[value].Add(source);
                
            }
        }

        public override void Update(float dt)
        {
            specialTileTacker.UpdateRemovals(GetFrameData<DestroyedObjectsData>());

            sourceCellStacks.Shuffle(random);

            foreach (var cellStack in sourceCellStacks)
            {
                TryGenerateSpecialTileFor(cellStack);
                TryGenerateDefaultTileFor(cellStack);
            }

        }


        private void TryGenerateSpecialTileFor(CellStack cellStack)
        {
            if (CanGenerationHappenIn(cellStack) == false)
                return;

            foreach (var tileSource in cellSpecialSourcesMap[cellStack])
            {
                var tileType = tileSource.TileType();

                if (specialTileTacker.DoesTileTypeNeedsGeneration(tileType)
                    && IsGenerationConditionSatisifed(cellStack, tileSource))
                {
                    if (specialTileTacker.CanTileTypeBeGenrated(tileType))
                        GenerateATileFor(cellStack, tileSource);
                    else
                        specialTileTacker.TryStartGenerationTurnFor(tileType);

                    specialTileTacker.AdvanceTurnsFor(tileType);
                }

            }
        }


        private void TryGenerateDefaultTileFor(CellStack cellStack)
        {
            if (
                CanGenerationHappenIn(cellStack)
                && cellDefaultSourceMap.ContainsKey(cellStack) 
                && IsGenerationConditionSatisifed(cellStack, cellDefaultSourceMap[cellStack])
                )
            {
                GenerateATileFor(cellStack, cellDefaultSourceMap[cellStack]);
            }
        }

        bool IsGenerationConditionSatisifed(CellStack cellStack, TileSource tileSource)
        {
            return tileSourceGenerationConditions[tileSource.GetType()].IsSatisfied(cellStack, gameplayController);
        }

        bool CanGenerationHappenIn(CellStack cellStack)
        {
            // TODO: Later remove the HedgeCell type and introduce a property.
            bool canFallThrough = cellStack.Top() is HedgeCell && physicsSystem.CalculateCanTileStackFallThrough(cellStack);
            bool canContain = cellStack.Top().CanContainTile();
            bool isFree = cellStack.componentCache.lockState.IsFree();

            return (canContain || canFallThrough) && isFree;
        }


        private void GenerateATileFor(CellStack cellStack, TileSource tileSource)
        {
            var tileStack = tileFactroy.CreateTileStack();

            tileStack.Push(CreateTileFor(tileSource));

            if (cellStack.HasTileStack())
                cellStack.CurrentTileStack().Destroy();

            cellStack.SetCurrnetTileStack(tileStack);

            var positon = cellStack.Position();
            positon.y += tileSource.GenerationYOffset();
            tileStack.SetPosition(positon);

            specialTileTacker.TryIncrementTileAmount(tileSource.TileType());

            GetFrameData<GeneratedTileStacksData>().tileStacks.Add(tileStack);
            // Note: We are depending on sender of this event Propagation.
            ServiceLocator.Find<EventManager>().Propagate(new TileStackGeneratedEvent(tileStack), this);
        }


        // TODO: Find a modular way for this. Maybe move this to TileSource?
        private Tile CreateTileFor(TileSource tileSource)
        {
            var type = tileSource.TileType();


            if (type.Equals(typeof(ColoredBead)))
                return CreateColoredBeadTile(ChooseAColor());
            else if (type.Equals(typeof(Nut)))
                return tileFactroy.CreateNutTile(initialLevel:1);
            else if (type.Equals(typeof(Lemonade)))
                return tileFactroy.CreateLemonadeTile();
            else if (type.Equals(typeof(Butterfly)))
                return tileFactroy.CreateButterflyTile(ChooseAColor());
            else if (type.Equals(typeof(RocketBox)))
                return tileFactroy.CreateRocketBoxTile(ChooseAColor());
            else if (type.Equals(typeof(GasCylinder)))
                return tileFactroy.CreateGasCylinder(ChooseAColor(), gasCylinderStartCountDown);
            else if(type.Equals(typeof(JamJar)))
                return tileFactroy.CreateJamJarTile();
            else if (type.Equals(typeof(CatColoredBead)))
                return tileFactroy.CreateCatColoredBead(ChooseAColor());
            else if (type.Equals(typeof(Rocket)))
                return tileFactroy.CreateRocketTile();
            else if (type.Equals(typeof(Bomb)))
                return tileFactroy.CreateBombTile();
            else if (type.Equals(typeof(Dynamite)))
                return tileFactroy.CreateDynamiteTile();
            else if (type.Equals(typeof(TNTBarrel)))
                return tileFactroy.CreateTNTBarrelTile();


            throw new Exception(string.Format("Can not create a tile of type {0}", type));
        }

        private Tile CreateColoredBeadTile(TileColor color)
        {
            if (colorsDirtinessChances.ContainsKey(color) == false)
                return tileFactroy.CreateCleanColoredBeadTile(color);
            else
            {
                var dirtinessChance = colorsDirtinessChances[color];
                var dirtiness = random.NextDouble() > dirtinessChance ? ColoredBead.DirtinessState.Clean : ColoredBead.DirtinessState.Dirty;
                return tileFactroy.CreateColoredBeadTile(color, dirtiness);
            }
        }

        public TileColor ChooseAColor()
        {
            return colors[MathExtra.WeightedRandomIndex(colorsTotalChances)];
        }
    }
}