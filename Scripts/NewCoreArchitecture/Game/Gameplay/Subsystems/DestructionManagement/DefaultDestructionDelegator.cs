

using System;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.DestructionManagement
{
    public interface DefaultDestructionPresentationHandler : PresentationHandler
    {
        void Destroy(Tile tile, Action<Tile> onComplete);
        void Destroy(Cell tile, Action<Cell> onComplete);
        void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onComplete);
    }


    public class DefaultDestructionDelegator : DestructionHandler
    {

        DefaultDestructionPresentationHandler presentationHandler;

        public void Initialize(GameplayController gpc)
        {
            presentationHandler = gpc.GetPresentationHandler<DefaultDestructionPresentationHandler>();
        }

        public bool DoesAccept(Tile tile)
        {
            return true;
        }

        public bool DoesAccept(Cell cell)
        {
            return true;
        }

        public bool DoesAccept(HitableCellAttachment attachment)
        {
            return true;
        }

        public void Destroy(Tile tile, Action<Tile> onComplete)
        {
            presentationHandler.Destroy(tile, onComplete);
        }

        public void Destroy(Cell cell, Action<Cell> onCompleted)
        {
            presentationHandler.Destroy(cell, onCompleted);
        }

        public void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onCompleted)
        {
            presentationHandler.Destroy(attachment, onCompleted);
        }

        public void Clear()
        {
            
        }
    }
}
