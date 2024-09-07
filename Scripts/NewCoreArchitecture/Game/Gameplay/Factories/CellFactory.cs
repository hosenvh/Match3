using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Factories
{
    public interface  CellFactory : Service
    {
        CellStack CreateCellStack(int xPos, int yPos);
        Cell CreateEmptyCell(bool allowFallThrough);

        Cell CreateRiverCell(bool allowFallThrough);
        Cell CreateGroundCell();

        Cell CreateMainArtifactCell(Direction direction, int size);
        Cell CreateMainArtifactWithRocketCell(Direction direction, int size);

        Cell CreateSlaveCell(CompositeCell master);

        Cell CreateGrassCell(int level);

        Cell CreateIvyRootCell();
        Cell CreateHedge(int level);

        Cell CreateCatPathCell();
    }
}