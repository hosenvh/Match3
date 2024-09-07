using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;

namespace Match3.Game.Gameplay.SubSystems.HoneyMechanic
{
    [After(typeof(DestructionSystem))]
    [Before(typeof(HoneyExpansionSystem))]
    public class HoneyJarSpillingSystem : GameplaySystem
    {

        TileFactory tileFactory;

        List<CellStack> pendingCellStacksToCreateHoney = new List<CellStack>();
        HoneyExpansionSystem honeyExpansionSystem;
        HoneyGenerationData honeyGenerationData;

        public HoneyJarSpillingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            tileFactory = ServiceLocator.Find<TileFactory>();
        }

        public override void Start()
        {
            honeyGenerationData = GetSessionData<HoneyGenerationData>();
            honeyExpansionSystem = gameplayController.GetSystem<HoneyExpansionSystem>();
        }

        public override void Update(float dt)
        {
            foreach(var tile in GetFrameData<DestroyedObjectsData>().tiles)
                if(tile is HoneyJar)
                {
                    pendingCellStacksToCreateHoney.Add(tile.Parent().Parent());
                    honeyGenerationData.honeyTilesBeingGenerated++;
                }

            for(int i = pendingCellStacksToCreateHoney.Count-1; i >=0; --i)
            {
                var cellStack = pendingCellStacksToCreateHoney[i];
                if (cellStack.HasTileStack() == false || IsFree(cellStack.CurrentTileStack()))
                {
                    gameplayController.creationUtility.PlaceTileInBoard(tileFactory.CreateHoneyTile(), cellStack);
                    pendingCellStacksToCreateHoney.RemoveAt(i);
                    honeyGenerationData.honeyTilesBeingGenerated--;
                    honeyExpansionSystem.UpdateHoneyCellStacks();
                }
            }
        }
    }
}