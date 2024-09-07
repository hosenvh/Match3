using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;


namespace Match3
{
    public class GooglePlayScript : MonoBehaviour
    {
        void Start()
        {
            var config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.Activate();
        }
    }
}