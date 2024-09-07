using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.DataStructures;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class MoveCountCondition : BoardFinderCondition
    {
        private readonly Bound<int> movesCountBound;

        public MoveCountCondition(Bound<int> movesCountBound)
        {
            this.movesCountBound = movesCountBound;
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            return movesCountBound.Contains(boardConfig.levelConfig.maxMove);
        }
    }

    public class MoveCountConditionDrawer : BoardFinderConditionDrawer<MoveCountCondition>
    {
        private Bound<int> movesCountBound = new Bound<int>();

        protected override void DrawConditionsInternal()
        {
            DrawInVerticallyArea(
                toDraw: () =>
                {
                    DrawInHorizontallyArea(
                        toDraw: DrawMovesCountCondition);
                });

            void DrawMovesCountCondition() => DrawIntBoundField(ref movesCountBound, label: "Moves Count");
        }

        private void DrawIntBoundField(ref Bound<int> bound, string label) => EditorUtilities.InsertIntBoundField(ref bound, label);

        protected override MoveCountCondition GetSelectedCondition()
        {
            return new MoveCountCondition(movesCountBound);
        }
    }
}