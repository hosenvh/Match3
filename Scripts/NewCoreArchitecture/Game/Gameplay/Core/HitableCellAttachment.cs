namespace Match3.Game.Gameplay.Core
{
    // NOTE: This concept is not mature yet, so it doesn't fit to other systems yet. Future changes should determine how this should be refactored.
    public abstract class HitableCellAttachment : CellAttachment
    {
        public abstract void Hit();

        public abstract bool IsDestroyed();


    }
}