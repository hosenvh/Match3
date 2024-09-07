using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.SubSystemsData.FrameData.General;
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.SubSystems.BuoyantMechanic
{
    public abstract class BuoyantSystem : GameplaySystem
    {

        protected DestroyedObjectsData destrotedObjectsData;
        protected GeneratedObjectsData generatedObjectsData;
        protected List<Buoyant> buoyants = new List<Buoyant>();
        protected CellStackBoard cellStackBoard;


        List<RiverCell> potentialRiverCells = new List<RiverCell>(32);
        List<RiverCell> availableRiverCells = new List<RiverCell>(32);

        UserMovementData userMovementData;

        protected BuoyantSystem(GameplayController gameplayController) : base(gameplayController)
        {
            userMovementData = GetFrameData<UserMovementData>();
            destrotedObjectsData = GetFrameData<DestroyedObjectsData>();
            generatedObjectsData = GetFrameData<GeneratedObjectsData>();

            var cellStacks = gameplayController.GameBoard().ArrbitrayCellStackArray();
            buoyants = QueryUtilities.ExtractTilesOnTop<Buoyant>(cellStacks);

            // NOTE: It is assumed river cells won't move when there are buoyants
            potentialRiverCells = QueryUtilities.ExtractCellsOnTop<RiverCell>(cellStacks, IsPotientialRiverCell);

            cellStackBoard = gameplayController.GameBoard().CellStackBoard();
        }

        private bool IsPotientialRiverCell(RiverCell riverCell)
        {
            return riverCell.Parent().HasAttachment<LilyPad>() == false && riverCell.Parent().HasAttachment<LilyPadBud>() == false;
        }

        protected bool IsUserMoved()
        {
            return userMovementData.moves > 0;
        }

        protected List<RiverCell> ExtractAvailableRiverCells(Predicate<RiverCell> isAvailable)
        {
            availableRiverCells.Clear();

            foreach (var riverCell in potentialRiverCells)
                if (isAvailable(riverCell))
                    availableRiverCells.Add(riverCell);

            return availableRiverCells;
        }
    }
}