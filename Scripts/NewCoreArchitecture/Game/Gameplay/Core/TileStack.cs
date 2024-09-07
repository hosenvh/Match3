using Match3.Foundation.Base.ComponentSystem;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Physics;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.Core
{
    public struct TileStackComponentCache
    {
        public LockState lockState;
        public TileStackPhysicalState tileStackPhysicalState;
    }

    public class TileStack : BasicEntity
    {
        public event Action onDestroyed = delegate { };
        public TileStackComponentCache componentCache = new TileStackComponentCache();
        Vector2 position;

        Stack<Tile> stack = new Stack<Tile>();
        CellStack parent;


        protected override void OnComponentAdded(Foundation.Base.ComponentSystem.Component component)
        {
            switch (component)
            {
                case LockState c:
                    componentCache.lockState = c;
                    break;
                case TileStackPhysicalState c:
                    componentCache.tileStackPhysicalState = c;
                    break;
            }
                    
        }

        public void SetParent(CellStack parent)
        {
            this.parent = parent;
        }

        public void Push(Tile tile)
        {
            stack.Push(tile);
            tile.SetParent(this);
        }

        public void Pop()
        {
            stack.Pop();
        }

        public void Destroy()
        {
            parent.DetachTileStack();
            onDestroyed();

            parent = null;
        }

        public Tile Top()
        {
            return stack.Peek();
        }

        public bool IsDepleted()
        {
            return stack.Count == 0;
        }

        public Stack<Tile> Stack()
        {
            return stack;
        }

        public CellStack Parent()
        {
            return parent;
        }

        public void SetPosition(Vector2 pos)
        {
            this.position = pos;
        }

        public Vector2 Position()
        {
            return position;
        }

        // NOTE: I'm not sure it's a good check.
        // TODO: Try to completly remove this.
        public bool IsDestroyed()
        {
            return parent == null;
        }
    }
}