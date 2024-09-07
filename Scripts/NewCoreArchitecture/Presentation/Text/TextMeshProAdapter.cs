using TMPro;
using UnityEngine;

namespace Match3.Presentation.TextAdapting
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProAdapter : GenericTextAdapter<TextMeshProUGUI>
    {
        public override void SetText(string text)
        {
            TryFindTarget();
            target.text = text;
        }

        public override string Text()
        {
            TryFindTarget();
            return target.text;
        }
    }
}