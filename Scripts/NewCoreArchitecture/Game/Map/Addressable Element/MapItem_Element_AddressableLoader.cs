using System;
using System.Collections;
using Match3.Utility.GolmoradLogging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



namespace Match3.Game.Map
{
    public class MapItem_Element_AddressableLoader : MapItem_Element
    {

        public struct ShowElementStateSettings
        {
            public int stateIndex;
            public bool withAnimation;
        }

        public GameObject loaderPresentationObject;
        
        
        private Spine.Unity.SkeletonGraphic skeletonGraphic = null;
        private Action onAnimationEnd;
        
        private AddressableMapItemElementData elementData;
        private ShowElementStateSettings showElementStateSettings;
        private MapItem_Element loadedMapItemElement;
        private bool isLoading = false;
        private bool loadingWithAnimation = false;
        private bool isAppearationAnimationFinished = false;
        private Action onAppearationAnimationFinished;
        private Action onOpeningBoxAnimationFinished;
        


        public MapItem_Element_AddressableLoader Setup(AddressableMapItemElementData addressableElementData)
        {
            skeletonGraphic = loaderPresentationObject.GetComponent<Spine.Unity.SkeletonGraphic>();
            loaderPresentationObject.gameObject.SetActive(false);
            elementData = addressableElementData;
            return this;
        }

        
        public override void ShowElementState(int index, bool withAnimation, Action onFinish)
        {
            if (loadedMapItemElement!=null)
            {
                loadedMapItemElement.ShowElementState(index, withAnimation, onFinish);
            }
            else if (!isLoading)
            {
                isLoading = true;

                IsElementDownloaded(isDownloaded =>
                {
                    if (!isDownloaded)
                    {
                        loaderPresentationObject.gameObject.SetActive(true);
                        loadingWithAnimation = true;
                        PlayAppearAnimation();
                    }
                    
                    showElementStateSettings = new ShowElementStateSettings {stateIndex = index, withAnimation = withAnimation};
                    Addressables.LoadAssetAsync<GameObject>(elementData.elementPrefabReference).Completed += OnElementLoadCompleted;
                    onFinish?.Invoke();
                }, () =>
                {
                    DebugPro.LogError<MapLogTag>(
                        $"Couldn't check if asset downloaded! Asset Runtime Key: {elementData.elementPrefabReference.RuntimeKey}");
                });
            }
        }


        private void IsElementDownloaded(Action<bool> isDownloaded, Action onFailed)
        {
            var asyncOperation = Addressables.GetDownloadSizeAsync(elementData.elementPrefabReference);
            asyncOperation.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                    isDownloaded(handle.Result == 0);
                else
                    onFailed();
            };
        }

        private void OnElementLoadCompleted(AsyncOperationHandle<GameObject> loadingHandler)
        {
            switch (loadingHandler.Status)
            {
                case AsyncOperationStatus.Succeeded:
                    var loadedItemObject = loadingHandler.Result.gameObject;
                    if (!loadingWithAnimation)
                    {
                        InstantiateLoadedElement(loadedItemObject);
                    }
                    else if (isAppearationAnimationFinished)
                        StartCreatingLoadedElementOperation(loadedItemObject);
                    else
                    {
                        onAppearationAnimationFinished += () =>
                        {
                            StartCreatingLoadedElementOperation(loadedItemObject);
                        };
                    }
                    break;
                case AsyncOperationStatus.Failed:
                    DebugPro.LogError<MapLogTag>(
                        $"Failed to load addressable element, Debug name: {loadingHandler.DebugName} \n Exception message: {loadingHandler.OperationException.Message}");
                    break;
            }
        }

        private void StartCreatingLoadedElementOperation(GameObject loadedElement)
        {
            PlayOpeningBoxAnimation(() =>
            {
                touchGraphicGameObject.SetActive(false);
                InstantiateLoadedElement(loadedElement);

                PlayDisappearAnimation(() =>
                {
                    loaderPresentationObject.SetActive(false);
                });
            });
        }
        
        private void InstantiateLoadedElement(GameObject loadedElement)
        {
            var loadedElementObject = Instantiate(loadedElement, transform, false);
            loadedMapItemElement = loadedElementObject.GetComponent<MapItem_Element>();
            loadedMapItemElement.ShowElementState(showElementStateSettings.stateIndex, 
                showElementStateSettings.withAnimation, delegate { });
        }
        

        
        private void PlayAppearAnimation()
        {
            StartCoroutine(PlaySpineAnimation(-1, 0, false, ()=>
            {
                PlayWiggleLoopAnimation();
                isAppearationAnimationFinished = true;
                onAppearationAnimationFinished?.Invoke();
            }));
        }

        private void PlayWiggleLoopAnimation()
        {
            StartCoroutine(PlaySpineAnimation(-1, 2, true, delegate {  }));
        }
        
        private void PlayOpeningBoxAnimation(Action onFinish)
        {
            StartCoroutine(PlaySpineAnimation(-1, 3, false, onFinish));
        }
        
        private void PlayDisappearAnimation(Action onFinish)
        {
            StartCoroutine(PlaySpineAnimation(-1, 4, false, onFinish));
        }
        
        
        
        IEnumerator PlaySpineAnimation(int prevIndex, int index, bool shouldLoopEnding, Action onAnimationEnd)
        {
            this.onAnimationEnd = onAnimationEnd;
            
            Spine.Animation spineAnimation = null;
            if (skeletonGraphic == null)
                skeletonGraphic = loaderPresentationObject.GetComponent<Spine.Unity.SkeletonGraphic>();
            if (skeletonGraphic.SkeletonData == null)
                yield return new WaitForEndOfFrame();
            
            if(prevIndex >= 0)
                spineAnimation = skeletonGraphic.SkeletonData.FindAnimation(prevIndex.ToString() + "_" + index.ToString());
            else
                spineAnimation = skeletonGraphic.SkeletonData.FindAnimation(index.ToString());

            if (spineAnimation != null)
            {
                skeletonGraphic.AnimationState.ClearTracks();
                skeletonGraphic.Skeleton.SetToSetupPose();
                skeletonGraphic.AnimationState.SetAnimation(0, spineAnimation.name, shouldLoopEnding);
                skeletonGraphic.AnimationState.Complete += AnimationState_Complete;
            }
        }
        
        private void AnimationState_Complete(Spine.TrackEntry trackEntry)
        {
            skeletonGraphic.AnimationState.Complete -= AnimationState_Complete;
            onAnimationEnd.Invoke();
        }
        
        private void OnDestroy()
        {
            Addressables.ReleaseInstance(loadedMapItemElement.gameObject);
        }

    }
}