using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Match3.Game.Effects
{
    public class MapItemBounceController : MonoBehaviour
    {
    
        [SerializeField] [Range(0, 1)] private float duration;
        [SerializeField] [Range(0, 0.5f)] private float scaleAnimationFactor;
        [SerializeField] [Range(0, 0.5f)] private float localMoveAnimationFactor;


        private HashSet<Transform> inBouncingTransforms = new HashSet<Transform>();

        public void DoBounce(Transform mapItemTransform)
        {
            if (inBouncingTransforms.Contains(mapItemTransform)) return;
            inBouncingTransforms.Add(mapItemTransform);
        
            var seq = DOTween.Sequence();
            Vector3 initLocalPosition = mapItemTransform.localPosition;
            Vector3 initLocalScale = mapItemTransform.localScale;

            seq.Insert(0, mapItemTransform.DOLocalMoveY(initLocalPosition.y + localMoveAnimationFactor, duration));
            seq.Insert(0,
                mapItemTransform.DOScale(
                    new Vector3(initLocalScale.x - scaleAnimationFactor, initLocalScale.y,
                        initLocalScale.z), duration / 2));
            seq.Insert(duration / 2,
                mapItemTransform.DOScale(
                    new Vector3(initLocalScale.x - 0.1f - scaleAnimationFactor,
                        initLocalScale.y + scaleAnimationFactor, initLocalScale.z),
                    duration));
            seq.Insert(duration, mapItemTransform.DOLocalMoveY(initLocalPosition.y, duration / 2));
            seq.Insert(duration, mapItemTransform.DOScale(initLocalScale, duration / 2));

            seq.InsertCallback(duration + 0.3f, () => { inBouncingTransforms.Remove(mapItemTransform); });
        }
    
    
        public void DoBounce(IEnumerable<Transform> mapItemTransforms)
        {
            foreach (var mapItemTransform in mapItemTransforms)
            {
                DoBounce(mapItemTransform);
            }
        }
    
    
    }
}

