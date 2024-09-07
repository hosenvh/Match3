using Match3.Game.Gameplay.Core;
using System.Collections.Generic;

namespace Match3.Game.Gameplay.Tiles
{
    public class GrassSackMainTile : CompositeTile
    {
        [System.Serializable]
        public struct ArtifactData
        {
            public int positionIndex;
            public int size;
            public Direction direction;
        }

        private List<ArtifactData> artifactsData = new List<ArtifactData>();

        public GrassSackMainTile() : base(initialLevel: 3, size: new Size(width: 2, Height: 2))
        {

        }

        public void AddArtifactData(ArtifactData data)
        {
            artifactsData.Add(data);
        }


        public List<ArtifactData> ArtifactsData()
        {
            return artifactsData;
        }
    }
}