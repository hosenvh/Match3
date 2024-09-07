using System;
using System.Collections;
using Match3.Development.DevOptions;
using UnityEngine;


namespace Match3.Development
{
    public class OpenLevelsConsecutivelyTool : MonoBehaviour
    {
        private bool isLoadingStarted;

        public void StartLoading(int startLevel, int finishLevel, float intervalTime)
        {
            isLoadingStarted = true;
            StartCoroutine(LoadLevelsRoutine(startLevel, finishLevel, intervalTime));
        }

        public void StopLoading()
        {
            isLoadingStarted = false;
        }

        private IEnumerator LoadLevelsRoutine(int startLevel, int finishLevel, float intervalTime)
        {
            int levelCount = startLevel;
            var wait = new WaitForSeconds(intervalTime);
            while (levelCount != finishLevel + 1 & isLoadingStarted)
            {
                Debug.Log("----- Level " + levelCount + " -----");
                try
                {
                    GameplayDevOptions.LoadLevel(levelCount);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex + "\n" + ex.Source);
                }
                yield return wait;
                levelCount++;
            }
            Debug.Log("---------- Level Loading Finished ----------");
        }
    }
}