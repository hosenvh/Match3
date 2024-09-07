
using UnityEngine;

namespace PandasCanPlay.HexaWord.Utility
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string targetMember;
        public ArrayElementTitleAttribute(string targetMember)
        {
            this.targetMember = targetMember;
        }
    }
}