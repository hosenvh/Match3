using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Unity;
using Match3.Foundation.Unity.Configuration;
using UnityEngine;


namespace Match3.MoreGames
{
    [CreateAssetMenu(menuName =  "Data/MoreGames", fileName = nameof(MoreGamesConfigurer))]
    public class MoreGamesConfigurer : ScriptableConfiguration, Configurer<MoreGamesGameItemPresenterGenerator>
    {
        [SerializeField] private ResourceGameObjectAsset gameItemPresenterAsset;
        [SerializeField] private MoreGamesGameItemData[] games;

        public void Configure(MoreGamesGameItemPresenterGenerator entity)
        {
            entity.Setup(gameItemPresenterAsset, games);
        }

        public override void RegisterSelf(ConfigurationManager manager)
        {
            manager.Register(this);
        }
    }
}