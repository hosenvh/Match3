using Match3.Presentation.TextAdapting;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.HUD
{
    // To better implementation we need to refactor HudElement to support specific hud type functionalities
    public class LifeHudController : MonoBehaviour
    {
        [SerializeField]
        GameObject fullLifeTextGameObject = default;
        
        [SerializeField]
        private Image infiniteLifeImage = null;
        
        [SerializeField]
        private TextAdapter lifeCountText = null, addLifeCounterText = null, infiniteLifeRemainTime = null;


        private void OnEnable()
        {
            GameProfiler.OnLifeTimerChangeEvent += Profiler_OnLifeTimerChangeEvent;
            GameProfiler.OnLifeCountChangeEvent += Profiler_OnLifeCountChangeEvent;
            GameProfiler.OnInfiniteLifeTimerChangeEvent += Profiler_OnInfiniteLifeTimerChangeEvent;
        }

        private void OnDisable()
        {
            GameProfiler.OnLifeTimerChangeEvent -= Profiler_OnLifeTimerChangeEvent;
            GameProfiler.OnLifeCountChangeEvent -= Profiler_OnLifeCountChangeEvent;
            GameProfiler.OnInfiniteLifeTimerChangeEvent -= Profiler_OnInfiniteLifeTimerChangeEvent;
        }

        void Start()
        {
            Profiler_OnLifeCountChangeEvent(Base.gameManager.profiler.LifeCount);
            Profiler_OnLifeTimerChangeEvent((int)Base.gameManager.profiler.LifeRefillTimer);
        }

        private void Profiler_OnLifeCountChangeEvent(int count)
        {
            if (count < Base.gameManager.profiler.GetMaxLifeCount())
            {
                fullLifeTextGameObject.SetActive(false);
                addLifeCounterText.gameObject.SetActive(true);
            }
            else
            {
                fullLifeTextGameObject.SetActive(true);
                addLifeCounterText.gameObject.SetActive(false);
            }

            SetLifeText(count);
        }

        private void Profiler_OnLifeTimerChangeEvent(int remainTime)
        {
            addLifeCounterText.SetText(string.Format("{0:00}:{1:00}", remainTime / 60, remainTime % 60));
        }

        private void Profiler_OnInfiniteLifeTimerChangeEvent(int remainTime)
        {
            infiniteLifeRemainTime.SetText(Utilities.GetFormatedTime(remainTime));
        }
        
        private void SetLifeText(int lifeCount)
        {
            if (lifeCount < 10)
            {
                lifeCountText.gameObject.SetActive(true);
                lifeCountText.SetText(lifeCount.ToString());
                infiniteLifeImage.gameObject.SetActive(false);
                infiniteLifeRemainTime.gameObject.SetActive(false);
            }
            else
            { 
                // Infinite life
                lifeCountText.gameObject.SetActive(false);
                infiniteLifeImage.gameObject.SetActive(true);
                infiniteLifeRemainTime.gameObject.SetActive(true);
                
                fullLifeTextGameObject.SetActive(false);
                addLifeCounterText.gameObject.SetActive(false);
            }
        }
        
        
        
    }
}


