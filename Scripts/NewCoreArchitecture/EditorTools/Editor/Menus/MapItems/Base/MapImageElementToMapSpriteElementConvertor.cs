using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class MapImageElementToMapSpriteElementConvertor : ComponentConvertor<MapItem_Element_Image>
    {
        protected override List<MapItem_Element_Image> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            var components = FindComponentsIn<MapItem_Element_Image>(gameObjects);

            for (int i = components.Count - 1; i >= 0; --i)
            {
                var gameObject = components[i].gameObject;
                if (HasComponent<Image>(gameObject) == false)
                {
                    Debug.LogError($"Gameobjet {gameObject} has no Image component", gameObject);
                    components.RemoveAt(i);
                }
            }

            return components;
        }

        protected override string ConversionTitle()
        {
            return "Image Element to Sprite Element";
        }

        protected override void ConvertComponent(MapItem_Element_Image imageElement)
        {
            var ownerGameObject = imageElement.gameObject;

            var image = imageElement.GetComponent<Image>();

            ChangeScriptTo<MapItem_Element_SpriteRenderer>(ownerGameObject, imageElement);
            SetPivot(ownerGameObject.transform.parent.As<RectTransform>(), new Vector2(0.5f, 0.5f));
            ConvertImageToSpriteRenderer(image, globalScaleUp);
        }
    }


}