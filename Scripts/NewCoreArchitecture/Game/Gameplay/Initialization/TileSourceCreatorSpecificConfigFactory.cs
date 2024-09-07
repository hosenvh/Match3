using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;

namespace Match3.Game.Gameplay.Initialization
{
    public class TileSourceCreatorSpecificConfigFactory : CellAttachmentSpecificConfigFactory<TileSourceCreatorConfig, TileSourceCreatorSpecificConfig>
    {
        public TileSourceCreatorSpecificConfigFactory(BoardConfig boardConfig, CellAttachmentFactory cellAttachmentFactory, List<TileSourceCreatorSpecificConfig> dynamicConfigs) : base(boardConfig, cellAttachmentFactory, dynamicConfigs)
        {
        }

        protected override CellAttachment CreateFrom(CellStack owner, TileSourceCreatorConfig staticConfig, TileSourceCreatorSpecificConfig dynamicConfig)
        {
           return cellAttachmentFactory.CreateTileSourceCreator(dynamicConfig.sourceTypes.Select(s => Type.GetType(s)));
        }
    }
}