using Match3.Foundation.Base.ServiceLocating;
using Match3.Presentation.Gameplay;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation
{
    
    [RequireComponent(typeof(Button))]
    public class ButtonSoundPlayer : MonoBehaviour
    {
        public Button myButton;
        public AudioClip sound;
        public float volume = 0.3f;
        
        private GameplaySoundManager _soundManager;

        //TODO: Cache necessary components in editor
        
        private void Start()
        {
            GetButtonComponent();
            
            _soundManager = ServiceLocator.Find<GameplaySoundManager>();
            myButton.onClick.AddListener(Play);
        }

        public void Play()
        {
            _soundManager.TryPlay(sound, false, volume);
        }

        private void GetButtonComponent()
        {
            if (myButton == null)
                myButton = transform.GetComponent<Button>();
        }
        
        
    }
    
}


