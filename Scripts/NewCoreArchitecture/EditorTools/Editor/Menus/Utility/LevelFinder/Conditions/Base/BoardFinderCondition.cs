namespace Match3.EditorTools.Editor.Menus.Utility.BoardFinder.Conditions.Base
{
    public interface BoardFinderCondition
    {
        bool IsSatisfied(BoardConfig boardConfig);
    }
}