using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;
using static Match3.EditorTools.Editor.Menus.MapItems.Base.SkeletonGraphicEditorConversionUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class MapObjectElementSkeletonGraphicToSkeletonAnimationConvertor : ComponentConvertor<MapItem_Element_GameObject>
    {
        protected override List<MapItem_Element_GameObject> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            var components = FindComponentsIn<MapItem_Element_GameObject>(gameObjects);

            for (int i = components.Count - 1; i >= 0; --i)
            {
                var gameObject = components[i].gameObject;
                if (HasComponent<SkeletonGraphic>(gameObject) == false)
                    components.RemoveAt(i);

            }

            return components;
        }

        protected override string ConversionTitle()
        {
            return "Object Element with skeleton graphic to skeleton animation";
        }

        protected override void ConvertComponent(MapItem_Element_GameObject objectElement)
        {
            var ownerGameObject = objectElement.gameObject;
            var skeletonGraphic = ownerGameObject.GetComponent<SkeletonGraphic>();

            ConvertSkeletonGraphicToSkeletonAnimation(skeletonGraphic, ownerGameObject);
        }
    }


}