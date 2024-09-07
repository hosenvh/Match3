using System;
using I2.Loc;
using Match3.Foundation.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "Task")]
public class TaskConfig : ScriptableObject
{
    
    public int id;
    public ResourceSpriteAsset iconSpriteResource;
    public string taskString;
    [HideInInspector] public LocalizedStringTerm taskLocalizedString = "";
    public int requiremnetStars;
    public ScenarioConfig preScenarioConfig, postScenarioConfig;
    public TaskConfig[] requirementTasks;
    
    
    #if UNITY_EDITOR

    private LanguageSourceAsset languageSource;
    private TermData myTermData = null;
    
    private void OnValidate()
    {
        if (Selection.activeObject != this) return;
        if (string.IsNullOrEmpty(this.name)) return;
        
        if(languageSource == null) languageSource = Resources.Load<LanguageSourceAsset>(LocalizationManager.TaskLanguageSource);

        myTermData = languageSource.mSource.GetTermData("Task/" + this.name);

        // Note: After renaming task name, term won't update until an update occur via inspector
        if (myTermData == null)
        {
            if(!string.IsNullOrEmpty(taskLocalizedString.mTerm))
                languageSource.mSource.RemoveTerm(taskLocalizedString.mTerm);
            
            myTermData = languageSource.mSource.AddTerm("Task/" + this.name, eTermType.Text, false);
            myTermData.SetTranslation(GetFarsiLanguageIndex(), taskString);
            taskLocalizedString = myTermData.Term;
        }

        if (myTermData == null) return;
        
        myTermData.SetTranslation(GetFarsiLanguageIndex(), taskString);
        taskLocalizedString = myTermData.Term;
        EditorUtility.SetDirty (languageSource);
        EditorUtility.SetDirty (this);
    }

    private int GetFarsiLanguageIndex()
    {
        return languageSource.mSource.GetLanguageIndex("Persian", true, false);
    } 

#endif
    
    public Sprite TaskIcon()
    {
        return iconSpriteResource.Load();
    }

    public string GetLocalizedTaskName()
    {
        return taskLocalizedString;
    }
    
}