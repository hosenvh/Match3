using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay.PrefabDatabases
{

    public interface CellAttachmentPrefabDatabase : Service
    {
        CellAttachmentPresenter PresenterPrefabFor(CellAttachment attachment);
    }

    // TODO: Make this more extensible.
    public class GameCellAttachmentPrefabDatabase : MonoBehaviour, CellAttachmentPrefabDatabase
    {
        [Serializable]
        public struct CellAttachmentPrefabEntry
        {
            [TypeAttribute(type: typeof(CellAttachment), includeAbstracts: false)]
            public string type;
            public CellAttachmentPresenter prefab;
        }

        public List<CellAttachmentPrefabEntry> cellAttachmentPrefabEntries;

        Dictionary<Type, CellAttachmentPresenter> cellAttachmentPrefabs;


        void Awake()
        {
            cellAttachmentPrefabs = new Dictionary<Type, CellAttachmentPresenter>();

            foreach (var entry in cellAttachmentPrefabEntries)
                cellAttachmentPrefabs.Add(Type.GetType(entry.type), entry.prefab);
        }

        public CellAttachmentPresenter PresenterPrefabFor(CellAttachment attachment)
        {
            if (cellAttachmentPrefabs.ContainsKey(attachment.GetType()))
                return cellAttachmentPrefabs[attachment.GetType()];

            throw new System.Exception(string.Format("Can't find the tile presenter for {0}", attachment.GetType()));
        }
    }
}