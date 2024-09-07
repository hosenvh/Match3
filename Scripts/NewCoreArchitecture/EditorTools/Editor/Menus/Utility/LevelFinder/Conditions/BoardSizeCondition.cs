using Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base;
using Match3.EditorTools.Editor.Utility;
using Match3.Foundation.Base.DataStructures;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions
{
    public class BoardSizeCondition : BoardFinderCondition
    {
        private readonly Bound<int> widthBound;
        private readonly Bound<int> heightBound;

        public BoardSizeCondition(Bound<int> widthBound, Bound<int> heightBound)
        {
            this.widthBound = widthBound;
            this.heightBound = heightBound;
        }

        public bool IsSatisfied(BoardConfig boardConfig)
        {
            return widthBound.Contains(boardConfig.Width())
                && heightBound.Contains(boardConfig.Height());
        }
    }

    public class BoardSizeConditionDrawer : BoardFinderConditionDrawer<BoardSizeCondition>
    {
        private Bound<int> boardWidthBound = new Bound<int>();
        private Bound<int> boardHeightBound = new Bound<int>();

        protected override void DrawConditionsInternal()
        {
            DrawInVerticallyArea(
                toDraw: () =>
                {
                    DrawInHorizontallyArea(toDraw: DrawWidthCondition);
                    DrawInHorizontallyArea(toDraw: DrawHeightCondition);
                });

            void DrawWidthCondition() => DrawIntBoundField(ref boardWidthBound, label: "Width");
            void DrawHeightCondition() => DrawIntBoundField(ref boardHeightBound, label: "Height");
        }

        private void DrawIntBoundField(ref Bound<int> bound, string label) => EditorUtilities.InsertIntBoundField(ref bound, label);

        protected override BoardSizeCondition GetSelectedCondition()
        {
            return new BoardSizeCondition(boardWidthBound, boardHeightBound);
        }
    }
}