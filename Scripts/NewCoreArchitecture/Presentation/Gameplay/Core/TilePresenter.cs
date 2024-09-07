using Match3.Foundation.Base.Destruction;
using Match3.Game.Gameplay.Core;
using System;
using Match3.Game.Effects;
using UnityEngine;
using UnityEngine.Events;

namespace Match3.Presentation.Gameplay.Core
{

    [System.Serializable]
    public class TileUnityEvent : UnityEvent<TilePresenter>
    { }


    public abstract class TilePresenter : MonoBehaviour, Foundation.Base.ComponentSystem.Component, Destroyable
    {
        public TileUnityEvent onHit;

        public ItemGlowEffectController glowEffectController;
        
        protected Tile tile;
        protected TileStackPresenter owner;

        Action<TilePresenter> destroyAction = null;

        public void Setup(Tile tile, TileStackPresenter owner)
        {
            this.tile = tile;
            this.owner = owner;
            tile.AddComponent(this);


            InternalSetup();
        }

        abstract protected void InternalSetup();

        public void PlayHitEffect(Action onCompleted)
        {
            onHit.Invoke(this);
            PlayHitAnimation(onCompleted);
        }

        protected virtual void PlayHitAnimation(Action onCompleted)
        {
            onCompleted();
        }

        public TileStackPresenter Owner()
        {
            return owner;
        }

        public Tile Tile()
        {
            return tile;
        }

        public void SetDestroyAction(Action<TilePresenter> action)
        {
            this.destroyAction = action;
        }

        public void Destroy()
        {
            if (destroyAction != null)
                destroyAction.Invoke(this);
            else
                UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}