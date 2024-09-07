
using System;
using Match3.Game.Gameplay.Core;
using Spine.Unity;
using UnityEngine;

namespace Match3.Presentation.Gameplay.Core
{
    public abstract class CellPresenter : MonoBehaviour, Foundation.Base.ComponentSystem.Component
    {

        protected Cell cell;

        public void Setup(Cell cell, CellStackPresenter cellStackPresenter)
        {
            this.cell = cell;
            cell.AddComponent(this);
            InternalSetup();
        }

        abstract protected void InternalSetup();

        public virtual void PlayHitAnimation(Action onCompleted)
        {
            onCompleted();
        }

        public Cell Cell()
        {
            return cell;
        }
    }
}