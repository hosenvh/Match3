using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;
using System;

namespace Match3.Game.Gameplay.Initialization
{
    // TODO: Find a better name.
    public abstract class BoardSpecificConfigFactory<T,S> where T : BaseItemConfig where S : BoardSpecificItemConfig
    {
        protected BoardConfig boardConfig;
        protected TileFactory tileFactory;
        protected List<S> dynamicConfigs;

        public BoardSpecificConfigFactory(BoardConfig boardConfig, TileFactory tileFactory, List<S> dynamicConfigs)
        {
            this.boardConfig = boardConfig;
            this.tileFactory = tileFactory;
            this.dynamicConfigs = dynamicConfigs;
        }

        public Tile CreateFrom(T config, int linearIndex)
        {
            return CreateFrom(config, BoardSpecificConfigFor(linearIndex));
        }

        protected abstract Tile CreateFrom(T staticConfig, S dynamicConfig);

        protected S BoardSpecificConfigFor(int linearIndex)
        {
            var dynamicConfig = dynamicConfigs.Find(c => c.cellIndex == linearIndex);

            if (dynamicConfig == null)
                dynamicConfig = (S)Activator.CreateInstance(typeof(S), linearIndex);

            return dynamicConfig;
        }
    }
}