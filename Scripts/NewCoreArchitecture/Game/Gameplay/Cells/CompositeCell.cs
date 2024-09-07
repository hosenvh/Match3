using System.Collections.Generic;
using Match3.Game.Gameplay.Core;

namespace Match3.Game.Gameplay.Cells
{
    public abstract class CompositeCell : Cell
    {
        List<SlaveCell> slaves = new List<SlaveCell>();
        Size size;

        public CompositeCell(Size size)
        {
            this.size = size;
        }

        public void AddSalve(SlaveCell slaveCell)
        {
            slaves.Add(slaveCell);
        }

        public List<SlaveCell> Slaves()
        {
            return slaves;
        }

        public Size Size()
        {
            return size;
        }
    }
}