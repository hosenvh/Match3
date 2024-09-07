using SeganX;
using UnityEngine;

namespace Match3.Presentation.TextAdapting
{
    [RequireComponent(typeof(LocalText))]
    public class LocalTextAdapter : GenericTextAdapter<LocalText>
    {
        public override void SetText(string text)
        {
            TryFindTarget();
            target.SetText(text);
        }

        public override string Text()
        {
            TryFindTarget();
            return target.currnetText;
        }
    }
}