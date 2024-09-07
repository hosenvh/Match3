using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.Core;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation.Gameplay.PrefabDatabases
{
    public interface TilePrefabDatabase : Service
    {
        TilePresenter PresenterPrefabFor(Tile tile);
    }

    public class TilePoolDatabase: MonoBehaviour
    {
        
    }

    public class GameTilePrefabDatabase : MonoBehaviour, TilePrefabDatabase
    {
        [Serializable]
        public struct TileTypePrefabEntry
        {
            [TypeAttribute(type : typeof(Tile), includeAbstracts : false)]
            public string type;
            public TilePresenter prefab;
        }

        [Serializable]
        public struct ColorTileTypePrefabEntry
        {
            [TypeAttribute(type: typeof(Tile), includeAbstracts: false)]
            public string type;

            [ArrayElementTitle(nameof(ColoredTilePrefabEntry.color))]
            public List<ColoredTilePrefabEntry> colorPrefabs;

        }

        [Serializable]
        public struct ColoredTilePrefabEntry
        {
            public TileColor color;
            public TilePresenter prefab;
        }

        [Serializable]
        public struct BeachBallPrefabEntry
        {
            public int colorsCount;
            public TilePresenter prefab;
        }

        [Serializable]
        public struct BalloonTiedTilesPrefabEntry
        {
            public Vector2Int relationalDirectionFromItsMainBalloonTile;
            public TilePresenter prefab;
        }

        public TilePoolDatabase tilePoolDatabase;

        public List<ColorTileTypePrefabEntry> coloredTileTypePrefabEntries;
        public List<TileTypePrefabEntry> tilePrefabEntries;
        public List<BeachBallPrefabEntry> beachBallPrefabEntries;
        public List<BalloonTiedTilesPrefabEntry> balloonTiedTilesPrefabEntries;

        Dictionary<Type, TilePresenter> tilePrefabs;
        Dictionary<Type, Dictionary<TileColor, TilePresenter>> coloredTilePrefabs;
        Dictionary<int, TilePresenter> beachBallPrefabs;
        Dictionary<Vector2Int, TilePresenter> balloonPrefabs;

        void Awake()
        {
            tilePrefabs = new Dictionary<Type, TilePresenter>();
            coloredTilePrefabs = new Dictionary<Type, Dictionary<TileColor, TilePresenter>>();
            beachBallPrefabs = new Dictionary<int, TilePresenter>();
            balloonPrefabs = new Dictionary<Vector2Int, TilePresenter>();

            foreach (var entry in tilePrefabEntries)
                tilePrefabs.Add(Type.GetType(entry.type), entry.prefab);


            foreach (var typeEntry in coloredTileTypePrefabEntries)
            {
                var type = Type.GetType(typeEntry.type);
                coloredTilePrefabs.Add(type, new Dictionary<TileColor, TilePresenter>());
                foreach (var colorEntry in typeEntry.colorPrefabs)
                {
                    coloredTilePrefabs[type].Add(colorEntry.color, colorEntry.prefab);
                }
            }

            foreach (var entry in beachBallPrefabEntries)
                beachBallPrefabs.Add(entry.colorsCount, entry.prefab);

            foreach (BalloonTiedTilesPrefabEntry entry in balloonTiedTilesPrefabEntries)
                balloonPrefabs.Add(entry.relationalDirectionFromItsMainBalloonTile, entry.prefab);
        }

        public TilePresenter PresenterPrefabFor(Tile tile)
        {
            TileColor tileColor = ExtractTileColorOf(tile);

            if (tileColor != TileColor.None)
            {
                if (coloredTilePrefabs.ContainsKey(tile.GetType()))
                    return coloredTilePrefabs[tile.GetType()][tileColor];
            }

            else if (tile is BeachBallMainTile beachBallMainTile)
                return beachBallPrefabs[beachBallMainTile.RemainingColors().Count];

            else if (tile is BalloonTiedTile balloonTiedTile)
                return balloonPrefabs[balloonTiedTile.GetRelationalDirectionFromItsMainBalloonTile()];

            if (tilePrefabs.ContainsKey(tile.GetType()))
                return tilePrefabs[tile.GetType()];

            throw new System.Exception(string.Format("Can't find the tile presenter for {0}", tile.GetType()));
        }

        private TileColor ExtractTileColorOf(Tile tile)
        {
            if (tile is VacuumCleaner vacuumCleaner)
                return vacuumCleaner.targetColor;

            var colorComponent = tile.componentCache.colorComponent;
            if (colorComponent != null)
                return colorComponent.color;

            return TileColor.None;
        }

    }
}