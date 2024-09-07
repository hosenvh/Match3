using RTLTMPro;
using UnityEngine;


namespace Match3.Presentation.TextAdapting
{
    [RequireComponent(typeof(RTLTextMeshPro))]
    public class RTLTextMeshProAdapter : GenericTextAdapter<RTLTextMeshPro>
    {

        private void Start()
        {
            TryFindTarget();
        }

        public override void SetText(string text)
        {
            TryFindTarget();
            target.text = text;
            target.UpdateText();
        }

        public override string Text()
        {
            TryFindTarget();
            return target.text;
        }
    }
}