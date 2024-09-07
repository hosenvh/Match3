using System.Collections;
using DG.Tweening;
using KitchenParadise.Presentation;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Gameplay;
using Match3.Game.Gameplay.Input;
using Match3.Game.Gameplay.SubSystemsData.SessionData;
using Match3.Game.Gameplay.Swapping;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{

    public class GameplayCacheFixer : MonoBehaviour
    {
        public GameplayStateRoot gameplayStateRoot;
        public GameObject emptyObject1;
        public GameObject emptyObject2;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            emptyObject1.transform.
              DoMoveWithForce(0.4f, 60, this.transform.position, 800, false).
              SetSpeedBased(true).
              SetEase(Ease.InSine).
              OnComplete(
              () => EmptyFunction());


            var source = ServiceLocator.Find<GameplaySoundManager>().FindAnAvailableFloatingAudioSource();

            var gpc = gameplayStateRoot.gameplayState.gameplayController;

            gpc.GetSystem<InputSystem>().GetType();

            var cellStacks = gpc.GameBoard().ArrbitrayCellStackArray();

        }

        void EmptyFunction()
        {

        }
    }
}