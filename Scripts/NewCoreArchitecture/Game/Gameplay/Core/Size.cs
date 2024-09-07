using UnityEngine;

namespace Match3.Game.Gameplay.Core
{
    public struct Size 
    {
        Vector2Int internalSize;

        public Size(int width, int Height)
        {
            internalSize = new Vector2Int(width, Height);
        }

        public int Witdth { get => internalSize.x; set => internalSize.x = value; }
        public int Height { get => internalSize.y; set => internalSize.y= value; }
    }
}