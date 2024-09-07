using System;
using Match3.EditorTools.Editor.Utility;


namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base
{
    public interface BoardFinderConditionDrawer
    {
        void DrawCondition();
        BoardFinderCondition GetSelectedCondition();
    }

    public abstract class BoardFinderConditionDrawer<TCondition> : BoardFinderConditionDrawer where TCondition : BoardFinderCondition
    {
        private class NoCondition : BoardFinderCondition
        {
            public bool IsSatisfied(BoardConfig boardConfig)
            {
                return true;
            }
        }

        private bool isEnabled;

        public void DrawCondition()
        {
            DrawInHorizontallyArea(
                toDraw: () =>
                {
                    DrawIsEnableToggle();
                    if (ShouldDrawConditions())
                        DrawConditionsInternal();
                });

            void DrawIsEnableToggle() => EditorUtilities.InsertToggle(ref isEnabled, title: typeof(TCondition).Name.SplitCamelCase());
            bool ShouldDrawConditions() => isEnabled;
        }

        protected abstract void DrawConditionsInternal();

        protected void DrawInHorizontallyArea(Action toDraw) => EditorUtilities.DrawInHorizontallyArea(toDraw);
        protected void DrawInVerticallyArea(Action toDraw) => EditorUtilities.DrawInVerticallyArea(toDraw);

        BoardFinderCondition BoardFinderConditionDrawer.GetSelectedCondition()
        {
            if (isEnabled == false)
                return new NoCondition();
            return GetSelectedCondition();
        }

        protected abstract TCondition GetSelectedCondition();
    }
}