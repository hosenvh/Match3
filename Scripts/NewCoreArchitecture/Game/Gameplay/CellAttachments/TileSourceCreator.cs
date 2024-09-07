using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using UnityEngine;


namespace Match3.Game.Gameplay.CellAttachments
{
    public class TileSourceCreator : CellAttachment
    {
        private readonly IEnumerable<Type> sourceTypes;

        public TileSourceCreator(IEnumerable<Type> sourceTypes)
        {
            this.sourceTypes = sourceTypes;
        }

        public IEnumerable<TileSource> CreateTileSources()
        {
            return sourceTypes.Select(type => new DynamicTileSource(type));
        }

        public IEnumerable<Type> GetSourceTypes()
        {
            return sourceTypes;
        }
    }
}