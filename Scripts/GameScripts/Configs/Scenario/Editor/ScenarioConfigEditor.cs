using System.Collections.Generic;
using System.Linq;
using System.Text;
using I2.Loc;
using Match3.CharacterManagement.Character.Base;
using Match3.Data.Map;
using Match3.Data.SocialAlbum;
using UnityEngine;
using UnityEditor;
using Match3.Utility.GolmoradLogging;


[CustomEditor(typeof(ScenarioConfig))]
public class ScenarioConfigEditor : Editor
{
    private class Data
    {
        public SocialAlbumPostsDataBase SocialAlbumPostsDataBase { get; }
        public MapsIDsDatabase MapsIDsDatabase { get; }

        public Data(SocialAlbumPostsDataBase socialAlbumPostsDataBase, MapsIDsDatabase mapsIDsDatabase)
        {
            SocialAlbumPostsDataBase = socialAlbumPostsDataBase;
            MapsIDsDatabase = mapsIDsDatabase;
        }
    }


    private ScenarioConfig myTarget;
    private State_Map state_Map;
    private LanguageSourceAsset languageSource;
    private Data data;
    
    void OnEnable()
    {
        myTarget = (ScenarioConfig)target;
        state_Map = FindObjectOfType<State_Map>();
        languageSource = Resources.Load<LanguageSourceAsset>(LocalizationManager.ScenarioLanguageSource);
        SetupData();
    }

    private void SetupData()
    {
        data = new Data(
            socialAlbumPostsDataBase: AssetEditorUtilities.FindAssetsByType<SocialAlbumPostsDataBase>()[0],
            mapsIDsDatabase: AssetEditorUtilities.FindAssetsByType<MapsIDsDatabase>()[0]);
    }

    private void OnDisable()
    {
        languageSource = null;
        Resources.UnloadUnusedAssets();
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ScenarioType selectedScenarioToAdd = ScenarioType.None;
        int addItemPosition = -1;
        if (myTarget.scenarioItems.Count > 0)
        {
            ScenarioItem removeItem = null, item = null;
            int swapItemIndexA = -1, swapItemIndexB = -1;
            for (int i = 0; i < myTarget.scenarioItems.Count; i++)
            {
                item = myTarget.scenarioItems[i];
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(16)))
                    removeItem = item;
                if (GUILayout.Button("U", GUILayout.Width(20), GUILayout.Height(16)) && i > 0)
                {
                    swapItemIndexA = i;
                    swapItemIndexB = i - 1;
                }
                if (GUILayout.Button("D", GUILayout.Width(20), GUILayout.Height(16)) && i < myTarget.scenarioItems.Count - 1)
                {
                    swapItemIndexA = i;
                    swapItemIndexB = i + 1;
                }
                
                GuiLine(18, GetScenarioColor(item.scenarioType));

                if (Event.current.type == EventType.Repaint)
                {
                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    GUI.Label(new Rect(lastRect.x + 20, lastRect.y, lastRect.width, lastRect.height), item.scenarioType.ToString(), GUIStyle.none);
                }

                GUILayout.EndHorizontal();

                DrawScenarioItemGUI(state_Map, item);

                EditorGUIUtility.labelWidth = 85;
                ScenarioType scenarioType = ScenarioType.None;
                GUILayout.FlexibleSpace();
                scenarioType = (ScenarioType)EditorGUILayout.EnumPopup("Add Scenario:", scenarioType);
                if (scenarioType != ScenarioType.None)
                {
                    selectedScenarioToAdd = scenarioType;
                    addItemPosition = i + 1;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
            }

            if (removeItem != null)
            {
                int removeItemIndex = myTarget.scenarioItems.FindIndex(si => si == removeItem);

                RemoveScenariosTerms(myTarget.scenarioItems, removeItemIndex);
                
                myTarget.scenarioItems.Remove(removeItem);
                
                BuildScenariosTerms(myTarget.scenarioItems, removeItemIndex);
                
                SaveLanguageSourceChanges();
            }
            
            
            if (swapItemIndexA >= 0)
            {
                RemoveScenarioTerms(myTarget.scenarioItems[swapItemIndexA]);
                RemoveScenarioTerms(myTarget.scenarioItems[swapItemIndexB]);
                
                ScenarioItem tempSwapItem = myTarget.scenarioItems[swapItemIndexA];
                myTarget.scenarioItems[swapItemIndexA] = myTarget.scenarioItems[swapItemIndexB];
                myTarget.scenarioItems[swapItemIndexB] = tempSwapItem;
                
                BuildScenarioTerms(myTarget.scenarioItems[swapItemIndexA], swapItemIndexA);
                BuildScenarioTerms(myTarget.scenarioItems[swapItemIndexB], swapItemIndexB);
                
                SaveLanguageSourceChanges();
            }
        }
        else
        {
            ScenarioType scenarioType = ScenarioType.None;
            scenarioType = (ScenarioType)EditorGUILayout.EnumPopup("Add Scenario", scenarioType);
            if (scenarioType != ScenarioType.None)
            {
                selectedScenarioToAdd = scenarioType;
                addItemPosition = 0;
            }
        }

