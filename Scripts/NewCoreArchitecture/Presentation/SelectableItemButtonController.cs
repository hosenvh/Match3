using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


namespace Match3
{

    public class SelectableItemButtonController : MonoBehaviour
    {
        public GameObject lockObject, highlightObject;
        public Button itemButton;
        public Image iconImage;

        [Space(10)] 
        public Sprite loadingItemIcon;

        private int index = -1;
        private UnityAction<int> onClickCallBack;
        private AssetReference addressableIconReference;
        
        
        private void Start()
        {
            itemButton.onClick.AddListener(Click);
        }

        public void Init(int index, UnityAction<int> onClickCallBack)
        {
            this.index = index;
            this.onClickCallBack = onClickCallBack;
        }

        public void SetIcon(Sprite spr)
        {
            iconImage.sprite = spr;
        }
        
        public void SetLock(bool isLock)
        {
            lockObject.SetActive(isLock);
        }

        public void SetSelect(bool selected)
        {
            highlightObject.SetActive(selected);
        }

        public void LoadIcon(AssetReference iconReference)
        {
            addressableIconReference = iconReference;
            //TODO: Activating loading animation object
            SetIcon(loadingItemIcon);
            
            iconReference.LoadAssetAsync<Sprite>().Completed += loadingHandler =>
            {
                switch (loadingHandler.Status)
                {
                    case AsyncOperationStatus.Succeeded:
                        //TODO: Deactivating loading animation object
                        SetIcon(loadingHandler.Result);
                        break;
                    case AsyncOperationStatus.Failed:
                        Debug.LogError(
                            $"Failed to load icon, Debug name: {loadingHandler.DebugName} \n Exception message: {loadingHandler.OperationException.Message}");
                        break;
                }
            };
        }
        
        public void Click()
        {
            onClickCallBack?.Invoke(index);
        }

        private void OnDestroy()
        {
            addressableIconReference?.ReleaseAsset();
        }
    }

}