using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Gameplay;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation
{

    [RequireComponent(typeof(Toggle))]
    public class ToggleSoundPlayer : MonoBehaviour
    {
        public Toggle myToggle;
        public AudioClip sound;
        public float volume = 0.3f;

        private GameplaySoundManager _soundManager;
        
        //TODO: Cache necessary components in editor
        
        private void Start()
        {
            GetToggleComponent();
            
            _soundManager = ServiceLocator.Find<GameplaySoundManager>();
            myToggle.onValueChanged.AddListener(isOn => Play());
        }

        public void Play()
        {
            _soundManager.TryPlay(sound, false, volume);
        }

        private void GetToggleComponent()
        {
            if (myToggle == null) 
                myToggle = transform.GetComponent<Toggle>();
        }
        
        
    }

}