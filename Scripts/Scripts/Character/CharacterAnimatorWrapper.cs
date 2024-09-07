using Match3.CharacterManagement.Character.Base;
using Match3.CharacterManagement.Character.Game;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game.Map;
using UnityEngine;
using UnityEngine.Serialization;


namespace Match3
{
    public class CharacterAnimatorWrapper : MonoBehaviour
    {
        [FormerlySerializedAs("character")]
        [SerializeField] private CharacterName characterName;

        private CharacterAnimator characterAnimator = null;

        public CharacterName CharacterName => characterName;

        private MapCharactersManager CharactersManager => Base.gameManager.mapCharactersManager;
        private MapManager MapManager => Base.gameManager.mapManager;
        
        
        
        private void Awake()
        {
            ConvertAndDeleteOldSingleMapData();
            CharactersManager.RegisterAnimatorWrapper(this);
        }
        
        
        
        public void SetupAnimator()
        {
            var position = GetStartPositionAtCurrentMap();
            var scale = GetStartScaleAtCurrentMap();
            if (IsObjectOnScene(position))
                GetCharacterAnimator().Setup(position, scale);
        }

        private Vector3 GetStartPositionAtCurrentMap()
        {
            return CharactersManager.GetCharacterStartPositionAtMap(characterName, MapManager.CurrentMapId);
        }

        private Vector3 GetStartScaleAtCurrentMap()
        {
            return CharactersManager.GetCharacterStartScaleAtMap(characterName, MapManager.CurrentMapId);
        }
        
        private bool IsObjectOnScene(Vector3 position)
        {
            return (position.x > 40.0f && position.x < 270.0f &&
                    position.y >= 0.0f &&
                    position.z > -110.0f && position.z < 60.0f);
        }
        
        

        private void ConvertAndDeleteOldSingleMapData()
        {
            var firstMapPosition = GetOldSingleMapStartPosition(defaultValue:Vector3.zero);
            if (firstMapPosition != Vector3.zero)
            {
                CharactersManager.SetCharacterStartPositionAtMap(characterName, firstMapPosition, MapManager.DefaultMapId);
                PlayerPrefs.DeleteKey(GetOldSingleMapStartPositionKey(0));
                PlayerPrefs.DeleteKey(GetOldSingleMapStartPositionKey(1));
                PlayerPrefs.DeleteKey(GetOldSingleMapStartPositionKey(2));
            }

            var firstMapScale = GetOldSingleMapStartScale(defaultValue:Vector3.one);
            if (firstMapScale != Vector3.one)
            {
                CharactersManager.SetCharacterStartScaleAtMap(characterName, firstMapScale, MapManager.DefaultMapId);
                PlayerPrefs.DeleteKey(GetOldSingleMapStartScaleKey(0));
                PlayerPrefs.DeleteKey(GetOldSingleMapStartScaleKey(1));
                PlayerPrefs.DeleteKey(GetOldSingleMapStartScaleKey(2));
            }
        }
        
        private Vector3 GetOldSingleMapStartPosition(Vector3 defaultValue)
        {
            return new Vector3(
                PlayerPrefs.GetFloat(GetOldSingleMapStartPositionKey(0), defaultValue.x),
                PlayerPrefs.GetFloat(GetOldSingleMapStartPositionKey(1), defaultValue.y), 
                PlayerPrefs.GetFloat(GetOldSingleMapStartPositionKey(2), defaultValue.z));
        }

        private string GetOldSingleMapStartPositionKey(int vectorIndex)
        {
            return "CharacterStartPosition" + characterName.ToString().ToLower() + vectorIndex.ToString();
        }

        private Vector3 GetOldSingleMapStartScale(Vector3 defaultValue)
        {
            return new Vector3(
                PlayerPrefs.GetFloat(GetOldSingleMapStartScaleKey(0), defaultValue.x),
                PlayerPrefs.GetFloat(GetOldSingleMapStartScaleKey(1), defaultValue.y),
                PlayerPrefs.GetFloat(GetOldSingleMapStartScaleKey(2), defaultValue.z));
        }

        private string GetOldSingleMapStartScaleKey(int vectorIndex)
        {
            return "CharacterStartScale" + characterName.ToString().ToLower() + vectorIndex.ToString();
        }


        
        public CharacterAnimator GetCharacterAnimator()
        {
            if (characterAnimator != null)
                return characterAnimator;

            characterAnimator = InstantiateCharacterAnimator();

            return characterAnimator;
        }

        CharacterAnimator InstantiateCharacterAnimator()
        {
            var characterPrefab = ServiceLocator.Find<CharactersManager>().LoadCharacterAnimator(characterName);
            return Instantiate<CharacterAnimator>(characterPrefab, this.transform);
        }

        public void ForceUpdateCharacterAnimator()
        {
            if(characterAnimator == null)
                return;
            var updatedCharacterAnimator = InstantiateNewCharacterAnimatorWithCurrentAnimatorProperties();
            characterAnimator.onFinish = null;
            characterAnimator.onScalingFinish = null;
            RemoveCharacterAnimator();
            characterAnimator = updatedCharacterAnimator;

            CharacterAnimator InstantiateNewCharacterAnimatorWithCurrentAnimatorProperties()
            {
                var newCharacterAnimator = InstantiateCharacterAnimator();
                newCharacterAnimator.transform.position = characterAnimator.transform.position;
                newCharacterAnimator.transform.rotation = characterAnimator.transform.rotation;
                newCharacterAnimator.transform.localScale = characterAnimator.transform.localScale;

                newCharacterAnimator.targetPosition = characterAnimator.targetPosition;
                newCharacterAnimator.targetRotation = characterAnimator.targetRotation;
                newCharacterAnimator.targetScaleFactor = characterAnimator.targetScaleFactor;
                newCharacterAnimator.moving = characterAnimator.moving;
                newCharacterAnimator.scaling = characterAnimator.scaling;
                newCharacterAnimator.walkSpeed = characterAnimator.walkSpeed;
                newCharacterAnimator.onFinish = characterAnimator.onFinish;
                newCharacterAnimator.onScalingFinish = characterAnimator.onScalingFinish;

                return newCharacterAnimator;
            }
        }
        
        
        
        public void RemoveCharacterAnimator()
        {
            if (characterAnimator != null)
                Destroy(characterAnimator.gameObject);
        }
    }
}