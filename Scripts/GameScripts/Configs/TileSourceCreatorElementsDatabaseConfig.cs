using System;
using System.Collections.Generic;
using UnityEngine;
using PandasCanPlay.HexaWord.Utility;
using Match3.Game.Gameplay.Core;

namespace Match3
{
    [CreateAssetMenu(menuName = "Baord/CellItems/TileSourceCreatorElementsDatabaseConfig")]
    public class TileSourceCreatorElementsDatabaseConfig : ScriptableObject
    {
        [Serializable]
        public class SourceSprite
        {
            [TypeAttribute(typeof(Tile), false)]
            public string type = String.Empty;
            public Sprite sprite = null;
        }

        [SerializeField] private List<SourceSprite> sourceSprites = new List<SourceSprite>();

        private readonly Dictionary<Type, Sprite> sourceSpritesDictionary = new Dictionary<Type, Sprite>();
        public Dictionary<Type, Sprite> SourceSpritesDictionary
        {
            get
            {
                if (sourceSpritesDictionary.Count == 0)
                {
                    foreach (SourceSprite sourceSprite in sourceSprites)
                        sourceSpritesDictionary.Add(Type.GetType(sourceSprite.type), sourceSprite.sprite);
                }
                return sourceSpritesDictionary;
            }
        }

        public Sprite GetVisual(Type type)
        {
            return SourceSpritesDictionary[type];
        }
    }
}