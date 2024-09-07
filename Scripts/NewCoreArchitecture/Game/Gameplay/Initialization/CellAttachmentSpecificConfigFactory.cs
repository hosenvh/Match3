using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using System.Collections.Generic;
using System;

namespace Match3.Game.Gameplay.Initialization
{
    public abstract class CellAttachmentSpecificConfigFactory<T,S> where T : BaseItemConfig where S : BoardSpecificItemConfig
    {
        protected BoardConfig boardConfig;
        protected CellAttachmentFactory cellAttachmentFactory;
        protected List<S> dynamicConfigs;

        public CellAttachmentSpecificConfigFactory(BoardConfig boardConfig, CellAttachmentFactory cellAttachmentFactory, List<S> dynamicConfigs)
        {
            this.boardConfig = boardConfig;
            this.cellAttachmentFactory = cellAttachmentFactory;
            this.dynamicConfigs = dynamicConfigs;
        }

        public CellAttachment CreateFrom(CellStack owner, T config, int linearIndex)
        {
            return CreateFrom(owner, config, BoardSpecificConfigFor(linearIndex));
        }

        protected abstract CellAttachment CreateFrom(CellStack owner ,T staticConfig, S dynamicConfig);

        protected S BoardSpecificConfigFor(int linearIndex)
        {
            var dynamicConfig = dynamicConfigs.Find(c => c.cellIndex == linearIndex);

            if (dynamicConfig == null)
                dynamicConfig = (S)Activator.CreateInstance(typeof(S), linearIndex);

            return dynamicConfig;
        }
    }
}
