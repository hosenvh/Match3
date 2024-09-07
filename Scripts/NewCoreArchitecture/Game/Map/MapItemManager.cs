using System;
using System.Collections.Generic;
using Match3.Data.Map;
using Match3.Foundation.Unity;
using Match3.Utility;
using Match3.Utility.GolmoradLogging;
using UnityEngine;


namespace Match3.Game.Map
{
    public class MapItemManager : Base
    {
        [SerializeField] private MapsItemsDatabase mapsItemsDatabase;
        [SerializeField] private ResourceMapItemElementAsset addressableItemLoader;
        
        public List<MapItem_State> CurrentMapStateItems { get; } = new List<MapItem_State>(256);
        public List<MapItem_UserSelect> CurrentMapUserSelectItems { get; } = new List<MapItem_UserSelect>(256);

        public MapsItemsDatabase MapsItemsDatabase => mapsItemsDatabase;
        public MapItemsPreviewController MapItemsPreviewController { private set; get; }
        

        private bool IsOldSaveDataConverted
        {
            get => PlayerPrefsEx.GetBoolean("MapItemCenterOldSaveDataConversion", false);
            set => PlayerPrefsEx.SetBoolean("MapItemCenterOldSaveDataConversion", value);
        }

        
        private AddressableMapItemElementsController addressableMapItemElementsController;


        private void Awake()
        {
            MapItemsPreviewController = new MapItemsPreviewController(gameManager.mapManager, this);
            addressableMapItemElementsController = new AddressableMapItemElementsController(addressableItemLoader);
        }

        public void InitializeCurrentMapItems()
        {
            if (!IsOldSaveDataConverted)
            {
                ConvertAndDeleteSingleMapSaveDataToMultiMap();
                IsOldSaveDataConverted = true;
            }

            foreach (var mapStateItem in CurrentMapStateItems)
            {
                mapStateItem.Init();
            }

            foreach (var mapUserSelectItem in CurrentMapUserSelectItems)
            {
                mapUserSelectItem.Init();
            }
        }

        public MapItem_Element CreateAddressableElement(Transform parent, AddressableMapItemElementData addressableMapItemElementData)
        {
            return addressableMapItemElementsController.CreateAddressableMapItemWithLoader(parent,
                addressableMapItemElementData);
        }

        public int GetUserSelectItemSelectedIndex(MapItem_UserSelect mapItemUserSelect)
        {
            return GetUserSelectItemSelectedIndex(mapItemUserSelect.GetItemId(), GetCurrentMapId());
        }

        public int GetUserSelectItemSelectedIndex(MapItemUserSelectMetadata userSelectMetadata, string mapId)
        {
            return GetUserSelectItemSelectedIndex(userSelectMetadata.itemId, mapId);
        }

        public int GetUserSelectItemSelectedIndex(int itemId, string mapId)
        {
            return PlayerPrefs.GetInt(UserSelectItem_SelectedIndexDataKey(itemId, mapId), -1);
        }

        public void SetUserSelectItemSelectedIndex(MapItem_UserSelect mapItemUserSelect, int selectedIndex)
        {
#if UNITY_EDITOR
            if (gameManager.taskManager.CanSave)
#endif
                PlayerPrefs.SetInt(
                    UserSelectItem_SelectedIndexDataKey(mapItemUserSelect.GetItemId(), GetCurrentMapId()),
                    selectedIndex);
        }

        // This way of saving doesn't save related items and their index should be -1 until player change their parents
        public void SaveUserSelectItemSelectedIndex(int itemId, string mapId, int selectedIndex)
        {
            PlayerPrefs.SetInt(UserSelectItem_SelectedIndexDataKey(itemId, mapId), selectedIndex);
        }

        private string UserSelectItem_SelectedIndexDataKey(int itemId, string mapId)
        {
            return $"mapItemSelectedIndex_{mapId}_{itemId.ToString()}";
        }


        public int GetStateItemStateIndex(MapItem_State stateItem, int initIndex)
        {
            return PlayerPrefs.GetInt(StateItem_StateIndexDataKey(stateItem.GetItemId(), GetCurrentMapId()), initIndex);
        }

        public int GetStateItemStateIndex(MapItemStateMetaData stateItem, string mapId)
        {
            return PlayerPrefs.GetInt(StateItem_StateIndexDataKey(stateItem.itemId, mapId), stateItem.initIndex);
        }

        public int GetStateItemStateIndex(int itemId, string mapId, int initIndex)
        {
            return PlayerPrefs.GetInt(StateItem_StateIndexDataKey(itemId, mapId), initIndex);
        }

        public void SetStateItemStateIndex(MapItem_State stateItem, int stateIndex)
        {
#if UNITY_EDITOR
            if (gameManager.taskManager.CanSave)
#endif
                PlayerPrefs.SetInt(StateItem_StateIndexDataKey(stateItem.GetItemId(), GetCurrentMapId()), stateIndex);
        }

        // This way of saving doesn't save related items and their index should be -1 until player change their parents
        public void SaveStateItemStateIndex(int itemId, string mapId, int stateIndex)
        {
            PlayerPrefs.SetInt(StateItem_StateIndexDataKey(itemId, mapId), stateIndex);
        }

