using System.Collections.Generic;
using I2.Loc;
using Match3.CharacterManagement.Character.Base;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif


public enum ScenarioType
{
    None,
    Dialogue,
    Character,
    MapItem_Selector,
    MapItem_SetSate,
    Camera_Focus,
    Screen,
    Audio,
    CharacterMove,
    CharacterAnim,
    BubbleDialogue,
    ChangeMap,
    SocialAlbumPost,
    CharacterScale
}


[System.Serializable]
public class ScenarioItem
{
    public string scenarioName;

    public ScenarioType scenarioType;
    public float delay;
    public bool goNext;

    public CharacterName character_0;
    public int int_0, int_1;
    public float float_0;
    public bool isInstantly;
    public Vector2 vector2_0;
    public Vector3 vector3_0;
    public Vector3 scale_0 = Vector3.one;
    public string string_0;
    public LocalizedStringTerm localizedString_0;
    public AudioClip audioClip_0;
    public LocalizedAudioClipTerm localizedAudio_0;
    public List<ScenarioDialogueData> dialogues = new List<ScenarioDialogueData>();
    public List<Vector3> vector3s = new List<Vector3>();
    public List<Vector3> scales = new List<Vector3>();
    public List<float> scalesDurations = new List<float>();
    public CharacterAnim characterAnim_0 = CharacterAnim.Idle;


#if UNITY_EDITOR
    public int selectedCharacterMoveIndex = -1;
    public int selectedCharacterScaleIndex = -1;
#endif
    
    
    public ScenarioItem(ScenarioType scenarioType)
    {
        this.scenarioType = scenarioType;
        this.scale_0 = Vector3.one;
    }


    public string GetLocalizedString0()
    {
        return localizedString_0.ToString();
    }

    public AudioClip GetLocalizedAudio0()
    {
        return localizedAudio_0.Load();
    }

}


[System.Serializable]
public class ScenarioDialogueData
{
    [FormerlySerializedAs("character")]
    public CharacterName characterName;
    public CharacterState characterState;
    public string characterDialogue = "new line";
    public LocalizedStringTerm characterLocalizedString;
    public bool isCharacterOnLeft;
    public LocalizedAudioClipTerm localizedAudio;

    public string GetLocalizedDialogueText()
    {
        return characterLocalizedString.ToString();
    }

    public AudioClip GetLocalizedAudioDialogue()
    {
        return localizedAudio.Load();
    }
}