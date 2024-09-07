using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation
{
    [Serializable]
    public class ColorUnityEvent : UnityEvent<Color>
    {

    }

    public class ColorSetter : MonoBehaviour
    {
        public Color color;
        public ColorUnityEvent onColorSet;

        public void SetColor()
        {
            onColorSet.Invoke(color);
        }

    }
}