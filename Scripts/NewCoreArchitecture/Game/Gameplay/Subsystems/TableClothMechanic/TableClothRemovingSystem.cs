using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using System.Collections.Generic;
using static Match3.Game.Gameplay.QueryUtilities;
using static Match3.Game.Gameplay.ActionUtilites;
using System;

namespace Match3.Game.Gameplay.SubSystems.TableClothMechanic
{

    public interface TableClothRemovingSystemPort : PresentationHandler
    {
        void Remove(TableCloth tableCloth, Action onCompleted);
    }

    public struct TableClothRemovingSystemKeyType : KeyType
    {

    }

    public class TableCloth
    {
        public TableClothMainTile mainTile;
        public List<SlaveTile> slaves = new List<SlaveTile>();
    }

    [Before(typeof(HitManagement.HitGenerationSystem))]
    public class TableClothRemovingSystem : GameplaySystem
    {
        List<TableCloth> notRemovedTableCloths = new List<TableCloth>();
        List<TableCloth> pendingTableCloths = new List<TableCloth>();


        TableClothRemovingSystemPort presentationHandler;

        public TableClothRemovingSystem(GameplayController gameplayController) : base(gameplayController)
        {
            presentationHandler = gameplayController.GetPresentationHandler<TableClothRemovingSystemPort>();

            foreach (var cellStack in gameplayController.GameBoard().ArrbitrayCellStackArray())
                if (HasTileOnTop<TableClothMainTile>(cellStack))
                    notRemovedTableCloths.Add(CreateTableClothFor(TopTile(cellStack) as TableClothMainTile));
        }

        private TableCloth CreateTableClothFor(TableClothMainTile tableClothMainTile)
        {
            var tableCloth = new TableCloth();
            tableCloth.mainTile = tableClothMainTile;

            var cellStackBoard = gameplayController.GameBoard().CellStackBoard();
            var position = tableClothMainTile.Parent().Parent().Position();

            for (int i = 0; i < tableClothMainTile.Size().Witdth; ++i)
                for (int j = 0; j < tableClothMainTile.Size().Height; ++j)
                    if (i != 0 || j != 0)
                        tableCloth.slaves.Add(TopTile(cellStackBoard[position.x + i, position.y + j]) as SlaveTile);

            return tableCloth;
        }

        public override void Update(float dt)
        {
            CheckForFilledTableCloths();

            ProcessPendingTableCleanerRemovals();
        }

        void CheckForFilledTableCloths()
        {
            for (int i = notRemovedTableCloths.Count - 1; i >= 0; --i)
            {
                var tableCLoth = notRemovedTableCloths[i];
                if (tableCLoth.mainTile.IsFilled())
                {
                    AddToPending(tableCLoth);
                    notRemovedTableCloths.RemoveAt(i);
                }
            }
        }

        private void AddToPending(TableCloth tableCloth)
        {
            pendingTableCloths.Add(tableCloth);
        }

        void ProcessPendingTableCleanerRemovals()
        {
            for (int i = pendingTableCloths.Count - 1; i >= 0; --i)
            {
                var tableCloth = pendingTableCloths[i];
                if (IsReadyToBeRemoved(tableCloth))
                {
                    pendingTableCloths.RemoveAt(i);
                    StartRemoval(tableCloth);
                }
            }

        }

        private void StartRemoval(TableCloth tableCloth)
        {
            FullyLock<TableClothRemovingSystemKeyType>(tableCloth.mainTile.Parent().Parent());

            foreach(var slave in tableCloth.slaves)
                FullyLock<TableClothRemovingSystemKeyType>(slave.Parent().Parent());

            presentationHandler.Remove(tableCloth, onCompleted: () => Remove(tableCloth));
        }

        private void Remove(TableCloth tableCloth)
        {
            FullyUnlock(tableCloth.mainTile.Parent().Parent());

            foreach (var slave in tableCloth.slaves)
                FullyUnlock(slave.Parent().Parent());

            Destroy(tableCloth);
        }

        private void Destroy(TableCloth tableCloth)
        {
            var destructionData = GetFrameData<DestructionData>();

            FullyDestroy(tableCloth.mainTile);
            destructionData.tilesToDestroy.Add(tableCloth.mainTile);

            foreach (var slave in tableCloth.slaves)
            {
                FullyDestroy(slave);
                destructionData.tilesToDestroy.Add(slave);
            }
        }

        private bool IsReadyToBeRemoved(TableCloth tableCloth)
        {
            if (IsFullyFree(tableCloth.mainTile.Parent().Parent()) == false)
                return false;

            foreach (var slave in tableCloth.slaves)
                if (IsFullyFree(slave.Parent().Parent()) == false)
                    return false;

            return true;
        }
    }
}