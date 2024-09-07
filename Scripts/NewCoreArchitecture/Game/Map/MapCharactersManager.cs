using System.Collections.Generic;
using Match3.CharacterManagement.Character.Base;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Game.Map
{
    public class MapCharactersManager : MonoBehaviour
    {
        
        private List<CharacterAnimatorWrapper> characterAnimatorWrappers = new List<CharacterAnimatorWrapper>();
        
        
        
        public void RegisterAnimatorWrapper(CharacterAnimatorWrapper characterAnimatorWrapper)
        {
            characterAnimatorWrappers.Add(characterAnimatorWrapper);
        }
        
        
        public void SetupCharactersAnimator()
        {
            foreach (var characterAnimatorWrapper in characterAnimatorWrappers)
                characterAnimatorWrapper.SetupAnimator();
        }
        
        
        
        public void SetCharacterStartPositionAtMap(CharacterName characterName, Vector3 position, string mapId)
        {
            PlayerPrefs.SetFloat(CharacterStartPositionKey(characterName, 0, mapId), position.x);
            PlayerPrefs.SetFloat(CharacterStartPositionKey(characterName, 1, mapId), position.y);
            PlayerPrefs.SetFloat(CharacterStartPositionKey(characterName, 2, mapId), position.z);
        }

        public void SetCharacterStartScaleAtMap(CharacterName characterName, Vector3 scale, string mapId)
        {
            PlayerPrefs.SetFloat(CharacterStartScaleKey(characterName, vectorIndex: 0, mapId), scale.x);
            PlayerPrefs.SetFloat(CharacterStartScaleKey(characterName, vectorIndex: 1, mapId), scale.y);
            PlayerPrefs.SetFloat(CharacterStartScaleKey(characterName, vectorIndex: 2, mapId), scale.z);
        }

        public Vector3 GetCharacterStartPositionAtMap(CharacterName characterName, string mapId)
        {
            Vector3 position = new Vector3(
                PlayerPrefs.GetFloat(CharacterStartPositionKey(characterName, 0, mapId)),
                PlayerPrefs.GetFloat(CharacterStartPositionKey(characterName, 1, mapId)),
                PlayerPrefs.GetFloat(CharacterStartPositionKey(characterName, 2, mapId)));
            return position;
        }

        public Vector3 GetCharacterStartScaleAtMap(CharacterName characterName, string mapId)
        {
            Vector3 scale = new Vector3(
                PlayerPrefs.GetFloat(CharacterStartScaleKey(characterName, vectorIndex: 0, mapId), defaultValue: 1f),
                PlayerPrefs.GetFloat(CharacterStartScaleKey(characterName, vectorIndex: 1, mapId), defaultValue: 1f),
                PlayerPrefs.GetFloat(CharacterStartScaleKey(characterName, vectorIndex: 2, mapId), defaultValue: 1f));
            return scale;
        }

        private string CharacterStartPositionKey(CharacterName characterName, int vectorIndex, string mapId)
        {
            return $"CharacterStartPosition_{characterName.ToString().ToLower()}_{mapId}_{vectorIndex.ToString()}";
        }
        
        private string CharacterStartScaleKey(CharacterName characterName, int vectorIndex, string mapId)
        {
            return $"CharacterStartScale_{characterName.ToString().ToLower()}_{mapId}_{vectorIndex.ToString()}";
        }
        
        public CharacterAnimator GetCharacter(CharacterName characterName)
        {
#if UNITY_EDITOR
            var characterWrapper = GetCharacterWrapper(characterName);
            var animator = characterWrapper.gameObject.GetComponentInChildren<CharacterAnimator>();
            if (animator != null)
                return animator;
#endif
            return GetCharacterWrapper(characterName).GetCharacterAnimator();
        }

        
        public CharacterAnimatorWrapper GetCharacterWrapper(CharacterName characterName)
        {
            CharacterAnimatorWrapper requestedAnimatorWrapper = null;
            foreach (var characterAnimatorWrapper in characterAnimatorWrappers)
            {
                if (characterName == characterAnimatorWrapper.CharacterName)
                    requestedAnimatorWrapper = characterAnimatorWrapper;
            }
            
            // if(requestedAnimatorWrapper == null)
                // DebugPro.LogError<MapLogTag>($"Please add character: {characterName}");
            
            return requestedAnimatorWrapper;
        }


        public void ClearCurrentMapCharactersAnimatorWrappers()
        {
            characterAnimatorWrappers.Clear();
        }
        
        
        
        public void ResetCharactersStartPositions()
        {
            var mapsMetaData = Base.gameManager.mapManager.MapsMetaDatabase.MapsMetadata;
            foreach (var mapMetaData in mapsMetaData)
            {
                foreach (var characterAnimatorWrapper in characterAnimatorWrappers)
                {
                    DeleteStartPositionData(characterAnimatorWrapper.CharacterName, mapMetaData.mapId);
                }  
            }
        }

        public void ResetCharactersStartScales()
        {
            var mapsMetaData = Base.gameManager.mapManager.MapsMetaDatabase.MapsMetadata;
            foreach (var mapMetaData in mapsMetaData)
            {
                foreach (var characterAnimatorWrapper in characterAnimatorWrappers)
                {
                    DeleteStartScaleData(characterAnimatorWrapper.CharacterName, mapMetaData.mapId);
                }
            }
        }
        
        private void DeleteStartPositionData(CharacterName characterName, string mapId)
        {
            PlayerPrefs.DeleteKey(CharacterStartPositionKey(characterName, 0, mapId));
            PlayerPrefs.DeleteKey(CharacterStartPositionKey(characterName, 1, mapId));
            PlayerPrefs.DeleteKey(CharacterStartPositionKey(characterName, 2, mapId));
        }

        private void DeleteStartScaleData(CharacterName characterName, string mapId)
        {
            PlayerPrefs.DeleteKey(CharacterStartScaleKey(characterName, 0, mapId));
            PlayerPrefs.DeleteKey(CharacterStartScaleKey(characterName, 1, mapId));
            PlayerPrefs.DeleteKey(CharacterStartScaleKey(characterName, 2, mapId));
        }
        
    }
}