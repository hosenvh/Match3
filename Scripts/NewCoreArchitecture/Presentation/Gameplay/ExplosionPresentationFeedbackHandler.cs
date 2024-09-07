using KitchenParadise.Utiltiy.Base;
using Match3.Game.Gameplay.Core;
using Match3.Game.Gameplay.Tiles.Explosives;
using Match3.Presentation.Gameplay.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Presentation.Gameplay
{
    public class ExplosionPresentationFeedbackHandler : MonoBehaviour
    {
        public List<GeneralGameplayFeedBack> feedbackPrefabs;
        public Transform targetTransform;

        public void Handle(Tile tile)
        {
            if (tile is TNTBarrel)
                PlayAFeedbackIn(tile.GetComponent<TilePresenter>().transform.position);
        }

        private void PlayAFeedbackIn(Vector3 position)
        {
            var feedback = Instantiate(feedbackPrefabs.RandomElement(), targetTransform, false);
            feedback.transform.position = position;
            feedback.Play();
        }
    }
}