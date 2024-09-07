using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.MoreGames
{
    public class MoreGamesTab : MonoBehaviour
    {
        [SerializeField] private GameObject tabButton;
        [SerializeField] private Transform contentParent;

        public void Setup()
        {
            var gameItemPresenterGenerator = new MoreGamesGameItemPresenterGenerator();
            ServiceLocator.Find<ConfigurationManager>().Configure(gameItemPresenterGenerator);
            gameItemPresenterGenerator.GenerateAllGamePresenters(contentParent);

            if (NoGameItemAdded())
                tabButton.SetActive(false);

            bool NoGameItemAdded()
            {
                return contentParent.childCount == 0;
            }
        }
    }
}