using Match3.Presentation;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public static class SkeletonGraphicEditorConversionUtilities
    {
        struct SkeletonGraphicState
        {
            public readonly SkeletonDataAsset skeletonDataAsset;
            public readonly string initialAnimation;
            public readonly bool shouldStartLooping;

            public SkeletonGraphicState(SkeletonDataAsset skeletonDataAsset, string initialAnimation, bool shouldStartLooping)
            {
                this.skeletonDataAsset = skeletonDataAsset;
                this.initialAnimation = initialAnimation;
                this.shouldStartLooping = shouldStartLooping;
            }
        }


        public static void ConvertSkeletonGraphicToSkeletonAnimation(SkeletonGraphic skeletonGraphic, GameObject ownerGameObject)
        {
            var state = ExtractSkeletonGraphicState(skeletonGraphic);
            DestroySkeletonGraphicRelatedComponents(ownerGameObject);
            CreateSkeletonAnimationComponents(ownerGameObject, state);
        }


        // NOTE: This is mostly copied from Spine package.
        private static void CreateSkeletonAnimationComponents(GameObject ownerGameObject, SkeletonGraphicState state)
        {

            SkeletonData data = state.skeletonDataAsset.GetSkeletonData(true);

            if (data == null)
            {
                for (int i = 0; i < state.skeletonDataAsset.atlasAssets.Length; i++)
                {
                    string reloadAtlasPath = AssetDatabase.GetAssetPath(state.skeletonDataAsset.atlasAssets[i]);
                    state.skeletonDataAsset.atlasAssets[i] = (AtlasAsset)AssetDatabase.LoadAssetAtPath(reloadAtlasPath, typeof(AtlasAsset));
                }
                data = state.skeletonDataAsset.GetSkeletonData(false);
            }

            Undo.AddComponent<MeshFilter>(ownerGameObject);
            Undo.AddComponent<MeshRenderer>(ownerGameObject);
            SkeletonAnimation newSkeletonAnimation = Undo.AddComponent<SkeletonAnimation>(ownerGameObject);

            newSkeletonAnimation.skeletonDataAsset = state.skeletonDataAsset;
            SpineEditorUtilities.IngestAdvancedRenderSettings(newSkeletonAnimation);

            try
            {
                newSkeletonAnimation.Initialize(false);
            }
            catch (System.Exception e)
            {

                Debug.LogError("Editor-instantiated SkeletonAnimation threw an Exception. Destroying GameObject to prevent orphaned GameObject.");

                throw e;
            }

            // Set Defaults
            bool noSkins = data.DefaultSkin == null && (data.Skins == null || data.Skins.Count == 0); // Support attachmentless/skinless SkeletonData.
            var skin = data.DefaultSkin ?? (noSkins ? null : data.Skins.Items[0]);
            if (skin != null)
            {
                newSkeletonAnimation.initialSkinName = skin.Name;
                newSkeletonAnimation.skeleton.SetSkin(skin);
            }

            newSkeletonAnimation.zSpacing = SpineEditorUtilities.defaultZSpacing;

            newSkeletonAnimation.skeleton.Update(0);
            newSkeletonAnimation.state.Update(0);
            newSkeletonAnimation.state.Apply(newSkeletonAnimation.skeleton);
            newSkeletonAnimation.skeleton.UpdateWorldTransform();
            newSkeletonAnimation.AnimationName = state.initialAnimation;
            newSkeletonAnimation.loop = state.shouldStartLooping;

            Undo.AddComponent<SkeletonAnimationOptimizer>(ownerGameObject);
        }

        private static void DestroySkeletonGraphicRelatedComponents(GameObject ownerGameObject)
        {
            var optimizer = ownerGameObject.GetComponent<SkeletonGraphicOptimizer>();
            if (optimizer != null)
                Undo.DestroyObjectImmediate(optimizer);
            Undo.DestroyObjectImmediate(ownerGameObject.GetComponent<SkeletonGraphic>());
            Undo.DestroyObjectImmediate(ownerGameObject.GetComponent<CanvasRenderer>());
        }

        private static SkeletonGraphicState ExtractSkeletonGraphicState(SkeletonGraphic skeletonGraphic)
        {
            return new SkeletonGraphicState(
                skeletonGraphic.skeletonDataAsset,
                skeletonGraphic.startingAnimation,
                skeletonGraphic.startingLoop);
        }
    }
}