        if (selectedScenarioToAdd != ScenarioType.None)
        {
            RemoveScenariosTerms(myTarget.scenarioItems, addItemPosition);
            
            var scenarioItem = new ScenarioItem(selectedScenarioToAdd);
            myTarget.scenarioItems.Insert(addItemPosition, scenarioItem);

            BuildScenariosTerms(myTarget.scenarioItems, addItemPosition);
            
            SaveLanguageSourceChanges();
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(myTarget);
        EditorApplication.update.Invoke();
    }


    private void BuildScenarioTerms(ScenarioItem scenarioItem, int scenarioIndex)
    {
        switch (scenarioItem.scenarioType)
        {
            case ScenarioType.Dialogue:
                foreach (var dialogue in scenarioItem.dialogues)
                    for (int j = 0; j < scenarioItem.dialogues.Count; ++j)
                    {
                        scenarioItem.dialogues[j].characterLocalizedString = BuildTerm(scenarioItem.scenarioType,
                            dialogue.characterDialogue, scenarioIndex, j).Term;
                    }
                break;
            case ScenarioType.Screen:
            case ScenarioType.BubbleDialogue:
                scenarioItem.localizedString_0 = BuildTerm(scenarioItem.scenarioType,
                    scenarioItem.string_0, scenarioIndex, -1).Term;
                break;
        }
    }
    
    private void RemoveScenarioTerms(ScenarioItem scenarioItem)
    {
        switch (scenarioItem.scenarioType)
        {
            case ScenarioType.Dialogue:
                foreach (var dialogue in scenarioItem.dialogues)
                    RemoveTerm(dialogue.characterLocalizedString.mTerm);
                break;
            case ScenarioType.Screen:
            case ScenarioType.BubbleDialogue:
                RemoveTerm(scenarioItem.localizedString_0.mTerm);
                break;
        }
    }
    
    
    private void BuildScenariosTerms(List<ScenarioItem> scenarioItems, int buildFromIndex)
    {
        for (int i = buildFromIndex; i < scenarioItems.Count; ++i)
        {
            BuildScenarioTerms(scenarioItems[i], i);
        }
    }

    private void RemoveScenariosTerms(List<ScenarioItem> scenarioItems, int fromScenarioIndex)
    {
        for (int i = fromScenarioIndex; i < scenarioItems.Count; ++i)
        {
            RemoveScenarioTerms(scenarioItems[i]);
        }
    }
    
    
    
    private TermData BuildTerm(ScenarioType sType, string farsiTranslation, int scenarioIndex, int dialogueIndex = -1)
    {
        StringBuilder termNameBuilder = new StringBuilder();
        var assetPathParts = AssetDatabase.GetAssetPath(myTarget).Split('/');
        var day = assetPathParts[assetPathParts.Length - 2];

        string typeName = "";
        switch (sType)
        {
            case ScenarioType.Dialogue:
                typeName = "Dialogue";
                break;
            case ScenarioType.Screen:
                typeName = "Screen";
                break;
            case ScenarioType.BubbleDialogue:
                typeName = "DialogBubble";
                break;
        }
        
        termNameBuilder.Append($"Scenario/Day{day}_{typeName}_");
        termNameBuilder.Append(myTarget.name.Replace(" ", "")
                               + "_SI" + scenarioIndex);
        
        if(sType == ScenarioType.Dialogue && dialogueIndex>=0)
            termNameBuilder.Append("_D" + dialogueIndex);
        
        var termData = languageSource.mSource.AddTerm( termNameBuilder.ToString(), 
            eTermType.Text, false);
        termData.SetTranslation(GetFarsiLanguageIndex(), farsiTranslation);

        return termData;
    }
    

