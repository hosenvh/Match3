using System;
using System.Collections.Generic;
using Match3;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Game;
using Match3.Game.Map;
using Match3.Presentation.TransitionEffects;
using Match3.QuickForwardScenario.Game;
using UnityEngine;


public class ScenarioManager : Base
{
    #region fields

    ScenarioScreen scenarioScreen = null;
    AudioSource audioSource;

    MapCameraController mapCameraController = null;
    Popup_DialogueBox popup_DialogueBox = null;

    private MapCharactersManager mapCharactersManager;

    private Dictionary<string, List<int>> unlockedUserSelects = new Dictionary<string, List<int>>();
    private Dictionary<string, List<int>> changedStates = new Dictionary<string, List<int>>();

    private QuickForwardScenarioController quickForwardScenarioController;

    #endregion

    #region methods


    public void Setup()
    {
        mapCharactersManager = gameManager.mapCharactersManager;
        gameManager.mapManager.OnMapChanged += UpdateCurrentMapCameraControllerAccess;
        gameManager.mapManager.OnMapChanged += UpdateCurrentMapAudioSourceAccess;
        gameManager.mapManager.OnMapChanged += UpdateCurrentMapScenarioScreenAccess;

        quickForwardScenarioController = new QuickForwardScenarioController();
    }


    private void UpdateCurrentMapCameraControllerAccess(State_Map currentMap)
    {
        mapCameraController = currentMap.mapCameraController;
    }

    private void UpdateCurrentMapAudioSourceAccess(State_Map currentMap)
    {
        audioSource = currentMap.mapAudioSource;
    }

    private void UpdateCurrentMapScenarioScreenAccess(State_Map currentMap)
    {
        scenarioScreen = currentMap.scenarioScreen;
    }
    
    
    
