namespace Match3.Presentation.Gameplay.BorderPresentation
{
    public class BorderInfo
    {
        public bool SEFilled;
        public bool NEFilled;
        public bool SWFilled;
        public bool NWFilled;

        public BorderInfo()
        {
            this.SEFilled = false;
            this.NEFilled = false;
            this.SWFilled = false;
            this.NWFilled = false;
        }
        public BorderInfo(BorderInfo other)
        {
            this.SEFilled = other.SEFilled;
            this.NEFilled = other.NEFilled;
            this.SWFilled = other.SWFilled;
            this.NWFilled = other.NWFilled;
        }

        public void FillSE()
        {
            SEFilled = true;
        }

        public void FillNE()
        {
            NEFilled = true;
        }

        public void FillSW()
        {
            SWFilled = true;
        }

        public void FillNW()
        {
            NWFilled = true;
        }

        public bool IsAllEmpty()
        {
            return !SEFilled && !NEFilled && !SWFilled && !NWFilled;
        }

        public void Reset()
        {
            SEFilled = false;
            NEFilled = false;
            SWFilled = false;
            NWFilled = false;
        }

        public int FillCount()
        {
            return (SEFilled ? 1 : 0) + (NEFilled ? 1 : 0) + (SWFilled ? 1 : 0) + (NWFilled ? 1 : 0);
        }

    }
}