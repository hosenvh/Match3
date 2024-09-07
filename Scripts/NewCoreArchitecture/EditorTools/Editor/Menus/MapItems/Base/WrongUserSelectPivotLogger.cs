using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;
using Match3.EditorTools.Editor.Base;
using static Match3.EditorTools.Editor.Base.BasicComponentEditorOperationUtilities;

namespace Match3.EditorTools.Editor.Menus.MapItems.Base
{
    public class WrongUserSelectPivotLogger : EditorComponentModifier<MapItem_UserSelect>
    {
        protected override List<MapItem_UserSelect> ExtractComponentsFrom(GameObject[] gameObjects)
        {
            return FindComponentsIn<MapItem_UserSelect>(gameObjects, (item) => HasComponentInChilderen<SkeletonGraphic>(item.gameObject));
        }

        protected override string OperationTitle()
        {
            return "Checking Pivot";
        }

        protected override void ProcessModification(MapItem_UserSelect component)
        {
            var rectTransform = component.transform as RectTransform;
            var pivot = rectTransform.pivot;
            var rect = rectTransform.rect;

            if ((pivot.x != 0.5f || pivot.y != 0.5f) && (rect.width != 0 || rect.height != 0))
                Debug.LogWarning($"Pivot of {component.name} is not (0.5,0.5)", component);
        }
    }
}