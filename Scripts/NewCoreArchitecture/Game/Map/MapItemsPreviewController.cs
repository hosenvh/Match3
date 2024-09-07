using System;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.TimeManagement;
using Match3.Presentation.Map;
using Match3.Presentation.TransitionEffects;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;


namespace Match3.Game.Map
{

    [Serializable]
    public class AddressableUserSelectMapItemPreviewData
    {
        public int itemId;
        public AddressableMapItemElementData addressableItemElementData;
    }

    [Serializable]
    public class PreviewAddressableStateMapItemData
    {
        public int itemId;
        public int stateIndex;
        public AddressableMapItemElementData addressableItemElementData;
    }


    [Serializable]
    public class MapItemsPreviewRequest
    {
        public bool previewAfterCameraMove;
        public string mapId;
        public List<AddressableUserSelectMapItemPreviewData> addressableSingleUserSelectsPreviewData;
        public List<PreviewAddressableStateMapItemData> addressableSingleStatePreviewData;
        public float cameraZoom;
        public Vector2 cameraPosition;
    }
    
    public class MapItemsPreviewController
    {

        private MapManager mapManager;
        private MapItemManager mapItemManager;
        private global::Game gameManager;

        private List<MapItem_UserSelect> hiddenUserSelectMapItems = new List<MapItem_UserSelect>();
        private List<MapItem_State> hiddenStateMapItems = new List<MapItem_State>();
        private List<MapItem_Element> mapItemElementsInPreview = new List<MapItem_Element>();

        private string playerSelectedMapId;


        public MapItemsPreviewController(MapManager mapManager, MapItemManager mapItemManager)
        {
            this.mapManager = mapManager;
            this.mapItemManager = mapItemManager;
            gameManager = Base.gameManager;
        }

        public void PreviewMapItems(MapItemsPreviewRequest previewRequest, Popup_MapItemPreview previewPopup, Action onFinished)
        {
            ClearCache();
            playerSelectedMapId = mapManager.CurrentMap.MapId;
            
            gameManager.SetActiveAllPopups(false);
            previewPopup.OnCloseEvent += () => OnClosePreviewPopup(onFinished);

            if (mapManager.CurrentMapId != previewRequest.mapId)
            {
                ChangeMap(previewRequest.mapId, () => PresentPreviewMapItems(previewRequest, previewPopup));
            }
            else
            {
                PresentPreviewMapItems(previewRequest, previewPopup);
            }
        }

        private void OnClosePreviewPopup(Action onFinished)
        {
            if (playerSelectedMapId != mapManager.CurrentMapId)
            {
                ChangeMap(playerSelectedMapId, () => { });
            }
            else
            {
                FinishPreview();
            }
            SetPlayerCanControlCamera(true);
            gameManager.SetActiveAllPopups(true);
            onFinished.Invoke();
        }

        private void ChangeMap(string mapId, Action onFinished)
        {
            ServiceLocator.Find<GameTransitionManager>().GoToMap<CloudTransitionEffect>(mapId, true, () =>
            {
                onFinished();
            });
        }

        private void PresentPreviewMapItems(MapItemsPreviewRequest previewRequest, Popup_MapItemPreview previewPopup)
        {
            SetPlayerCanControlCamera(false);
            if (!previewRequest.previewAfterCameraMove)
            {
                PreviewAddressableUserSelectMapElements(previewRequest.addressableSingleUserSelectsPreviewData);
                PreviewAddressableStateMapElements(previewRequest.addressableSingleStatePreviewData);
            }
                
            SetCameraFocus(previewRequest.cameraPosition, previewRequest.cameraZoom, () =>
            {
                if (previewRequest.previewAfterCameraMove)
                {
                    PreviewAddressableUserSelectMapElements(previewRequest.addressableSingleUserSelectsPreviewData);
                    PreviewAddressableStateMapElements(previewRequest.addressableSingleStatePreviewData);    
                }
                    
                ServiceLocator.Find<UnityTimeScheduler>().Schedule(1, () => previewPopup.gameObject.SetActive(true), this); 
            });
        }

        private void PreviewAddressableUserSelectMapElements(List<AddressableUserSelectMapItemPreviewData> addressablePreviewDatas)
        {
            foreach (var singleAddressableUserSelectPreviewData in addressablePreviewDatas)
            {
                var userSelectMapItem = mapItemManager.GetUserSelectItemFromCurrentMap(singleAddressableUserSelectPreviewData.itemId);
                hiddenUserSelectMapItems.Add(userSelectMapItem);
                
                userSelectMapItem.Hide();
                
                var previewMapItemElement = mapItemManager.CreateAddressableElement(userSelectMapItem.transform, singleAddressableUserSelectPreviewData.addressableItemElementData);
                mapItemElementsInPreview.Add(previewMapItemElement);
                
                previewMapItemElement.ShowElementState(1, true, delegate {  });
            }
        }

        private void PreviewAddressableStateMapElements(List<PreviewAddressableStateMapItemData> addressablePreviewDatas)
        {
            // TODO:
        }

        private void FinishPreview()
        {
            foreach (var stateMapItem in hiddenStateMapItems)
            {
                stateMapItem.UnHide();
            }

            foreach (var userSelectMapItem in hiddenUserSelectMapItems)
            {
                userSelectMapItem.UnHide();
            }

            foreach (var previewMapItemElement in mapItemElementsInPreview)
            {
                Object.Destroy(previewMapItemElement.gameObject);
            }
        }

        private void ClearCache()
        {
            hiddenUserSelectMapItems.Clear();
            hiddenStateMapItems.Clear();
            mapItemElementsInPreview.Clear();
        }

        private void SetPlayerCanControlCamera(bool canControl)
        {
            mapManager.CurrentMap.mapCameraController.canUserControlCamera = canControl;
        }

        private void SetCameraFocus(Vector2 targetPosition, float zoomValue, Action onFocusFinished)
        {
            mapManager.CurrentMap.mapCameraController.Focus(targetPosition, zoomValue, onFocusFinished);
        }
        
    }    
}


