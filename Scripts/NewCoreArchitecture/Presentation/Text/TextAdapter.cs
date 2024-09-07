using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.TextAdapting
{

    public abstract class TextAdapter : MonoBehaviour
    {
        public abstract void SetText(string text);

        public abstract string Text();
    }
}