using System;
using Medrick.LiveOps.AddressableAssets;
using SeganX;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Popup_AddressableWrapper : GameState
{
    public void Setup<T>(AssetReference reference, Action<T> onComplete, Action onError) where T : GameState
    {
        Addressables.LoadAssetAsync<GameObject>(reference).Completed += (operationHandle) =>
        {
            var loadedObject = operationHandle.Result;
            Close();

            if (loadedObject == null)
            {
                Debug.LogError("[AddressableWrapper] Could not find " + typeof(T).Name + " with assRef: " + reference);
                onError.Invoke();
            }
            else
            {
                var openedPopup = gameManager.OpenPopup<T>(loadedObject);
                openedPopup.gameObject.AddComponent<AddressableMemoryCleaner>();
                onComplete.Invoke(openedPopup.GetComponent<T>());
            }
        };
    }

    public override void Back()
    {
    }
}