    public void SaveScenarioStates(List<ScenarioItem> scenarioItems)
    {
        unlockedUserSelects.Clear();
        changedStates.Clear();
        
        var lastScenarioMapId = gameManager.mapManager.DefaultMapId;
        
#if UNITY_EDITOR
        if (gameManager.taskManager.CanSave)
#endif
        {
            foreach (var item in scenarioItems)
            {
                switch (item.scenarioType)
                {
                    case ScenarioType.ChangeMap:
                        lastScenarioMapId = item.string_0;
                        break;
                    case ScenarioType.Character:
                        mapCharactersManager.SetCharacterStartPositionAtMap(item.character_0, item.vector3_0, lastScenarioMapId);
                        mapCharactersManager.SetCharacterStartScaleAtMap(item.character_0, item.scale_0, lastScenarioMapId);
                        break;
                    case ScenarioType.MapItem_Selector:
                        gameManager.mapItemManager.SaveUserSelectItemSelectedIndex(item.int_0, lastScenarioMapId,
                            item.int_1);
                        StoreUnlockedMapItemUserSelect(lastScenarioMapId, item.int_0);
                        break;
                    case ScenarioType.MapItem_SetSate:
                        gameManager.mapItemManager.SaveStateItemStateIndex(item.int_0, lastScenarioMapId, item.int_1);
                        StoreChangedMapItemState(lastScenarioMapId, item.int_0);
                        break;
                    case ScenarioType.Camera_Focus:
                        mapCameraController.CameraStartPosition = item.vector2_0;
                        break;
                    case ScenarioType.CharacterMove:
                        mapCharactersManager.SetCharacterStartPositionAtMap(item.character_0,
                            item.vector3s[item.vector3s.Count - 1], lastScenarioMapId);
                        break;
                    case ScenarioType.CharacterScale:
                        mapCharactersManager.SetCharacterStartScaleAtMap(item.character_0,
                            item.scales[item.scales.Count - 1], lastScenarioMapId);
                        break;
                    case ScenarioType.SocialAlbumPost:
                        gameManager.socialAlbumController.UnlockPost(item.string_0);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void StartScenarios(List<ScenarioItem> scenarioItems, System.Action onFinish)
    {
        quickForwardScenarioController.OpenPopup(GetScenarioName());
        mapCameraController.canUserControlCamera = false;
        ShowScenarioItem(scenarioItems, 0, () =>
        {
            mapCameraController.canUserControlCamera = true;
            quickForwardScenarioController.ClosePopup();
            onFinish();
        });

        string GetScenarioName() => scenarioItems[0].scenarioName;
    }

    void ShowScenarioItem(List<ScenarioItem> scenarioItems, int scenarioIndex, System.Action onFinish)
    {
        if (scenarioIndex < scenarioItems.Count)
        {
            quickForwardScenarioController.ResumeQuickForward();
            
            ScenarioItem scenarioItem = scenarioItems[scenarioIndex];
            CharacterAnimator characterAnimator;

            switch (scenarioItem.scenarioType)
            {
                case ScenarioType.None:
                    break;
                case ScenarioType.Dialogue:
                    ShowTaskDialogueBox(scenarioItem, 0, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.Character:
                    characterAnimator = mapCharactersManager.GetCharacter(scenarioItem.character_0);
                    characterAnimator.SetPosition(scenarioItem.vector3_0);
                    characterAnimator.SetRotation(scenarioItem.int_0);
                    characterAnimator.SetScale(scenarioItem.scale_0);
                    if (!scenarioItem.goNext)
                        ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    break;
                case ScenarioType.MapItem_Selector:
                    quickForwardScenarioController.PauseQuickForward();
                    MapItem_UserSelect mapItem_UserSelect = gameManager.mapItemManager.GetUserSelectItemFromCurrentMap(scenarioItem.int_0);

                    AnalyticsManager.SendEvent(new AnalyticsData_Map_SelectObject(scenarioItem.int_0.ToString(), -1, true));

                    mapItem_UserSelect.ShowElement(0, false, null);

                    if (scenarioItem.delay < -1)
                        ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    else
                    {
                        gameManager.tutorialManager.CheckThenShowTutorial(2, 0, null);
                        gameManager.OpenPopup<Popup_MapItemSelector>().Setup(mapItem_UserSelect, false, (selectedIndex) =>
                        {
                            gameManager.mapItemManager.SetUserSelectItemSelectedIndex(mapItem_UserSelect, selectedIndex);
                            mapItem_UserSelect.ShowElement(selectedIndex, false, null);
                            mapItem_UserSelect.Save(selectedIndex);
                        }, (confirm) =>
                        {

                            mapItem_UserSelect.ShowElement(-1, false, null);
                            int elementIndex = gameManager.mapItemManager.GetUserSelectItemSelectedIndex(mapItem_UserSelect);
                            AnalyticsManager.SendEvent(new AnalyticsData_Map_ChangeObject(scenarioItem.int_0.ToString(), elementIndex, true));
#if UNITY_EDITOR
                            if (!gameManager.taskManager.CanSave)
                                elementIndex = 0;
#endif
                            mapItem_UserSelect.ShowElement(elementIndex, true, () =>
                                {
                                    if (!scenarioItem.goNext)
                                        ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                                });
                        });
                    }
                    break;
                case ScenarioType.MapItem_SetSate:
                    quickForwardScenarioController.PauseQuickForward();
                    MapItem_State mapItem_State = gameManager.mapItemManager.GetStateItemFromCurrentMap(scenarioItem.int_0);
                    mapItem_State.ShowState(scenarioItem.int_1, true, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.Camera_Focus:
                    mapCameraController.Focus(scenarioItem.vector2_0, scenarioItem.float_0, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    },  scenarioItem.isInstantly);
                    break;
                case ScenarioType.Screen:
                    var localizedAudioClip = scenarioItem.GetLocalizedAudio0();
                    if (localizedAudioClip != null)
                        audioSource.PlayOneShot(localizedAudioClip);
                    scenarioScreen.Setup(scenarioItem.GetLocalizedString0(), () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.Audio:
                    audioSource.PlayOneShot(scenarioItem.audioClip_0);
                    DelayCall(scenarioItem.audioClip_0.length, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.CharacterMove:
                    AnalyticsManager.SendEvent(new AnalyticsData_Story("move", ""));

                    mapCharactersManager.GetCharacter(scenarioItem.character_0).SetupMove(scenarioItem.vector3s, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.CharacterScale:
                    AnalyticsManager.SendEvent(new AnalyticsData_Story("scale", ""));

                    mapCharactersManager.GetCharacter(scenarioItem.character_0).SetupScale(scenarioItem.scales, scenarioItem.scalesDurations, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.CharacterAnim:
                    mapCharactersManager.GetCharacter(scenarioItem.character_0).BlendAnimation((CharacterAnim)scenarioItem.int_0, scenarioItem.int_1, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.BubbleDialogue:
                    var localizedAudio = scenarioItem.GetLocalizedAudio0();
                    var configuration = ServiceLocator.Find<ServerConfigManager>().data;
                    var shouldPlayAudio = localizedAudio != null && configuration.config.scenarioDialoguesAudioServerConfig.playDialogueAudio;
                    if (shouldPlayAudio)
                        audioSource.PlayOneShot(localizedAudio);
                    mapCharactersManager.GetCharacter(scenarioItem.character_0).ShowBubbleDialogue(scenarioItem.GetLocalizedString0(), scenarioItem.float_0, () =>
                    {
                        if (!scenarioItem.goNext)
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    });
                    break;
                case ScenarioType.ChangeMap:
                    quickForwardScenarioController.PauseQuickForward();
                    if (gameManager.mapManager.CurrentMapId == scenarioItem.string_0)
                        ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    else
                        ChangeMap(scenarioItem, () =>
                        {
                            ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                        });
                    break;
                case ScenarioType.SocialAlbumPost:
                    quickForwardScenarioController.PauseQuickForward();
                    var socialPost = gameManager.socialAlbumController.TryGetPost(scenarioItem.string_0);
                    var socialAlbumPostPicPresenter = gameManager.OpenPopup<Popup_SocialAlbumPicturePresenter>();
                    socialAlbumPostPicPresenter.Setup(socialPost);
                    socialAlbumPostPicPresenter.OnCloseEvent += () => ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
                    break;
                default:
                    break;
            }

            if (scenarioItem.goNext)
                ShowNextScenario(scenarioItems, scenarioIndex, onFinish);
        }
        else
            onFinish();
    }
    

    void ShowNextScenario(List<ScenarioItem> scenarioItems, int scenarioIndex, System.Action onFinish)
    {
        if (scenarioItems[scenarioIndex].delay > 0)
        {
            DelayCall(scenarioItems[scenarioIndex].delay, () =>
            {
                ShowScenarioItem(scenarioItems, scenarioIndex + 1, onFinish);
            });
        }
        else
            ShowScenarioItem(scenarioItems, scenarioIndex + 1, onFinish);
    }

    void ShowTaskDialogueBox(ScenarioItem scenarioItem, int index, System.Action onFinish)
    {
        if (index < scenarioItem.dialogues.Count)
        {
            AnalyticsManager.SendEvent(new AnalyticsData_Story("dialogue", scenarioItem.scenarioName + index));
            if (popup_DialogueBox == null)
                popup_DialogueBox = gameManager.OpenPopup<Popup_DialogueBox>();

            var currentDialogue = scenarioItem.dialogues[index];
            popup_DialogueBox.Setup(currentDialogue, () =>
            {
                //gameManager.ClosePopup();
                ShowTaskDialogueBox(scenarioItem, index + 1, onFinish);
            }, () => {
                index = scenarioItem.dialogues.Count;
                OnFinishDialogue(onFinish);
            });
        }
        else
        {
            OnFinishDialogue(onFinish);
        }
    }

    private void OnFinishDialogue(Action onFinish)
    {
        gameManager.ClosePopup();
        popup_DialogueBox = null;
        onFinish();
    }

    
    private void ChangeMap(ScenarioItem mapChangeScenarioItem, Action onFinished)
    {
        ServiceLocator.Find<GameTransitionManager>().GoToMap<CloudTransitionEffect>(mapChangeScenarioItem.string_0, true, ()=>
        {
            HideChangedMapItemsInMap(mapChangeScenarioItem.string_0);
            onFinished();
        });
    }


    private void StoreUnlockedMapItemUserSelect(string mapId, int itemId)
    {
        if(!unlockedUserSelects.ContainsKey(mapId))
            unlockedUserSelects.Add(mapId, new List<int>());
        unlockedUserSelects[mapId].Add(itemId);
    }

    private void StoreChangedMapItemState(string mapId, int itemId)
    {
        if(!changedStates.ContainsKey(mapId))
            changedStates.Add(mapId, new List<int>());
        changedStates[mapId].Add(itemId);
    }
    
    private void HideChangedMapItemsInMap(string mapId)
    {
        if(unlockedUserSelects.ContainsKey(mapId))
            foreach (var userSelectsIds in unlockedUserSelects[mapId])
            {
                gameManager.mapItemManager.GetUserSelectItemFromCurrentMap(userSelectsIds)
                    .ShowElement(-1, false, delegate { });
            }

        if(changedStates.ContainsKey(mapId))
            foreach (var statesId in changedStates[mapId])
            {
                var stateItem = gameManager.mapItemManager.GetStateItemFromCurrentMap(statesId);
                if(stateItem is MapItem_State_Single stateSingle)
                    stateSingle.ShowState(stateSingle.initIndex, false, delegate {  });
                else if(stateItem is MapItem_State_Group groupState)
                    groupState.ShowState(0, false, delegate {  });
                else if(stateItem is MapItem_State_Related relatedState)
                    relatedState.ShowState(0, false, delegate {  });
            }
    }
    
    #endregion
    
    
    public void ResetUserStateForGetReadyToSetScenario()
    {
        gameManager.taskManager.ResetTasks();
        gameManager.mapItemManager.ResetMapItems();
        mapCharactersManager.ResetCharactersStartPositions();
        mapCharactersManager.ResetCharactersStartScales();
        gameManager.profiler.LastUnlockedLevel = 0;
    }

    public void SkipScenario(int targetScenarioIndex)
    {
        targetScenarioIndex = Mathf.Min(targetScenarioIndex, gameManager.taskManager.LastTaskId());
        string lastUsedMapId = gameManager.mapManager.CurrentMapId;

        for (int i = 0; i < targetScenarioIndex; i++)
        {
            TaskConfig taskConfig = null;
            foreach (var item in gameManager.taskManager.CurrentTasksList)
            {
                taskConfig = item;
                gameManager.profiler.SetStarCount(gameManager.profiler.StarCount - item.requiremnetStars);
                if (item.id > 1)
                    gameManager.profiler.LastUnlockedLevel += item.requiremnetStars;
                break;
            }
            gameManager.taskManager.SetTaskDone(taskConfig);
            List<ScenarioItem> scenarioItems = gameManager.taskManager.GetScenarioItems(taskConfig);
            gameManager.scenarioManager.SaveScenarioStates(scenarioItems);

            lastUsedMapId = ExtractLastChangedToMap(scenarioItems);
        }

        ServiceLocator.Find<GameTransitionManager>().GoToMap<EmptyTransitionEffect>(lastUsedMapId, false);

        string ExtractLastChangedToMap(List<ScenarioItem> scenarioItems)
        {
            string result = gameManager.mapManager.DefaultMapId;
            foreach (ScenarioItem scenarioItem in scenarioItems)
            {
                if (scenarioItem.scenarioType == ScenarioType.ChangeMap)
                    result = scenarioItem.string_0;
            }
            return result;
        }
    }
}