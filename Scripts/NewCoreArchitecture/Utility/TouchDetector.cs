using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Utility
{
    [DisallowMultipleComponent]
    public class TouchDetector : MonoBehaviour
    {
        private Action onUserTouch;

        public void Setup(Action onUserTouch)
        {
            this.onUserTouch = onUserTouch;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                onUserTouch.Invoke();
        }
    }
}