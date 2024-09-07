using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class MapObjectElementImageToSpriteConvertor : ComponentConvertor<MapItem_Element_GameObject>
    {
        protected override List<MapItem_Element_GameObject> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            var components = FindComponentsIn<MapItem_Element_GameObject>(gameObjects);

            for (int i = components.Count - 1; i >= 0; --i)
            {
                var gameObject = components[i].gameObject;
                if (HasComponent<Image>(gameObject) == false)
                    components.RemoveAt(i);
                
            }

            return components;
        }

        protected override string ConversionTitle()
        {
            return "Object Element with image to Sprite Element";
        }

        protected override void ConvertComponent(MapItem_Element_GameObject objectElement)
        {
            var ownerGameObject = objectElement.gameObject;

            var image = objectElement.GetComponent<Image>();

            SetPivot(ownerGameObject.transform.parent.As<RectTransform>(), new Vector2(0.5f, 0.5f));
            ConvertImageToSpriteRenderer(image, globalScaleUp);
        }
    }


}