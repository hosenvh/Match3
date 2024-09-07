using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScenarioLevelMappingWindow : EditorWindow
{
    private Rect levelSection;
    private Rect scenarioSection;
    private Texture2D levelTexture;
    private Texture2D scenarioTexture;
    private TaskConfig[] taskConfigsArray = null;
    private List<TaskConfig> sortedConfigList = new List<TaskConfig>();
    private int scenarioInput, levelInput;
    private int maxTask;
    private TaskConfig lastTask;
    private List<int> levels = new List<int>();
    
    [MenuItem("Golmorad/Utility/Scenario Level Mapping")]
    private static void OpenWindow()
    {
        var window = OpenWindow();
        SetupWindow();

        ScenarioLevelMappingWindow OpenWindow() => GetWindow<ScenarioLevelMappingWindow>(title: "Find Scenario/Level");

        void SetupWindow()
        {
            window.maxSize = new Vector2(280, 600);
            window.minSize = window.maxSize;
            window.position = new Rect(new Vector2(600, 300), window.maxSize);
        }
    }

    private void OnEnable()
    {
        GetAllTasks();
      
        void GetAllTasks()
        {
            taskConfigsArray = Resources.LoadAll<TaskConfig>("Tasks");
            sortedConfigList = taskConfigsArray.ToList();
            sortedConfigList.Sort((x, y) => x.id.CompareTo(y.id));
        }
    }
  
    private void OnGUI() 
    {
        DrawLayouts();
        DrawLevelFinder();
        DrawScenarioFinder();
    }
  
    private void DrawLayouts()
    {
        levelSection.x = 0;
        levelSection.y = 0;
        levelSection.width = Screen.width;
        levelSection.height = Screen.height / 2f;

        scenarioSection.x = 0;
        scenarioSection.y = Screen.height/2f;
        scenarioSection.width = Screen.width;
        scenarioSection.height = Screen.height / 2f;
    }

    private void DrawScenarioFinder()
    {
        GUILayout.BeginArea(scenarioSection);
        GUILayout.Label("Find Scenario");
        GUILayout.BeginHorizontal();
        GUILayout.Label("LEVEL");
        
        DrawLevelInput();
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Find Scenario"))
            FindTheTask();
      
        GUILayout.BeginHorizontal();
        GUILayout.Label("TASK");
        
        lastTask = EditorGUILayout.ObjectField("", lastTask, typeof(TaskConfig), false) as TaskConfig;
        
        GUILayout.EndHorizontal();      
        GUILayout.EndArea();
    }
  
    private void DrawLevelFinder()
    {
        GUILayout.BeginArea(levelSection);
        GUILayout.Label("Find Level");
        GUILayout.BeginHorizontal();     
        GUILayout.Label("SCENARIO");
      
        DrawScenarioInput();
        GUILayout.EndHorizontal();
      
        if (GUILayout.Button("Find level"))
        {
            levels.Clear();
            levels= GetLevels();
        }
      
        foreach (var level in levels)
            GUILayout.Label(level.ToString());
      
        GUILayout.EndArea();
    }
  
    private void DrawScenarioInput()
    {
        scenarioInput = EditorGUILayout.IntField(scenarioInput);
    }
  
    private void DrawLevelInput()
    {
        levelInput = EditorGUILayout.IntField(levelInput);
    }
  
    private void FindTheTask()
    {
        var level = levelInput;

        foreach (var Task in sortedConfigList)
        {
            if (Task.id > 1)
                level -= Task.requiremnetStars;

            lastTask = Task;
            if (level  <= 0)
                break;
        }
    }
  
    private List<int> GetLevels()
    {
        List<int> result = new List<int>();
        var level = 0;
        var range = 0;
        
        foreach (var taskConfig in sortedConfigList)
        {
            level += taskConfig.requiremnetStars;
            if (taskConfig.id == scenarioInput)
            {
                range = taskConfig.requiremnetStars;
                for (int i = 1; i <= range; i++)
                    result.Add(level - i); 
                    
                break;
            }
        }

        return result;
    }
}
