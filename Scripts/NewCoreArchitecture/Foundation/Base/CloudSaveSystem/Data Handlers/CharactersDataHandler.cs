using System;
using Match3.CharacterManagement.Character.Base;
using UnityEngine;


namespace Match3.CloudSave
{

    public class CharactersDataHandler : ICloudDataHandler
    {

        private const string CharacterPosKey = "characterPos"; 
        private const string CharacterScaleKey = "characterScale";

        
        public void CollectData(ICloudDataStorage cloudStorage)
        {
            var mapsMetaData = Base.gameManager.mapManager.MapsMetaDatabase.MapsMetadata;
            var mapCharactersManager = Base.gameManager.mapCharactersManager;

            foreach (var mapMetaData in mapsMetaData)
            {
                foreach (CharacterName character in (CharacterName[]) Enum.GetValues(typeof(CharacterName)))
                {
                    var startPosition =
                        mapCharactersManager.GetCharacterStartPositionAtMap(character, mapMetaData.mapId);
                    cloudStorage.SetFloat($"{CharacterPosKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_x", startPosition.x);
                    cloudStorage.SetFloat($"{CharacterPosKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_Y", startPosition.y);
                    cloudStorage.SetFloat($"{CharacterPosKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_Z", startPosition.z);

                    var startScale =
                        mapCharactersManager.GetCharacterStartScaleAtMap(character, mapMetaData.mapId);
                    cloudStorage.SetFloat($"{CharacterScaleKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_x", startScale.x);
                    cloudStorage.SetFloat($"{CharacterScaleKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_Y", startScale.y);
                    cloudStorage.SetFloat($"{CharacterScaleKey}_{mapMetaData.mapId}_{character.ToString().ToLower()}_Z", startScale.z);
                }
            }
        }

        
        public void SpreadData(ICloudDataStorage cloudStorage)
        {
            var characters = (CharacterName[]) Enum.GetValues(typeof(CharacterName));
            SpreadOldSingleMapBasedData(cloudStorage, characters);
            SpreadMultiMapBasedData(cloudStorage, characters);
        }


        private void SpreadOldSingleMapBasedData(ICloudDataStorage cloudStorage, CharacterName[] characters)
        {
            var defaultPosition = new Vector3(0, -500, 0);
            var defaultScale = Vector3.one;
            var firstMapId = Base.gameManager.mapManager.DefaultMapId;
            var charactersManager = Base.gameManager.mapCharactersManager;
            
            foreach (var character in characters)
            {
                var startPosition = GetStartPositionFromCloudStorage(cloudStorage,
                    $"{CharacterPosKey}_{character.ToString().ToLower()}", defaultPosition);

                if (startPosition != defaultPosition)
                    charactersManager.SetCharacterStartPositionAtMap(character, startPosition, firstMapId);

                var startScale = GetStartScaleFromCloudStorage(cloudStorage,
                    $"{CharacterScaleKey}_{character.ToString().ToLower()}", defaultScale);

                if (startScale != defaultScale)
                    charactersManager.SetCharacterStartScaleAtMap(character, startScale, firstMapId);
            }
        }

        private void SpreadMultiMapBasedData(ICloudDataStorage cloudStorage, CharacterName[] characters)
        {
            var defaultPosition = new Vector3(0, -500, 0);
            var defaultScale = Vector3.one;
            var mapsMetaData = Base.gameManager.mapManager.MapsMetaDatabase.MapsMetadata;
            var charactersManager = Base.gameManager.mapCharactersManager;
            
            foreach (var mapMetaData in mapsMetaData)
            {
                var mapId = mapMetaData.mapId;
                foreach (var characterName in characters)
                {
                    var startPosition = GetStartPositionFromCloudStorage(cloudStorage,
                        $"{CharacterPosKey}_{mapId}_{characterName.ToString().ToLower()}", defaultPosition);

                    if(startPosition != defaultPosition)
                        charactersManager.SetCharacterStartPositionAtMap(characterName, startPosition, mapId);

                    var startScale = GetStartScaleFromCloudStorage(cloudStorage,
                        $"{CharacterScaleKey}_{mapId}_{characterName.ToString().ToLower()}", defaultScale);

                    if(startScale != defaultScale)
                        charactersManager.SetCharacterStartScaleAtMap(characterName, startScale, mapId);
                }
            }
        }
        
        private Vector3 GetStartPositionFromCloudStorage(ICloudDataStorage cloudStorage, string key, Vector3 defaultPosition)
        {
            return new Vector3(
                cloudStorage.GetFloat($"{key}_x", defaultPosition.x),
                cloudStorage.GetFloat($"{key}_Y", defaultPosition.y),
                cloudStorage.GetFloat($"{key}_Z", defaultPosition.z));
        }

        private Vector3 GetStartScaleFromCloudStorage(ICloudDataStorage cloudStorage, string key, Vector3 defaultScale)
        {
            return new Vector3(
                cloudStorage.GetFloat($"{key}_x", defaultScale.x),
                cloudStorage.GetFloat($"{key}_Y", defaultScale.y),
                cloudStorage.GetFloat($"{key}_Z", defaultScale.z));
        }
        
    }

}


