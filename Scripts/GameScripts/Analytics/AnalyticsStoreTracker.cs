using UnityEngine;
using Match3.Foundation.Base;
using System;

namespace Match3.Overlay.Analytics
{
    public class AnalyticsStoreTracker
    {
        const string STORE_NAME_KEY = "StoreTracker_StoreName";

        readonly StoreFunctionalityManager storeManager;

        public AnalyticsStoreTracker(StoreFunctionalityManager storeFunctionalityManager)
        {
            this.storeManager = storeFunctionalityManager;
        }

        public void TrackStore(Action<string> onStoreIsChangedAction)
        {
            if (StoreIsChanged(storeManager.StoreName()))
                onStoreIsChangedAction.Invoke(SavedStoreName());
            
            SaveStoreName(storeManager.StoreName());
        }

        private bool StoreIsChanged(string currentStore)
        {
            var savedStore = SavedStoreName();

            return savedStore.IsNullOrEmpty() == false
                && savedStore.Equals(currentStore) == false;
        }

        private string SavedStoreName()
        {
            return PlayerPrefs.GetString(STORE_NAME_KEY, "");
        }

        private void SaveStoreName(string storeName)
        {
            PlayerPrefs.SetString(STORE_NAME_KEY, storeName);
        }
    }
}
