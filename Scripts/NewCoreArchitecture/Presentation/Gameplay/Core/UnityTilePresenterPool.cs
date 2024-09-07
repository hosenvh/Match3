using DG.Tweening;
using Match3.Foundation.Unity.ObjectPooling;
using Match3.Game.Effects;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Core
{
    public class UnityTilePresenterPool : UnityComponentObjectPool<TilePresenter>
    {
        protected override void ActiveObject(TilePresenter obj)
        {
        }

        protected override void DeactiveObject(TilePresenter obj)
        {
            var transform = obj.gameObject.transform;

            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            DOTween.Kill(transform);
            if(obj.glowEffectController)
                obj.glowEffectController.SetNormal();
            transform.SetParent(targetTransform, false);
        }
    }
}
