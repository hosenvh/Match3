using System;
using Match3.Foundation.Base.ComponentSystem;

namespace Match3.Game.Gameplay.Core
{
    public abstract class CellAttachment : BasicEntity
    {
        CellStack owner;

        public CellStack Owner()
        {
            return owner;
        }

        public void SetOwner(CellStack owner)
        {
            this.owner = owner;
        }

        public bool HasOwner()
        {
            return owner != null;
        }
    }
}