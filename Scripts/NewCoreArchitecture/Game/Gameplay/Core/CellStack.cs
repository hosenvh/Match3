using Match3.Foundation.Base.ComponentSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.Core
{
    public struct CellStackComponentCache
    {
        public LockState lockState;
    }

    // TODO: Define a container for attachments;
    public class CellStack : BasicEntity
    {
        public CellStackComponentCache componentCache = new CellStackComponentCache();

        Vector2Int gridPosition;
        Stack<Cell> stack = new Stack<Cell>();
        List<CellAttachment> attachments = new List<CellAttachment>();

        TileStack currentTileStack;

        protected override void OnComponentAdded(Foundation.Base.ComponentSystem.Component component)
        {
            switch (component)
            {
                case LockState c:
                    componentCache.lockState = c;
                    break;
            }

        }

        public CellStack(int xPos, int yPos)
        {
            gridPosition = new Vector2Int(xPos, yPos);
        }

        public void Push(Cell cell)
        {
            stack.Push(cell);
            cell.SetParent(this);
        }

        public Cell Top()
        {
            return stack.Peek();
        }

        public void Pop()
        {
            stack.Pop();
        }

        public Vector2Int Position()
        {
            return gridPosition;
        }

        public bool HasTileStack()
        {
            return currentTileStack != null;
        }

        public TileStack CurrentTileStack()
        {
            return currentTileStack;
        }

        public void SetCurrnetTileStack(TileStack stack)
        {
            this.currentTileStack = stack;
            stack.SetParent(this);
        }

        public void DetachTileStack()
        {
            this.currentTileStack = null;
        }

        public Stack<Cell> Stack()
        {
            return stack;
        }

        public void Attach(CellAttachment attachment)
        {
            attachment.SetOwner(this);
            attachments.Add(attachment);
        }

        public IEnumerable<CellAttachment> Attachments()
        {
            return attachments;
        }

        public bool HasAttachment<T>() where T : CellAttachment
        {
            for (int i = 0; i < attachments.Count; ++i)
                if (attachments[i] is T)
                    return true;
            return false;
        }

        public T GetAttachment<T>() where T : CellAttachment
        {
            for (int i = 0; i < attachments.Count; ++i)
                if (attachments[i] is T)
                    return (T)attachments[i];
            return default(T);
        }

        public void GetAttachments<T>(ref List<T> container) where T : CellAttachment
        {
            var count = attachments.Count;
            for (int i = 0; i < count; ++i)
                if (attachments[i] is T)
                    container.Add((T)attachments[i]);
        }

        public void RemoveAttachment(CellAttachment attachment) 
        {
            if (attachment.Owner() == this)
                attachment.SetOwner(null);

            attachments.Remove(attachment);
        }
    }
}