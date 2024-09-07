using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class WrongImageAspectRatioLogger : EditorComponentModifier<Image>
    {
        protected override List<Image> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            return FindComponentsIn<Image>(gameObjects);
        }

        protected override string OperationTitle()
        {
            return "Checking Aspect Ratio";
        }

        protected override void ProcessModification(Image component)
        {
            var spriteRec = component.sprite.rect;
            var imageRect = component.rectTransform.rect;

            var spriteRatio = spriteRec.width / spriteRec.height;
            var imageRatio = imageRect.width / imageRect.height;

            if (Mathf.Approximately(spriteRatio, imageRatio) == false)
                Debug.LogWarning($"Aspect Ratio is wrong in {component.name}", component);
        }
    }
}