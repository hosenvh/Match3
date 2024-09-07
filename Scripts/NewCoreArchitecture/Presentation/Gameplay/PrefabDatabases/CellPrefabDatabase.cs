
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PrefabDatabases
{
    // TODO: Make this more extensible.
    public class CellPrefabDatabase : MonoBehaviour, Service
    {

        [Serializable]
        public struct CellPrefabEntry
        {
            [TypeAttribute(type: typeof(Cell), includeAbstracts: false)]
            public string type;
            public Core.CellPresenter prefab;
        }

        public List<CellPrefabEntry> cellPrefabEntries;

        Dictionary<Type, Core.CellPresenter> cellPrefabs;


        void Awake()
        {
            cellPrefabs = new Dictionary<Type, Core.CellPresenter>();

            foreach (var entry in cellPrefabEntries)
                cellPrefabs.Add(Type.GetType(entry.type), entry.prefab);
        }

        public Core.CellPresenter PresenterPrefabFor(Cell cell)
        {
            if (cellPrefabs.ContainsKey(cell.GetType()))
                return cellPrefabs[cell.GetType()];

            throw new System.Exception(string.Format("Can't find the tile presenter for {0}", cell.GetType()));
        }

    }
}