        private string StateItem_StateIndexDataKey(int itemId, string mapId)
        {
            return $"mapItemStateIndex_{mapId}_{itemId.ToString()}";
        }


        public void RegisterMapItem_UserSelect(MapItem_UserSelect mapItem_UserSelect)
        {
#if UNITY_EDITOR
            if (CurrentMapUserSelectItems.Find(item => item.GetItemId() == mapItem_UserSelect.GetItemId()))
                DebugPro.LogError<MapLogTag>("Please change the MapItem_UserSelect: " + mapItem_UserSelect.GetItemId().ToString());
#endif
            CurrentMapUserSelectItems.Add(mapItem_UserSelect);
        }

        public void RegisterMapItem_State(MapItem_State mapItem_State)
        {
#if UNITY_EDITOR
            if (CurrentMapStateItems.Find(item => item.GetItemId() == mapItem_State.GetItemId()))
                DebugPro.LogError<MapLogTag>("Please change the MapItemStateIndex: " + mapItem_State.GetItemId().ToString() + " _ " +
                               mapItem_State.gameObject.name);
#endif
            CurrentMapStateItems.Add(mapItem_State);
        }


        public MapItem_UserSelect GetUserSelectItemFromCurrentMap(int itemId)
        {
            foreach (var item in CurrentMapUserSelectItems)
                if (item.GetItemId() == itemId)
                    return item;

            return null;
        }

        public MapItem_State GetStateItemFromCurrentMap(int itemId)
        {
            foreach (var item in CurrentMapStateItems)
                if (item.GetItemId() == itemId)
                    return item;

            return null;
        }


        public void ClearCurrentMapItems()
        {
            CurrentMapStateItems.Clear();
            CurrentMapUserSelectItems.Clear();
        }


        public void ResetMapItems()
        {
            var allMapsItemsDatas = mapsItemsDatabase.MapsItemsData;
            foreach (var mapItemsData in allMapsItemsDatas)
            {
                foreach (var userSelect in mapItemsData.userSelectItems)
                {
                    PlayerPrefs.DeleteKey(UserSelectItem_SelectedIndexDataKey(userSelect.itemId, mapItemsData.mapId));
                }

                foreach (var stateItem in mapItemsData.stateItems)
                {
                    PlayerPrefs.DeleteKey(StateItem_StateIndexDataKey(stateItem.itemId, mapItemsData.mapId));
                }
            }
        }



        public void RegisterUserSelectAddressableElement(AddressableMapItemElementData elementData)
        {
            addressableMapItemElementsController.RegisterUserSelectAddressableElement(elementData);
        }

        public bool HasAddressableElement(MapItem_UserSelect_Single userSelectSingle)
        {
            return addressableMapItemElementsController.HasUserSelectAddressableElement(gameManager.mapManager.CurrentMapId,
                userSelectSingle.GetItemId());
        }

        public List<AddressableMapItemElementData> GetAddressableElements(MapItem_UserSelect_Single userSelectSingle)
        {
            return addressableMapItemElementsController.GetUserSelectAddressableElements(
                gameManager.mapManager.CurrentMapId,
                userSelectSingle.GetItemId());
        }
        
        

        private void ConvertAndDeleteSingleMapSaveDataToMultiMap()
        {
            foreach (var mapItemsData in mapsItemsDatabase.MapsItemsData)
            {
                if (!IsThisFirstGameMap(mapItemsData.mapId)) continue;

                foreach (var userSelectItem in mapItemsData.userSelectItems)
                {
                    var oldSelectedIndexDataKey = OldUserSelectItem_SelectedIndexDataKey(userSelectItem.itemId);
                    if (PlayerPrefs.HasKey(oldSelectedIndexDataKey))
                    {
                        var selectedIndex = PlayerPrefs.GetInt(oldSelectedIndexDataKey, 0);
                        SaveUserSelectItemSelectedIndex(userSelectItem.itemId, mapItemsData.mapId, selectedIndex);
                        PlayerPrefs.DeleteKey(oldSelectedIndexDataKey);
                    }
                }

                foreach (var stateItem in mapItemsData.stateItems)
                {
                    var oldStateIndexDataKey = OldStateItem_StateIndexDataKey(stateItem.itemId);
                    if (PlayerPrefs.HasKey(oldStateIndexDataKey))
                    {
                        var stateIndex = PlayerPrefs.GetInt(oldStateIndexDataKey, 0);
                        SaveStateItemStateIndex(stateItem.itemId, mapItemsData.mapId, stateIndex);
                        PlayerPrefs.DeleteKey(oldStateIndexDataKey);
                    }
                }
            }
        }

        private string OldUserSelectItem_SelectedIndexDataKey(int itemId)
        {
            return $"mapItemSelectedIndex_{itemId.ToString()}";
        }

        private string OldStateItem_StateIndexDataKey(int itemId)
        {
            return $"mapItemStateIndex_{itemId.ToString()}";
        }


        private bool IsThisFirstGameMap(string mapId)
        {
            return mapId == gameManager.mapManager.DefaultMapId;
        }

        private string GetCurrentMapId()
        {
            return gameManager.mapManager.CurrentMap.MapId;
        }
    }
}