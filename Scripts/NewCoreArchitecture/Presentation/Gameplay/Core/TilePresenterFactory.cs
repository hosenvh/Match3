using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Factories;
using Match3.Game.Gameplay.Matching;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.PrefabDatabases;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Match3.Presentation.Gameplay.Core
{
    public class TilePresenterFactory : MonoBehaviour
    {

        [SerializeField]
        public class PoolingInfo
        {
            [Type(typeof(Tile), includeAbstracts:false)]
            public string type;
            public int amount;
        }

        public int reserveCount;
        public List<PoolingInfo> poolingInfos;
        public Transform poolingContainer;

        TilePrefabDatabase tilePrefabDatabase;
        TileFactory tileFactory;

        Dictionary<TileColor, UnityTilePresenterPool> coloredBeadsPools = new Dictionary<TileColor, UnityTilePresenterPool>();

        Dictionary<Type, UnityTilePresenterPool> generalPools = new Dictionary<Type, UnityTilePresenterPool>();

        public void Setup()
        {
            this.tilePrefabDatabase = ServiceLocator.Find<TilePrefabDatabase>();
            this.tileFactory = ServiceLocator.Find<TileFactory>();

            SetupPoolForColoredBead(TileColor.Blue);
            SetupPoolForColoredBead(TileColor.Green);
            SetupPoolForColoredBead(TileColor.Pink);
            SetupPoolForColoredBead(TileColor.Purple);
            SetupPoolForColoredBead(TileColor.Red);
            SetupPoolForColoredBead(TileColor.Yellow);

            SetupGeneralPoolFor(tileFactory.CreateRocketTile(), 3);
            SetupGeneralPoolFor(tileFactory.CreateBombTile(), 3);
            SetupGeneralPoolFor(tileFactory.CreateDynamiteTile(), 2);
            SetupGeneralPoolFor(tileFactory.CreateTNTBarrelTile(), 1);

        }

        private void SetupGeneralPoolFor(Tile tile, int reserve)
        {
            var pool = new UnityTilePresenterPool();

            pool.SetPoolTransform(poolingContainer);
            pool.SetComponentPrefab(tilePrefabDatabase.PresenterPrefabFor(tile));

            pool.Reserve(reserve);

            generalPools.Add(tile.GetType(), pool);
        }

        void SetupPoolForColoredBead(TileColor color)
        {
            var pool = new UnityTilePresenterPool();

            pool.SetPoolTransform(poolingContainer);
            pool.SetComponentPrefab(tilePrefabDatabase.PresenterPrefabFor(tileFactory.CreateCleanColoredBeadTile(color)));

            pool.Reserve(reserveCount);

            coloredBeadsPools.Add(color, pool);
        }

        public TilePresenter CreateFor(Tile tile, Transform parent)
        {
            TilePresenter presenter;
            UnityTilePresenterPool pool = null;

            pool = TryFindPoolFor(tile);

            if (pool != null)
            {
                presenter = pool.Acquire();
                presenter.transform.SetParent(parent, false);
                presenter.SetDestroyAction(pool.Release);
            }
            else
                presenter = UnityEngine.Object.Instantiate(tilePrefabDatabase.PresenterPrefabFor(tile), parent, false);

            return presenter;
        }

        private UnityTilePresenterPool TryFindPoolFor(Tile tile)
        {
            if (tile is ColoredBead colorBead)
                return coloredBeadsPools[colorBead.GetComponent<TileColorComponent>().color];
            else if (generalPools.ContainsKey(tile.GetType()))
                return generalPools[tile.GetType()];

            return null;
        }
    }
}
