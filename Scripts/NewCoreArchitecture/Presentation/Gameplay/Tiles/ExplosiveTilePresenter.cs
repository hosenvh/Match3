using Match3.Presentation.Gameplay.Core;
using Spine.Unity;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Tiles
{

    // TODO: Maybe move particle handling to another component?
    public abstract class ExplosiveTilePresenter : SpineTilePresenter
    {
        // TODO: Remove this if it's not needed.
        public ParticleSystem sparkParticlePrefab;
        public Transform sparkObjectTransform;

        [SpineBone(dataField: "skeletonDataAsset")]
        public string sparkBoneName;

        Spine.Bone sparkBone;
        Vector3 bonePosition;

        protected sealed override void OnPreSetup()
        {
            if (sparkObjectTransform)
                sparkBone = skeletonGraphic.Skeleton.FindBone(sparkBoneName);
            else
                this.enabled = false;
        }

        private void Update()
        {
            if (sparkObjectTransform == null || sparkBone == null)
                return;

            bonePosition.x = sparkBone.worldX * 95;
            bonePosition.y = sparkBone.worldY * 95;
            sparkObjectTransform.position = skeletonGraphic.transform.TransformPoint(bonePosition);
        }

        //public override void PlayHitAnimation(Action onCompleted)
        //{
        //    base.PlayHitAnimation(delegate { });

        //   
        //}
    }
}