using UnityEngine;

namespace Match3.Presentation.TextAdapting
{
    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class UnityTextAdapter : GenericTextAdapter<UnityEngine.UI.Text>
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