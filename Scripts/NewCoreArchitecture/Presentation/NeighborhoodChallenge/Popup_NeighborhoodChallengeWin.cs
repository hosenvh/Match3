using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.NeighborhoodChallenge;
using SeganX;
using Match3.Presentation.Gameplay;
using UnityEngine;
using UnityEngine.Events;



namespace Match3.Presentation.NeighborhoodChallenge
{
    public class Popup_NeighborhoodChallengeWin : Popup_WinBase
    {
        [SerializeField] private LocalText scoreText = default;
        public UnityEvent onScoreSubmitted;

        public CounterUI candyCounter;



        public override void InternalSetup()
        {
            scoreText.SetText(score.ToString());

            var userInfo = ServiceLocator.Find<NeighborhoodChallengeManager>().UserInfo();
            candyCounter.Setup(() => userInfo.Score);
            candyCounter.SetDelta(0);
        }

        public void OnScoreSubmitted()
        {
            onScoreSubmitted.Invoke();
        }
        
    }
}