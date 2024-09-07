
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Physics;
using Match3.Game.Gameplay.SubSystems.GrassSackMechanic;
using Match3.Game.Gameplay.SubSystems.IvyMechanic;
using Match3.Game.Gameplay.SubSystems.PowerUpManagement;

namespace Match3.Main
{
    public class MainCellFactory : CellFactory
    {

        public CellStack CreateCellStack(int xPos, int yPos)
        {
            var cellStack = new CellStack(xPos, yPos);

            cellStack.AddComponent(new LockState());


            return cellStack;
        }

        public Cell CreateEmptyCell(bool allowFallThrough)
        {
            var cell = new EmptyCell();
            cell.AddComponent(new CellPhysicalProperties(allowFallThrough));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: false));
            return cell;
        }

        public Cell CreateGroundCell()
        {
            var cell = new GroundCell();
            cell.AddComponent(new CellPhysicalProperties(true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: true));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: true));
            return cell;
        }

        public Cell CreateRiverCell(bool allowFallThrough)
        {
            var cell = new RiverCell();
            cell.AddComponent(new CellPhysicalProperties(allowFallThrough));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: false));
            return cell;
        }

        public Cell CreateGrassCell(int level)
        {
            var cell = new GrassCell(level, maxLevel: 3);
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: true));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: true));
            return cell;
        }

        public Cell CreateMainArtifactCell(Direction direction, int size)
        {
            var cell = new ArtifactMainCell(direction, size);
            cell.AddComponent(new CellPhysicalProperties( canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: true));
            return cell;
        }


        public Cell CreateMainArtifactWithRocketCell(Direction direction, int size)
        {
            var cell = new ArtifactWithRocketMainCell(direction, size);
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: true));
            return cell;
        }

        public Cell CreateSlaveCell(CompositeCell  master)
        {
            var cell = new SlaveCell(master);

            // TODO: The properties must come form the master cell probably.
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(master.GetComponent< GrassSackMechanicCellProperties>().canCreateItemOnCell));

            return cell;
        }

        public Cell CreateIvyRootCell()
        {
            var cell = new IvyRootCell();
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: true));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: false));
            return cell;
        }

        public Cell CreateHedge(int level)
        {
            var cell = new HedgeCell(level);
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: true));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: false));
            return cell;
        }

        public Cell CreateCatPathCell()
        {
            var cell = new CatPathCell();
            cell.AddComponent(new CellPhysicalProperties(canTileFallThrough: true));
            cell.AddComponent(new CellPowerUpProperties(isHammerTarget: false));
            cell.AddComponent(new IvyMechanicCellProperties(canBeTakenOverByIvy: false));
            cell.AddComponent(new GrassSackMechanicCellProperties(canCreateItemOnCell: true));
            return cell;
        }
    }
}