    private void RemoveTerm(string term)
    {
        languageSource.mSource.RemoveTerm(term);
        LocalizationEditor.RemoveParsedTerm(term);
        LocalizationEditor.mSelectedKeys.Remove(term);
    }
    
    private void UpdateTranslation(string term, int languageIdx, string translation)
    {
        var termData = languageSource.mSource.GetTermData(term) ??
                       languageSource.mSource.AddTerm(term, eTermType.Text, false);
        
        termData.SetTranslation(languageIdx, translation);
        EditorUtility.SetDirty (languageSource);
    }

    private int GetFarsiLanguageIndex()
    {
        return languageSource.mSource.GetLanguageIndex("Persian", true, false);
    } 

    private void SaveLanguageSourceChanges()
    {
        EditorUtility.SetDirty(languageSource);
        AssetDatabase.SaveAssets();
    }
    
    
    void GuiLine(int height, Color color)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, color);
    }

    public Color GetScenarioColor(ScenarioType scenarioType)
    {
        switch (scenarioType)
        {
            case ScenarioType.Character:
                return new Color(.5f, .5f, .5f);
            case ScenarioType.Dialogue:
                return new Color(.1f, .5f, .5f);
            case ScenarioType.MapItem_Selector:
                return new Color(.5f, .9f, .9f);
            case ScenarioType.MapItem_SetSate:
                return new Color(.9f, .5f, .5f);
            case ScenarioType.Camera_Focus:
                return new Color(.2f, .65f, .2f);
            case ScenarioType.Screen:
                return new Color(.1f, .4f, .6f);
            case ScenarioType.Audio:
                return Color.yellow;
            case ScenarioType.CharacterMove:
                return new Color(.5f, .1f, .9f);
            case ScenarioType.CharacterScale:
                return new Color(.0f, .1f, .9f);
            case ScenarioType.CharacterAnim:
                return Color.magenta;
            case ScenarioType.BubbleDialogue:
                return new Color(.8f, .12f, .63f);
            case ScenarioType.ChangeMap:
                return new Color(1f, 0.34f, 0f);
            case ScenarioType.SocialAlbumPost:
                return new Color(0.44f, 0.8f, 1f);
            default:
                DebugPro.LogError<ScenarioLogTag>("please add new scenario color!");
                return Color.black;
        }
    }
    
    
    
    public void DrawScenarioItemGUI(State_Map state_Map, ScenarioItem scenarioItem)
    {
        int addPosition = -1;
        CharacterAnimator characterAnimator;
        Vector3 tempVector3 = scenarioItem.vector3_0;
        Vector3 tempScale = scenarioItem.scale_0;
        GUIStyle guiStyle = new GUIStyle(other: EditorStyles.label);

        switch (scenarioItem.scenarioType)
        {
            case ScenarioType.Dialogue:
                ScenarioDialogueData scenarioDialogueData = null;

                if (scenarioItem.dialogues.Count > 0)
                {
                    int swapItemIndexA = -1, swapItemIndexB = -1;
                    for (int i = 0; i < scenarioItem.dialogues.Count; i++)
                    {
                        ScenarioDialogueData item = scenarioItem.dialogues[i];

                        GUILayout.BeginHorizontal();
                        EditorGUIUtility.labelWidth = 60;
                        item.characterName = (CharacterName)EditorGUILayout.EnumPopup("character:", item.characterName, GUILayout.Width(160));
                        EditorGUIUtility.labelWidth = 35;
                        item.characterState = (CharacterState)EditorGUILayout.EnumPopup("state:", item.characterState, GUILayout.Width(95));
                        EditorGUIUtility.labelWidth = 36;
                        item.isCharacterOnLeft = EditorGUILayout.Toggle("left?:", item.isCharacterOnLeft);
                        EditorGUIUtility.labelWidth = 55;
                        item.characterDialogue = EditorGUILayout.TextField("dialogue:", item.characterDialogue);
                        UpdateTranslation(item.characterLocalizedString.mTerm, GetFarsiLanguageIndex(),
                            item.characterDialogue);

                        if (GUILayout.Button("U", GUILayout.Width(20), GUILayout.Height(16)) && i > 0)
                        {
                            swapItemIndexA = i;
                            swapItemIndexB = i - 1;
                        }
                        if (GUILayout.Button("D", GUILayout.Width(20), GUILayout.Height(16)) && i < scenarioItem.dialogues.Count - 1)
                        {
                            swapItemIndexA = i;
                            swapItemIndexB = i + 1;
                        }

                        if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(16)))
                            scenarioDialogueData = item;
                        if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(16)))
                            addPosition = scenarioItem.dialogues.IndexOf(item) + 1;
                        GUILayout.EndHorizontal();
                        guiStyle.alignment = TextAnchor.MiddleCenter;
                        GUILayout.Label(item.characterDialogue.Persian(), guiStyle);
                        GUILayout.Space(16);
                    }

                    if (swapItemIndexA >= 0)
                    {
                        RemoveTerm(scenarioItem.dialogues[swapItemIndexA].characterLocalizedString.mTerm);
                        RemoveTerm(scenarioItem.dialogues[swapItemIndexB].characterLocalizedString.mTerm);

                        ScenarioDialogueData tempSwapItem = scenarioItem.dialogues[swapItemIndexA];
                        scenarioItem.dialogues[swapItemIndexA] = scenarioItem.dialogues[swapItemIndexB];
                        scenarioItem.dialogues[swapItemIndexB] = tempSwapItem;

                        var scenarioIndex = myTarget.scenarioItems.IndexOf(scenarioItem);
                        scenarioItem.dialogues[swapItemIndexA].characterLocalizedString = BuildTerm(ScenarioType.Dialogue, scenarioItem.dialogues[swapItemIndexA].characterDialogue,
                            scenarioIndex, swapItemIndexA).Term;
                        scenarioItem.dialogues[swapItemIndexB].characterLocalizedString = BuildTerm(ScenarioType.Dialogue, scenarioItem.dialogues[swapItemIndexB].characterDialogue,
                            scenarioIndex, swapItemIndexB).Term;
                        SaveLanguageSourceChanges();
                    }
                }
                else
                    if (GUILayout.Button("Add Dialogue", GUILayout.Height(16)))
                        addPosition = 0;

                if (addPosition >= 0)
                {
                    RemoveScenarioTerms(scenarioItem);
                    scenarioItem.dialogues.Insert(addPosition, new ScenarioDialogueData());
                    BuildScenarioTerms(scenarioItem, myTarget.scenarioItems.IndexOf(scenarioItem));
                    SaveLanguageSourceChanges();
                }

                if (scenarioDialogueData != null)
                {
                    RemoveScenarioTerms(scenarioItem);
                    scenarioItem.dialogues.Remove(scenarioDialogueData);
                    BuildScenarioTerms(scenarioItem, myTarget.scenarioItems.IndexOf(scenarioItem));
                    SaveLanguageSourceChanges();
                }

                break;
            case ScenarioType.Character:
                tempVector3 = scenarioItem.vector3_0;
                tempScale = scenarioItem.scale_0;
                float tempInt = scenarioItem.int_0;

                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                scenarioItem.character_0 = (CharacterName)EditorGUILayout.EnumPopup("character:", scenarioItem.character_0, GUILayout.Width(160));
                EditorGUIUtility.labelWidth = 55;
                scenarioItem.vector3_0 = EditorGUILayout.Vector3Field("location:", scenarioItem.vector3_0, GUILayout.Width(230));
                EditorGUIUtility.labelWidth = 50;
                scenarioItem.int_0 = EditorGUILayout.IntField("rotation:", scenarioItem.int_0, GUILayout.Width(85));
                EditorGUIUtility.labelWidth = 50;
                scenarioItem.scale_0 = EditorGUILayout.Vector3Field("scale:", scenarioItem.scale_0, GUILayout.Width(180));

                if (state_Map)
                {
                    characterAnimator = Base.gameManager.mapCharactersManager.GetCharacter(scenarioItem.character_0);

                    if (tempVector3 != scenarioItem.vector3_0)
                        characterAnimator.SetPosition(scenarioItem.vector3_0);

                    if (tempInt != scenarioItem.int_0)
                        characterAnimator.SetRotation(scenarioItem.int_0);

                    if (tempScale != scenarioItem.scale_0)
                        characterAnimator.SetScale(scenarioItem.scale_0);

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Get Position", GUILayout.Width(80), GUILayout.Height(16)))
                        scenarioItem.vector3_0 = characterAnimator.GetPosition();

                    if (GUILayout.Button("Get Scale", GUILayout.Width(80), GUILayout.Height(16)))
                        scenarioItem.scale_0 = characterAnimator.GetScale();

                    if (GUILayout.Button("Set Position", GUILayout.Width(80), GUILayout.Height(16)))
                    {
                        characterAnimator.SetPosition(scenarioItem.vector3_0);
                        characterAnimator.SetRotation(scenarioItem.int_0);
                    }

                    if (GUILayout.Button("Set Scale", GUILayout.Width(80), GUILayout.Height(16)))
                        characterAnimator.SetScale(scenarioItem.scale_0);
                }
                GUILayout.EndHorizontal();

                break;
            case ScenarioType.MapItem_Selector:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 40;
                scenarioItem.int_0 = EditorGUILayout.IntField("index:", scenarioItem.int_0, GUILayout.Width(90));

                if (Selection.activeGameObject)
                {
                    MapItem_UserSelect mapItem_UserSelect = Selection.activeGameObject.GetComponent<MapItem_UserSelect>();
                    GUILayout.FlexibleSpace();
                    if (mapItem_UserSelect && GUILayout.Button("Get Index", GUILayout.Width(70), GUILayout.Height(16)))
                        scenarioItem.int_0 = mapItem_UserSelect.GetItemId();
                }
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.MapItem_SetSate:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 40;
                scenarioItem.int_0 = EditorGUILayout.IntField("index:", scenarioItem.int_0, GUILayout.Width(90));
                EditorGUIUtility.labelWidth = 40;
                scenarioItem.int_1 = EditorGUILayout.IntField("state:", scenarioItem.int_1, GUILayout.Width(70));
                if (Selection.activeGameObject)
                {
                    MapItem_State mapItem_State = Selection.activeGameObject.GetComponent<MapItem_State>();
                    GUILayout.FlexibleSpace();
                    if (mapItem_State && GUILayout.Button("Get Index", GUILayout.Width(70), GUILayout.Height(16)))
                        scenarioItem.int_0 = mapItem_State.GetItemId();
                }
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.Camera_Focus:
                Vector2 tempVector2 = scenarioItem.vector2_0;
                float tempFloat = scenarioItem.float_0;

                GUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 55;
                scenarioItem.vector2_0 = EditorGUILayout.Vector2Field("position:", scenarioItem.vector2_0, GUILayout.Width(230));

                EditorGUIUtility.labelWidth = 45;
                scenarioItem.float_0 = EditorGUILayout.FloatField("zoom:", scenarioItem.float_0, GUILayout.Width(70));
                scenarioItem.float_0 = Mathf.Clamp(scenarioItem.float_0, 4, 10);

                EditorGUIUtility.labelWidth = 65;
                scenarioItem.isInstantly = EditorGUILayout.Toggle("Instantly?", scenarioItem.isInstantly, GUILayout.Width(90));
                EditorGUIUtility.labelWidth = 45;

                if (state_Map)
                {
                    Transform cameraParent = state_Map.mapCameraController.cameraParentTransform;
                    if (tempVector2 != scenarioItem.vector2_0)
                        cameraParent.position = new Vector3(scenarioItem.vector2_0.x, cameraParent.position.y, scenarioItem.vector2_0.y);

                    if (tempFloat != scenarioItem.float_0)
                        state_Map.mapCameraController.mapCamera.orthographicSize = scenarioItem.float_0;

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Get Position", GUILayout.Width(80), GUILayout.Height(16)))
                    {
                        scenarioItem.vector2_0.x = cameraParent.position.x;
                        scenarioItem.vector2_0.y = cameraParent.position.z;
                    }
                    if (GUILayout.Button("Set Position", GUILayout.Width(80), GUILayout.Height(16)))
                    {
                        cameraParent.position = new Vector3(scenarioItem.vector2_0.x, cameraParent.position.y, scenarioItem.vector2_0.y);
                        state_Map.mapCameraController.mapCamera.orthographicSize = scenarioItem.float_0;
                    }
                }
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.Screen:
                scenarioItem.string_0 = EditorGUILayout.TextField("text:", scenarioItem.string_0);
                UpdateTranslation(scenarioItem.localizedString_0.mTerm, GetFarsiLanguageIndex(),
                    scenarioItem.string_0);
                break;
            case ScenarioType.Audio:
                scenarioItem.audioClip_0 = EditorGUILayout.ObjectField(scenarioItem.audioClip_0, typeof(AudioClip), false) as AudioClip;
                break;
            case ScenarioType.CharacterMove:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                scenarioItem.character_0 = (CharacterName)EditorGUILayout.EnumPopup("character:", scenarioItem.character_0, GUILayout.Width(160));
                if (scenarioItem.vector3s.Count > 0)
                {
                    tempVector3 = scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex];

                    GUILayout.Label((scenarioItem.selectedCharacterMoveIndex + 1).ToString() + "/" + scenarioItem.vector3s.Count.ToString(), GUILayout.Width(25));
                    if (GUILayout.Button("<", GUILayout.Width(20), GUILayout.Height(16)) && scenarioItem.selectedCharacterMoveIndex > 0)
                        scenarioItem.selectedCharacterMoveIndex--;
                    if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(16)) && scenarioItem.selectedCharacterMoveIndex < scenarioItem.vector3s.Count - 1)
                        scenarioItem.selectedCharacterMoveIndex++;
                    scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex] = EditorGUILayout.Vector3Field("", scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex], GUILayout.Width(230));

                    if (state_Map)
                    {
                        characterAnimator = Base.gameManager.mapCharactersManager.GetCharacter(scenarioItem.character_0);
                        if (tempVector3 != scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex])
                            characterAnimator.SetPosition(scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex]);

                        if (GUILayout.Button("Get Position", GUILayout.Width(80), GUILayout.Height(16)))
                            scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex] = characterAnimator.GetPosition();

                        if (GUILayout.Button("Set Position", GUILayout.Width(80), GUILayout.Height(16)))
                            characterAnimator.SetPosition(scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex]);
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        scenarioItem.vector3s.Remove(scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex]);
                        if (scenarioItem.selectedCharacterMoveIndex > 0)
                            scenarioItem.selectedCharacterMoveIndex--;
                    }

                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        scenarioItem.vector3s.Insert(scenarioItem.selectedCharacterMoveIndex, scenarioItem.vector3s[scenarioItem.selectedCharacterMoveIndex]);
                        scenarioItem.selectedCharacterMoveIndex += 1;
                    }
                }
                else
                {
                    if (GUILayout.Button("+", GUILayout.Height(20), GUILayout.Height(16)))
                    {
                        scenarioItem.selectedCharacterMoveIndex = 0;
                        scenarioItem.vector3s.Add(new Vector3(150f, 3.1f, -25f));
                    }
                }
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.CharacterScale:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                scenarioItem.character_0 = (CharacterName)EditorGUILayout.EnumPopup("character:", scenarioItem.character_0, GUILayout.Width(160));
                if (scenarioItem.scales.Count > 0)
                {
                    tempScale = scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex];

                    GUILayout.Label((scenarioItem.selectedCharacterScaleIndex + 1).ToString() + "/" + scenarioItem.scales.Count.ToString(), GUILayout.Width(25));
                    if (GUILayout.Button("<", GUILayout.Width(20), GUILayout.Height(16)) && scenarioItem.selectedCharacterScaleIndex > 0)
                        scenarioItem.selectedCharacterScaleIndex--;
                    if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(16)) && scenarioItem.selectedCharacterScaleIndex < scenarioItem.scales.Count - 1)
                        scenarioItem.selectedCharacterScaleIndex++;
                    scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex] = EditorGUILayout.Vector3Field("", scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex], GUILayout.Width(180));
                    scenarioItem.scalesDurations[scenarioItem.selectedCharacterScaleIndex] = EditorGUILayout.FloatField("Duration", scenarioItem.scalesDurations[scenarioItem.selectedCharacterScaleIndex], GUILayout.Width(95));

                    if (state_Map)
                    {
                        characterAnimator = Base.gameManager.mapCharactersManager.GetCharacter(scenarioItem.character_0);
                        if (tempScale != scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex])
                            characterAnimator.SetScale(scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex]);

                        if (GUILayout.Button("Get Scale", GUILayout.Width(80), GUILayout.Height(16)))
                            scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex] = characterAnimator.GetScale();

                        if (GUILayout.Button("Set Scale", GUILayout.Width(80), GUILayout.Height(16)))
                            characterAnimator.SetScale(scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex]);
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        scenarioItem.scales.Remove(scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex]);
                        scenarioItem.scalesDurations.Remove(scenarioItem.scalesDurations[scenarioItem.selectedCharacterScaleIndex]);
                        if (scenarioItem.selectedCharacterScaleIndex > 0)
                            scenarioItem.selectedCharacterScaleIndex--;
                    }

                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        scenarioItem.scales.Insert(scenarioItem.selectedCharacterScaleIndex, scenarioItem.scales[scenarioItem.selectedCharacterScaleIndex]);
                        scenarioItem.scalesDurations.Insert(scenarioItem.selectedCharacterScaleIndex, scenarioItem.scalesDurations[scenarioItem.selectedCharacterScaleIndex]);
                        scenarioItem.selectedCharacterScaleIndex += 1;
                    }
                }
                else
                {
                    if (GUILayout.Button("+", GUILayout.Height(20), GUILayout.Height(16)))
                    {
                        scenarioItem.selectedCharacterScaleIndex = 0;
                        scenarioItem.scales.Add(new Vector3(1, 1, 1));
                        scenarioItem.scalesDurations.Add(0);
                    }
                }
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.CharacterAnim:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                scenarioItem.character_0 = (CharacterName)EditorGUILayout.EnumPopup("character:", scenarioItem.character_0, GUILayout.Width(160));
                EditorGUIUtility.labelWidth = 40;
                CharacterAnim characterAnim = (CharacterAnim)scenarioItem.int_0;
                characterAnim = (CharacterAnim)EditorGUILayout.EnumPopup("index:", characterAnim, GUILayout.Width(105));
                scenarioItem.int_0 = (int)characterAnim;
                EditorGUIUtility.labelWidth = 40;
                scenarioItem.int_1 = EditorGUILayout.IntField("index:", scenarioItem.int_1, GUILayout.Width(70));
                //EditorGUIUtility.labelWidth = 65;
                //bool_0 = EditorGUILayout.Toggle("idle finish?:", bool_0);
                GUILayout.EndHorizontal();
                break;
            case ScenarioType.BubbleDialogue:
                GUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 60;
                scenarioItem.character_0 = (CharacterName)EditorGUILayout.EnumPopup("character:", scenarioItem.character_0, GUILayout.Width(160));
                EditorGUIUtility.labelWidth = 55;
                scenarioItem.float_0 = EditorGUILayout.FloatField("duration:", scenarioItem.float_0, GUILayout.Width(80));
                EditorGUIUtility.labelWidth = 55;
                scenarioItem.string_0 = EditorGUILayout.TextField("dialogue:", scenarioItem.string_0);
                UpdateTranslation(scenarioItem.localizedString_0.mTerm, GetFarsiLanguageIndex(),
                    scenarioItem.string_0);
                GUILayout.EndHorizontal();
                guiStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.Label(string.IsNullOrEmpty(scenarioItem.string_0) ? "" : scenarioItem.string_0.Replace("\\n", "\n").Persian(), guiStyle);
                //GUILayout.Space(16);
                break;
            case ScenarioType.ChangeMap:
                var mapIds = data.MapsIDsDatabase.mapIDs;
                var selectedIndex = mapIds.IndexOf(scenarioItem.string_0);
                if (selectedIndex < 0) selectedIndex = 0;
                selectedIndex = EditorGUILayout.Popup("MapId: ", selectedIndex, mapIds.ToArray());
                scenarioItem.string_0 = mapIds[selectedIndex];
                break;
            case ScenarioType.SocialAlbumPost:
                var socialPosts = data.SocialAlbumPostsDataBase.albumPosts;
                List<string> postIds = socialPosts.Select(post => post.postId).ToList();
                var selectedPost = postIds.IndexOf(scenarioItem.string_0);
                if (selectedPost < 0) selectedPost = 0;
                selectedPost = EditorGUILayout.Popup("PostId: ", selectedPost, postIds.ToArray());
                scenarioItem.string_0 = postIds[selectedPost];
                break;
            default:
                break;
        }

        //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        GUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 40;
        scenarioItem.delay = EditorGUILayout.FloatField("delay:", scenarioItem.delay, GUILayout.Width(70));
        EditorGUIUtility.labelWidth = 55;
        scenarioItem.goNext = EditorGUILayout.Toggle("continue:", scenarioItem.goNext, GUILayout.Width(75));
    }

    
    
    
    
}