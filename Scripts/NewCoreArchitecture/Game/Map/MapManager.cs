using System;
using Match3.Data.Map;
using Match3.Data.Unity.PersistentTypes;
using Match3.Foundation.Unity;
using Match3.Utility.GolmoradLogging;
using UnityEngine;
using PlayerPrefsEx = Match3.Utility.PlayerPrefsEx;


namespace Match3.Game.Map
{
    
    public class MapManager : MonoBehaviour
    {
        private const string LastLoadedMapIdDataKey = "LastLoadedMapId";
        public event Action OnStartMapChanging;
        public event Action<State_Map> OnMapChanged;

        [MapSelector] [SerializeField] private string defaultMap = default;
        [SerializeField] private MapsMetaDatabase mapsMetaDatabase = default;

        public MapsMetaDatabase MapsMetaDatabase => mapsMetaDatabase;
        public string DefaultMapId => defaultMap;
        public State_Map CurrentMap { private set; get; }
        public string CurrentMapId => CurrentMap.MapId;
        
        private MapItemManager MapItemManager => Base.gameManager.mapItemManager;
        private MapCharactersManager MapCharactersManager => Base.gameManager.mapCharactersManager;

        private PersistentBool IsDefaultMapSetUnlocked;
        

        private void Awake()
        {
            IsDefaultMapSetUnlocked = new PersistentBool("DefaultMapSetUnlocked");
            if (!IsDefaultMapSetUnlocked.Get())
            {
                IsDefaultMapSetUnlocked.Set(true);
                SetMapUnlocked(defaultMap);
            }
        }

        public State_Map OpenMap(string mapId)
        {
            try
            {
                OnStartMapChanging?.Invoke();

                var requestedMapMetadata = FindMapMetaData(mapId);
                CurrentMap = CloseCurrentAndLoadMapState(requestedMapMetadata.mapResourcesPath);
                CurrentMap.OnCloseEvent += ClearCurrentMapRelatedSubSystemsData;
                CurrentMap.OnCloseEvent += ReleaseMapStateReference;

                InitializeSubSystems();
                SaveLastLoadedMapId(mapId);
                SetMapUnlocked(mapId);
                OnMapChanged?.Invoke(CurrentMap);
                return CurrentMap;
            }
            catch(MapMetaDataNotFoundException exception)
            {
                DebugPro.LogException<MapLogTag>(exception);
                return null;
            }
        }

        private void ClearCurrentMapRelatedSubSystemsData()
        {
            MapItemManager.ClearCurrentMapItems();
            MapCharactersManager.ClearCurrentMapCharactersAnimatorWrappers();
        }

        private void InitializeSubSystems()
        {
            MapItemManager.InitializeCurrentMapItems();
            MapCharactersManager.SetupCharactersAnimator();
        }
        
        private MapMetaData FindMapMetaData(string mapId)
        {
            foreach (var mapMetaData in mapsMetaDatabase.MapsMetadata)
            {
                if (mapMetaData.mapId == mapId)
                {
                    return mapMetaData;
                }
            }
            
            throw new MapMetaDataNotFoundException(mapId);
        }

        private State_Map CloseCurrentAndLoadMapState(string mapResourcesPath)
        {
            return Base.gameManager.GoToStateByResourcesPath<State_Map>(mapResourcesPath);
        }

        private void ReleaseMapStateReference()
        {
            CurrentMap = null;
        }


        public State_Map OpenLastLoadedMap()
        {
            var lastLoadedMapId = GetLastLoadedMapId();
            return OpenMap(lastLoadedMapId);
        }

        public string GetLastLoadedMapId()
        {
            return PlayerPrefs.GetString(LastLoadedMapIdDataKey, defaultMap);
        }

        private void SaveLastLoadedMapId(string mapId)
        {
            PlayerPrefs.SetString(LastLoadedMapIdDataKey, mapId);
        }

        public bool IsMapUnlocked(string mapId)
        {
            return PlayerPrefsEx.GetBoolean(GetMapUnlockDataKey(mapId), false);
        }

        public void SetMapUnlocked(string mapId)
        {
            PlayerPrefsEx.SetBoolean(GetMapUnlockDataKey(mapId), true);
        }
        
        private string GetMapUnlockDataKey(string mapId)
        {
            return $"MapUnlockStatus_{mapId}";
        }
        
        
        
        public bool IsInMap()
        {
            return CurrentMap != null;
        }
        
        
        
        public void RestoreLastLoadedMapId(string mapId)
        {
            SaveLastLoadedMapId(mapId);
        }
    }

    public sealed class MapMetaDataNotFoundException : Exception
    {
        public MapMetaDataNotFoundException(string requestedId)
        {
            Message.Insert(0, $"Requested '{requestedId}' map ID isn't at MapsMetaDatabase");
        }
    }
    
}


