using System;
using System.Collections.Generic;
using UnityEngine;
using PandasCanPlay.HexaWord.Utility;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/DuckTileElementsDatabaseConfig")]
    public class DuckTileElementsDatabaseConfig : ScriptableObject
    {
        [Serializable]
        public class SourceSprite
        {
            [TypeAttribute(typeof(Tile), false)]
            public string type = String.Empty;
            public TileColor color = TileColor.None;
            public Sprite sprite = null;
        }

        [SerializeField] private List<SourceSprite> sourceSprites = new List<SourceSprite>();

        private readonly Dictionary<DuckItem, Sprite> sourceSpritesDictionary = new Dictionary<DuckItem, Sprite>();
        public Dictionary<DuckItem, Sprite> SourceSpritesDictionary
        {
            get
            {
                if (sourceSpritesDictionary.Count == 0)
                {
                    foreach (SourceSprite sourceSprite in sourceSprites)
                        sourceSpritesDictionary.Add(new DuckItem(Type.GetType(sourceSprite.type), sourceSprite.color), sourceSprite.sprite);
                }
                return sourceSpritesDictionary;
            }
        }

        public Sprite GetVisual(DuckItem item)
        {
            return SourceSpritesDictionary[item];
        }
    }
}