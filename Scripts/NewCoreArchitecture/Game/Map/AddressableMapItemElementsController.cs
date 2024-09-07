using System;
using System.Collections.Generic;
using Match3.Foundation.Unity;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Match3.Game.Map
{

    public class AddressableMapItemElementsController
    {

        private const string UserSelectAddressableElementsDataKey = "UserSelectAddressableElementsData";
        
        private Dictionary<string, List<AddressableMapItemElementData>> userSelectAddressableElements =
            new Dictionary<string, List<AddressableMapItemElementData>>();

        private ResourceMapItemElementAsset addressableItemLoader;


        public AddressableMapItemElementsController(ResourceMapItemElementAsset addressableLoader)
        {
            addressableItemLoader = addressableLoader;
            LoadUserSelectAddressableElements();
            //TODO: Maybe we should add a download mechanism in case of saved addressable elements data were deleted
        }

        public MapItem_Element CreateAddressableMapItemWithLoader(Transform parent, AddressableMapItemElementData addressableMapItemElementData)
        {
            var loaderElement = Object
                .Instantiate(addressableItemLoader.Load() as MapItem_Element_AddressableLoader, parent, false)
                .Setup(addressableMapItemElementData);
            return loaderElement;
        }
        
        public void RegisterUserSelectAddressableElement(AddressableMapItemElementData elementData)
        {
            InsertIntoUserSelectAddressableElements(elementData);
            SaveUserSelectAddressableElements();
        }

        public bool HasUserSelectAddressableElement(string mapId, int itemId)
        {
            return userSelectAddressableElements.ContainsKey( GetElementGeneralId(mapId, itemId));
        }
        
        public List<AddressableMapItemElementData> GetUserSelectAddressableElements(string mapId, int itemId)
        {
            var itemKey = GetElementGeneralId(mapId, itemId);
            userSelectAddressableElements.TryGetValue(itemKey, out var elements);
            return elements ?? new List<AddressableMapItemElementData>();
        }

        private void SaveUserSelectAddressableElements()
        {
            List<AddressableMapItemElementData> addressableElements = new List<AddressableMapItemElementData>();
            foreach (var userSelectAddressableElementsList in userSelectAddressableElements.Values)
            {
                foreach (var userSelectAddressableElement in userSelectAddressableElementsList)
                {
                    addressableElements.Add(userSelectAddressableElement);
                }
            }

            var serializationWrap = new AddressableMapItemElementsDataSerializationWrap(addressableElements);
            PlayerPrefs.SetString(UserSelectAddressableElementsDataKey, serializationWrap.GetSerializedJson());
        }

        private void LoadUserSelectAddressableElements()
        {
            var serializedAddressableElements = PlayerPrefs.GetString(UserSelectAddressableElementsDataKey, "");
            if (!serializedAddressableElements.IsNullOrEmpty())
            {
                var addressableItemsSerializationWrap =
                    JsonUtility.FromJson<AddressableMapItemElementsDataSerializationWrap>(serializedAddressableElements);
                foreach (var addressableMapItem in addressableItemsSerializationWrap.addressableMapItemsData)
                {
                    InsertIntoUserSelectAddressableElements(addressableMapItem);
                }
            }
        }

        private void InsertIntoUserSelectAddressableElements(AddressableMapItemElementData elementData)
        {
            var elementGeneralId = GetElementGeneralId(elementData);
            
            if (userSelectAddressableElements.ContainsKey(elementGeneralId))
            {
                if(!userSelectAddressableElements[elementGeneralId].Contains(elementData))
                    userSelectAddressableElements[elementGeneralId].Add(elementData);
            }
            else
                userSelectAddressableElements.Add(elementGeneralId, new List<AddressableMapItemElementData>(){elementData});
        }
        
        private string GetElementGeneralId(AddressableMapItemElementData addressableElementData)
        {
            return $"{addressableElementData.mapId}_{addressableElementData.itemId.ToString()}";
        }
        
        private string GetElementGeneralId(string mapId, int itemId)
        {
            return $"{mapId}_{itemId.ToString()}";
        }


        [Serializable]
        public class AddressableMapItemElementsDataSerializationWrap
        {
            public List<AddressableMapItemElementData> addressableMapItemsData;

            public AddressableMapItemElementsDataSerializationWrap(List<AddressableMapItemElementData> addressableMapItemsData)
            {
                this.addressableMapItemsData = addressableMapItemsData;
            }

            public string GetSerializedJson()
            {
                return JsonUtility.ToJson(this);
            }
        }
        
        
    }

}
