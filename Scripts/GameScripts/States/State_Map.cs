using Match3.Foundation.Base.EventManagement;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;
using SeganX;
using System;
using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.TaskManagement;
using Match3.Game;
using Match3.Game.Effects;
using Match3.Game.TaskManagement;



public class MapEnteredEvent : GameEvent
{
    public readonly bool FirstEntering = false;

    public MapEnteredEvent(bool isFirst)
    {
        FirstEntering = isFirst;
    }
}


public class State_Map : GameState
{
    private static bool firstTimeEnteredMap = true;

    [SerializeField] private string mapId = default;
    [SerializeField] private List<MonoConditionalTask> awakeMapTasks = new List<MonoConditionalTask>();
    
    [Space(20)]
    public MapStateScenarioPlayer mapStateScenarioPlayer;
    public MapCameraController mapCameraController = null;
    public AudioSource mapAudioSource;
    public ScenarioScreen scenarioScreen;
    public Canvas Canvas_Map;
    public Spine.Unity.SkeletonGraphic pointerSkeletonGraphic = null;
    public MapItemGloryShineController mapItemGloryShineController;
    public MapItemBounceController MapItemBounceController;
    public Animation mapItem_imageAnimatorPrefab, mapItem_meshRendererAnimatorPrefab, mapItem_spriteRendererAnimationPrefab;
    
    public string MapId => mapId;

    #region methods

    
    
    public void Setup(Action onMapStartTasksComplete)
    {
        ExecuteMapTasks(()=>
        {
            MapEnterInitialization();
            onMapStartTasksComplete();
        });
    }

    private void MapEnterInitialization()
    {
        mapStateScenarioPlayer.Setup(gameManager.scenarioManager);
            
        ServiceLocator.Find<EventManager>().Propagate(new MapEnteredEvent(firstTimeEnteredMap), this);
        firstTimeEnteredMap = false;
    }
    
    private void ExecuteMapTasks(Action onComplete)
    {
        if (awakeMapTasks.Count == 0)
        {
            onComplete();
            return;
        }
        
        var tasksExecutor = new SequentialSortedTaskChain();
        foreach (var task in awakeMapTasks)
        {
            tasksExecutor.AddTask(task, task.priority, task.id);
        }
        tasksExecutor.Execute(onComplete,onComplete);
    }
    
    
    
    IEnumerator Start()
    {
        yield return null;
        gameManager.SetLanguageChanging(false);
    }
    

    #endregion
}