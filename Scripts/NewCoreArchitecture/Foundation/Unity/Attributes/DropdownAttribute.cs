
using UnityEngine;
using System.Collections.Generic;

namespace PandasCanPlay.HexaWord.Utility
{
    // TODO: Extend it for other types.
    public class DropdownAttribute : PropertyAttribute
    {
        public List<string> options;

        public DropdownAttribute(params string[] values)
        {
            this.options = new List<string>(values);
        }
    }
}