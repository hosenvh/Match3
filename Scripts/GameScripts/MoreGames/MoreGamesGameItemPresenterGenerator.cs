using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.MoreGames
{
    public class MoreGamesGameItemPresenterGenerator
    {
        private MoreGamesGameItemData[] gameItemsData;
        private ResourceGameObjectAsset gameItemPresenterResourceAsset;

        public void Setup(ResourceGameObjectAsset gameItemPresenter, MoreGamesGameItemData[] gamesData)
        {
            gameItemsData = gamesData;
            gameItemPresenterResourceAsset = gameItemPresenter;
        }

        public void GenerateAllGamePresenters(Transform parent)
        {
            MoreGamesGameItemPresenter[] gameItemPresenters = new MoreGamesGameItemPresenter[gameItemsData.Length];
            var gameItemPresenterPrefab = gameItemPresenterResourceAsset.Load().GetComponent<MoreGamesGameItemPresenter>();
            for (int i = 0; i < gameItemPresenters.Length; i++)
            {
                gameItemPresenters[i] = Object.Instantiate(gameItemPresenterPrefab, parent);
                int t = i;
                gameItemPresenters[i].Setup(gameSprite: gameItemsData[i].icon.Load(), title: gameItemsData[i].title, onButtonClick: () => HandleGameButtonClick(gameItemsData[t]));
            }

            void HandleGameButtonClick(MoreGamesGameItemData gameItemData)
            {
                ServiceLocator.Find<EventManager>().Propagate(new MoreGameIconClickedEvent(gameItemData.gameAnalyticsTitle), sender: this);
                Application.OpenURL(gameItemData.link);
            }
        }
    }
}