using UnityEngine;

namespace Match3.Presentation.Gameplay.LogicPresentationHandlers
{
    // TODO: Rename  this.
    public class PresentationController : MonoBehaviour
    {
        protected GameplayState gameState;

        public void Setup(GameplayState gameState)
        {
            this.gameState = gameState;
            InternalSetup(gameState);
        }

        virtual protected void InternalSetup(GameplayState gameState)
        {

        }
    }
}