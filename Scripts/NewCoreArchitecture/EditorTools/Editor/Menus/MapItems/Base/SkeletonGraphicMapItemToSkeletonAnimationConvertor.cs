using Match3.Presentation;
using Spine;
using Spine.Unity;
using Spine.Unity.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;
using static Match3.EditorTools.Editor.Menus.MapItems.Base.SkeletonGraphicEditorConversionUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class SkeletonGraphicMapItemToSkeletonAnimationConvertor : ComponentConvertor<MapItem_Element_Spine>
    {


        protected override List<MapItem_Element_Spine> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            var components = FindComponentsIn<MapItem_Element_Spine>(gameObjects);

            for (int i = components.Count - 1; i >= 0; --i)
            {
                var gameObject = components[i].gameObject;

                var skeletonGraphic = gameObject.GetComponent<SkeletonGraphic>();

                if (skeletonGraphic == null)
                {
                    Debug.LogError($"Item {gameObject.name} doesn't have skeleton graphic", gameObject);
                    components.RemoveAt(i);
                }
                else if (skeletonGraphic.SkeletonData == null)
                {
                    Debug.LogError($"Item {gameObject.name} doesn't have skeleton graphic data", gameObject);
                    components.RemoveAt(i);
                }
            }

            return components;
        }

        protected override string ConversionTitle()
        {
            return "Convert To Spine";
        }

        protected override void ConvertComponent(MapItem_Element_Spine component)
        {
            var ownerGameObject = component.gameObject;
            var skeletonGraphic = ownerGameObject.GetComponent<SkeletonGraphic>();


            Scale(ownerGameObject.transform, globalScaleUp);

            ChangeScriptTo<MapItem_Element_SpineAnimation>(ownerGameObject, component);
            ConvertSkeletonGraphicToSkeletonAnimation(skeletonGraphic, ownerGameObject);

            //Undo.DestroyObjectImmediate(ownerGameObject.transform.As<RectTransform>());

        }


    }
}