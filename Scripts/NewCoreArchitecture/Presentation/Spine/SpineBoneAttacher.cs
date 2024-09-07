using Spine;
using Spine.Unity;
using UnityEngine;

namespace Match3.Presentation
{
    public class SpineBoneAttacher : MonoBehaviour
    {
        public SkeletonGraphic skeletonGraphic;

        [SpineBone(dataField: "skeletonDataAsset")]
        public string targetBoneName;

        public Transform targetTransform;

        Bone targetBone;

        public void Start()
        {
            targetBone = skeletonGraphic.Skeleton.FindBone(targetBoneName);
        }

        public void Update()
        {
            // NOTE: Note sure these are the best approaches for getting bone position and rotation.
            targetTransform.transform.position = targetBone.GetWorldPosition(skeletonGraphic.transform, 1/skeletonGraphic.SkeletonDataAsset.scale);
            targetTransform.transform.rotation = Quaternion.Euler(0, 0, targetBone.WorldRotationX);
        }
    }
}