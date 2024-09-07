using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class PureImageToSpriteRendererConvertor : ComponentConvertor<Image>
    {
        protected override List<Image> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            var components = FindComponentsIn<Image>(gameObjects);

            for (int i = components.Count-1; i >= 0; --i)
            {
                var gameObject = components[i].gameObject;
                if (HasComponent<Button>(gameObject) || HasComponent<MapItem_Element>(gameObject))
                {
                    Debug.LogError($"Gameobjet {gameObject} is not a pure Image", gameObject);
                    components.RemoveAt(i);
                }
            }

            return components;
        }

        protected override void ConvertComponent(Image image)
        {
            ConvertImageToSpriteRenderer(image, globalScaleUp);
        }

        protected override string ConversionTitle()
        {
            return "Image to Sprite Renderer";
        }
    }


}