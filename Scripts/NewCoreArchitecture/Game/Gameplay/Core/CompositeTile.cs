
using Match3.Game.Gameplay.Tiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Game.Gameplay.Core
{
    // TODO: Change this similar to CompositeCell
    public abstract class CompositeTile : Tile
    {
        List<SlaveTile> slaves = new List<SlaveTile>();
        Size size;

        protected CompositeTile(Size size)
        {
            this.size = size;
        }

        protected CompositeTile(int initialLevel,  Size size) : base(initialLevel)
        {
            this.size = size;
        }

        public void AddSalve(SlaveTile slaveCell)
        {
            slaves.Add(slaveCell);
        }

        public List<SlaveTile> Slaves()
        {
            return slaves;
        }

        public Size Size()
        {
            return size;
        }
    }
}