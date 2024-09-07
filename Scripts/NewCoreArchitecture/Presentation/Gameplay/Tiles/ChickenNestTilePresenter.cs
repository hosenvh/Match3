using System;
using KitchenParadise.Presentation;
using Match3.Presentation.Gameplay.Core;


namespace Match3.Presentation.Gameplay.Tiles
{
    public class ChickenNestTilePresenter : SpineTilePresenter
    {
        protected override void OnPreSetup()
        {
        }

        protected override void PlayHitAnimation(Action onCompleted)
        {
            if (tile.IsDestroyed())
            {
                onCompleted.Invoke();
                return;
            }
            if (tile.CurrentLevel() > 0)
                base.PlayHitAnimation(onCompleted);
        }

        public void PlayDestroyingAnimation()
        {
            skeletonGraphic.AnimationState.SetAnimation(trackIndex: 0, popAnimations[0], onCompleted: delegate {  });
        }
    }
}