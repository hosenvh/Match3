using Match3.Foundation.Unity.ObjectPooling;
using Match3.Game.Gameplay.CellAttachments;
using Match3.Game.Gameplay.Cells;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.DestructionManagement;
using Match3.Game.Gameplay.Tiles;
using Match3.Presentation.Gameplay.CellAttachments;
using Match3.Presentation.Gameplay.Cells;
using Match3.Presentation.Gameplay.Core;
using Match3.Presentation.Gameplay.LevelConditionManagement;
using Match3.Presentation.Gameplay.Tiles;
using PandasCanPlay.HexaWord.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public static class PresentationDestructionUtility
    {
        public static void Destroy(Tile tile, Action<Tile> onComplete)
        {
            tile.GetComponent<TilePresenter>().PlayHitEffect(() => DestroyTilePresenterFor(tile, onComplete));
        }

        static void DestroyTilePresenterFor(Tile tile, Action<Tile> onComplete)
        {
            tile.GetComponent<TilePresenter>().Destroy();
            onComplete(tile);
        }

        public static void Destroy(Cell cell, Action<Cell> onComplete)
        {
            cell.GetComponent<Core.CellPresenter>().PlayHitAnimation(() => DestroyCellPresenterFor(cell, onComplete));
        }


        static void DestroyCellPresenterFor(Cell cell, Action<Cell> onComplete)
        {
            GameObject.Destroy(cell.GetComponent<Core.CellPresenter>().gameObject);
            onComplete(cell);
        }

        // NOTE: Hit management for attachment is not yet fully defined.
        // TODO: Refactor this when hit management for attachments is fully definedl
        public static void Destroy(HitableCellAttachment cellAttachment, Action<HitableCellAttachment> onComplete)
        {
            var presenter = cellAttachment.GetComponent<CellAttachmentPresenter>();
            if (presenter is RopePresenter ropePresenter)
                ropePresenter.PlayHitEffect(() => DestroyAttachmentPresenterFor(cellAttachment, presenter, onComplete));
            else
                onComplete.Invoke(cellAttachment);
        }

        private static void DestroyAttachmentPresenterFor(HitableCellAttachment cellAttachment, CellAttachmentPresenter attachmentPresenter, Action<HitableCellAttachment> onComplete)
        {
            GameObject.Destroy(attachmentPresenter.gameObject);
            onComplete(cellAttachment);
        }
    }

    public class PresentationDestructionHandler : MonoBehaviour, DefaultDestructionPresentationHandler
    {
        [Serializable]
        public struct ExplosionParticleObjectPoolData
        {
            [TypeAttribute(typeof(ExplosiveTile), includeAbstracts: false)]
            public string type;
            public ParticleObjectPool pool;
        }

        public List<ExplosionParticleObjectPoolData> explosionParticlePools;

        public ExplosionPresentationFeedbackHandler explosionPresentationFeedbackHandler;

        public Transform destructionTopContainer;

        Dictionary<Type, ParticleObjectPool> objectPools = new Dictionary<Type, ParticleObjectPool>();

        void Awake()
        {
            foreach (var element in explosionParticlePools)
                objectPools.Add(Type.GetType(element.type), element.pool);
        }


        public void Destroy(Tile tile, Action<Tile> onComplete)
        {
            PresentationDestructionUtility.Destroy(tile, onComplete);

            TryPlayExplostionEffect(tile);

            explosionPresentationFeedbackHandler.Handle(tile);
        }

        public void TryPlayExplostionEffect(Tile tile)
        {
            if (tile is ExplosiveTile)
                PlayExplosionEffect(tile as ExplosiveTile);
        }


        public void Destroy(Cell cell, Action<Cell> onComplete)
        {
            if (ShouldMoveToTop(cell))
                cell.GetComponent<CellPresenter>().transform.SetParent(destructionTopContainer);

            PresentationDestructionUtility.Destroy(cell, onComplete);
        }


        public void Destroy(HitableCellAttachment attachment, Action<HitableCellAttachment> onComplete)
        {
            PresentationDestructionUtility.Destroy(attachment, onComplete);
        }

        public void PlayExplosionEffect(ExplosiveTile explosiveTile)
        {
            var presenter = explosiveTile.GetComponent<ExplosiveTilePresenter>();
            Vector3 pos = presenter.transform.position;
            pos.z -= 50;

            ParticleSystem particle = null;

            particle = objectPools[explosiveTile.GetType()].Acquire();
            particle.transform.position = pos;
        }

        private bool ShouldMoveToTop(Cell cell)
        {
            return cell is AbstractArtifactMainCell;
        }

    }
}