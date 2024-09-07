using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Game.Gameplay.Core;
using Match3.Presentation.Gameplay.Core;
using UnityEngine;
using DG.Tweening;

namespace Match3.Presentation.Gameplay.PowerUpActivation
{
    public class HandPowerUpEffect : MonoBehaviour
    {
        public float swapSpeed;


        public void Play(CellStack origin, CellStack destination, Action onCompleted)
        {
            var originTilePresenter = origin.CurrentTileStack().GetComponent<TileStackPresenter>();
            var destinationTilePresenter = destination.CurrentTileStack().GetComponent<TileStackPresenter>();

            originTilePresenter.SetLogicPositionUpdateFlag(false);
            destinationTilePresenter.SetLogicPositionUpdateFlag(false);

            this.transform.position = originTilePresenter.transform.position;


            this.transform.
                DOMove(destinationTilePresenter.transform.position, swapSpeed).
                SetSpeedBased(true).
                OnComplete(() => CompleteSwap(originTilePresenter, destinationTilePresenter, onCompleted));

            originTilePresenter.transform.DOMove(destinationTilePresenter.transform.position, swapSpeed).SetSpeedBased(true);
            destinationTilePresenter.transform.DOMove(originTilePresenter.transform.position, swapSpeed).SetSpeedBased(true);

        }

        void CompleteSwap(TileStackPresenter origin, TileStackPresenter destination, Action onCompleted)
        {
            origin.SetLogicPositionUpdateFlag(true);
            destination.SetLogicPositionUpdateFlag(true);

            onCompleted();
            Destroy(this.gameObject);
        }
    }
}