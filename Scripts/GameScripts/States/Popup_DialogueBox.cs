#pragma warning disable CS0162
using I2.Loc;
using Match3;
using Match3.CharacterManagement.Character.Base;
using Match3.CharacterManagement.Character.Game;
using UnityEngine;
using SeganX;
using Match3.Foundation.Base.ServiceLocating;


public class Popup_DialogueBox : GameState
{
    [SerializeField]
    GameObject objects_Left = default, objects_Right = default;

    [SerializeField]
    private LocalText dialogueText = null, leftNameText = default, rightNameText = default;
    [SerializeField]
    private GameObject leftCharacterObject = null, rightCharacterObject = null;
    [SerializeField]
    private AudioSource audioSource;

    System.Action onClick = null;
    System.Action onSkipCallback = null;

    public Popup_DialogueBox Setup(ScenarioDialogueData dialogue, System.Action onClick, System.Action onSkipCallback)
    {
        var configuration = ServiceLocator.Find<ServerConfigManager>().data;
        var audioDialogue = dialogue.GetLocalizedAudioDialogue();
        var shouldPlayAudio = audioDialogue != null && configuration.config.scenarioDialoguesAudioServerConfig.playDialogueAudio;

        if (shouldPlayAudio)
            audioSource.PlayOneShot(audioDialogue);
        dialogueText.SetText(dialogue.GetLocalizedDialogueText());

        bool isOnLeft = dialogue.isCharacterOnLeft;
        objects_Left.SetActive(isOnLeft);
        objects_Right.SetActive(!isOnLeft);

        string characterName = "";
        switch (dialogue.characterName)
        {
            case CharacterName.Golmorad:
                characterName = ScriptLocalization.Misc_CharacterNames.Man;
                break;
            case CharacterName.Elham:
                characterName = ScriptLocalization.Misc_CharacterNames.Woman;
                break;
            case CharacterName.Mother:
                characterName = ScriptLocalization.Misc_CharacterNames.Mother;
                break;
            case CharacterName.Postman:
                characterName = ScriptLocalization.Misc_CharacterNames.Postman;
                break;
            case CharacterName.Tooti:
                characterName = ScriptLocalization.Misc_CharacterNames.Parrot;
                break;
            case CharacterName.Plumber:
                characterName = ScriptLocalization.Misc_CharacterNames.Plumber;
                break;
            case CharacterName.CarDriver:
                characterName = ScriptLocalization.Misc_CharacterNames.CarDriver;
                break;
            case CharacterName.Ghalisho:
                characterName = ScriptLocalization.Misc_CharacterNames.Ghalisho;
                break;
            case CharacterName.Pezhman:
                characterName = ScriptLocalization.Misc_CharacterNames.Pezhman;
                break;
            case CharacterName.Dad:
                characterName = ScriptLocalization.Misc_CharacterNames.Dad;
                break;
            case CharacterName.Khatereh:
                characterName = ScriptLocalization.Misc_CharacterNames.Khatereh;
                break;
            case CharacterName.Masood:
                characterName = ScriptLocalization.Misc_CharacterNames.Masood;
                break;
            case CharacterName.Mahboobeh:
                characterName = ScriptLocalization.Misc_CharacterNames.Mahboobeh;
                break;
            case CharacterName.BabyBoy:
                characterName = ScriptLocalization.Misc_CharacterNames.BabyBoy;
                break;
            case CharacterName.BabyGirl:
                characterName = ScriptLocalization.Misc_CharacterNames.BabyGirl;
                break;
            case CharacterName.Golmorad_Scifi:
                characterName = ScriptLocalization.Misc_CharacterNames.ManScifi;
                break;
            case CharacterName.UglyElham:
                characterName = ScriptLocalization.Misc_CharacterNames.UglyElham;
                break;
            case CharacterName.Pirate:
                characterName = ScriptLocalization.Misc_CharacterNames.Pirate;
				break;
            case CharacterName.FemaleTooti:
                characterName = ScriptLocalization.Misc_CharacterNames.FemaleParrot;
                break;
            default:
                break;
        }
        leftNameText.SetText(characterName);
        rightNameText.SetText(characterName);
        InstantiateCharacter(dialogue, isOnLeft);
        this.onClick = onClick;
        this.onSkipCallback = onSkipCallback;

        ValidateReceivedDialogueText();

        void ValidateReceivedDialogueText()
        {
            if (IsTextInvalid())
                LogErrorTextIsNotValid();

            bool IsTextInvalid() => dialogue.GetLocalizedDialogueText() == "";
            void LogErrorTextIsNotValid() => Debug.LogError($"Trying to show a dialogue popup with an empty string | characterName: {dialogue.characterName}");
        }

        return this;
    }

    private void InstantiateCharacter(ScenarioDialogueData dialogue, bool isOnLeft)
    {
        RemoveChildren();
        Spine.Unity.SkeletonGraphic data = ServiceLocator.Find<CharactersManager>().LoadCharacterSpine(dialogue.characterName) ;
        Spine.Unity.SkeletonGraphic skeletonGraphic = null;
        if (isOnLeft)
        {
            skeletonGraphic = Instantiate<Spine.Unity.SkeletonGraphic>(data, leftCharacterObject.transform, false);
        }
        else
        {
            skeletonGraphic = Instantiate<Spine.Unity.SkeletonGraphic>(data, rightCharacterObject.transform, false);
        }

        skeletonGraphic.AnimationState.SetAnimation(0, GetAnimationName(dialogue.characterName, dialogue.characterState), false);
    }



    private void RemoveChildren()
    {
        var data = leftCharacterObject.GetComponentsInChildren<Spine.Unity.SkeletonGraphic>();
        foreach (var item in data)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject);
        }

        data = rightCharacterObject.GetComponentsInChildren<Spine.Unity.SkeletonGraphic>();
        foreach (var item in data)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject);
        }
    }

    public void OnButtonClick()
    {
        audioSource.Stop();
        if (onClick != null)
            onClick();
    }

    public override void Back()
    {
    }

    string GetAnimationName(CharacterName character, CharacterState state)
    {
        switch (character)
        {
            case CharacterName.Golmorad:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal_1";
                    case CharacterState.Smile:
                        return "smile_1";
                    case CharacterState.Sad:
                        return "sad_1";
                    case CharacterState.Laugh:
                        return "laugh_1";
                    case CharacterState.Angry:
                        return (Random.value > .5f) ? "angry_1" : "angry_2";
                    case CharacterState.Think:
                    case CharacterState.Surprise:
                        return (Random.value > .5f) ? "think_1" : "think_2";
                    case CharacterState.Phone:
                        return "phone_1";
#if !UNITY_EDITOR
                    default:
                        return "normal_1";
                }
#else
                }
                break;
#endif
            case CharacterName.Elham:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal_1";
                    case CharacterState.Sad:
                        return (Random.value > .5f) ? "sad_1" : "sad_2";
                    case CharacterState.Laugh:
                    case CharacterState.Smile:
                        return "laugh_1";
                    case CharacterState.Angry:
                        return (Random.value > .5f) ? "angry_1" : "angry_2";
                    case CharacterState.Think:
                        return "think_1";
                    case CharacterState.Surprise:
                        return "suprise_1";
                    case CharacterState.Phone:
                        return null;
#if !UNITY_EDITOR
                    default:
                        return "normal_1";
                }
#else
                }
                break;
#endif
            case CharacterName.UglyElham:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal_1";
                    case CharacterState.Sad:
                        return (Random.value > .5f) ? "sad_1" : "sad_2";
                    case CharacterState.Laugh:
                    case CharacterState.Smile:
                        return "laugh_1";
                    case CharacterState.Angry:
                        return (Random.value > .5f) ? "angry_1" : "angry_2";
                    case CharacterState.Think:
                        return "think_1";
                    case CharacterState.Surprise:
                        return "suprise_1";
                    case CharacterState.Phone:
                        return null;
                    #if !UNITY_EDITOR
                    default:
                        return "normal_1";
                }
                    #else
                }
                break;
            #endif
            case CharacterName.Mother:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal_1";
                    case CharacterState.Phone:
                        return "phone_1";
#if !UNITY_EDITOR
                    default:
                        return "normal_1";
                }
#else
                }
                break;
#endif
            case CharacterName.Postman:
                switch (state)
                {
                    case CharacterState.Normal:
                        return Random.Range(1, 6).ToString();
                    case CharacterState.Smile:
                        return Random.Range(6, 11).ToString();
#if !UNITY_EDITOR
                    default:
                        return Random.Range(1, 6).ToString();
                }
#else
                }
                break;
#endif
            case CharacterName.Plumber:
                switch (state)
                {
                    case CharacterState.Normal:
                        int random = Random.Range(0, 4);
                        switch (random)
                        {
                            case 0:
                                return "1";
                            case 1:
                                return "3";
                            case 2:
                                return "7";
                        }
                        return "9";
                    case CharacterState.Phone:
                        return Random.value > .5f ? "2" : "8";
                    case CharacterState.Surprise:
                        return Random.value > .5f ? "4" : "6";
#if !UNITY_EDITOR
                    default:
                        return "1";
                }
#else
                }
                break;
#endif
            case CharacterName.CarDriver:
                switch (state)
                {
                    case CharacterState.Normal:
                        return Random.Range(5, 9).ToString();
                    case CharacterState.Smile:
                        return Random.Range(1, 5).ToString();
#if !UNITY_EDITOR
                    default:
                        return Random.Range(5, 9).ToString();
                }
#else
                }
                break;
#endif
            case CharacterName.Ghalisho:
                switch (state)
                {
                    case CharacterState.Normal:
                        return Random.value > .5f ? "1" : "3";
                    case CharacterState.Phone:
                        return Random.value > .5f ? "2" : "5";
                    case CharacterState.Smile:
                        return Random.value > .5f ? "4" : "6";
#if !UNITY_EDITOR
                    default:
                        return Random.value > .5f ? "1" : "3";
                }
#else
                }
                break;
#endif
            case CharacterName.Pezhman:
                switch (state)
                {
                    case CharacterState.Phone:
                        return "1";
                    case CharacterState.Smile:
                        return "2";
                    case CharacterState.Angry:
                        return "3";
                    case CharacterState.Normal:
                        return "4";
                    case CharacterState.Sad:
                        return "5";
                    case CharacterState.Laugh:
                        return "6";
#if !UNITY_EDITOR
                    default:
                        return "4";
                }
#else
                }
                break;
#endif
            case CharacterName.Dad:
                switch (state)
                {
                    case CharacterState.Angry:
                        return "angry";
                    case CharacterState.Laugh:
                        return "happy";
                    case CharacterState.Sad:
                        return "sad";
                    case CharacterState.Normal:
                        return "smile";
                    case CharacterState.Smile:
                        return "smile";
                    case CharacterState.Surprise:
                        return "surprise";
                    case CharacterState.Think:
                        return "thinking";
#if !UNITY_EDITOR
                    default:
                        return "smile";
                }
#else
                }
                break;
#endif
            case CharacterName.Khatereh:
                switch (state)
                {
                    case CharacterState.Phone:
                        return "1";
                    case CharacterState.Laugh:
                        return "2";
                    case CharacterState.Normal:
                        return "3";
                    case CharacterState.Surprise:
                        return "4";
                    case CharacterState.Angry:
                        return "5";
                    case CharacterState.Sad:
                        return "6";
#if !UNITY_EDITOR
                    default:
                        return "3";
                }
#else
                }
#endif
                break;
            case CharacterName.Masood:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "1";
                    case CharacterState.Smile:
                        return Random.value > .5f ? "2" : "5"; ;
                    case CharacterState.Sad:
                        return "3";
                    case CharacterState.Surprise:
                        return "4";
#if !UNITY_EDITOR
                    default:
                        return "1";
                }
#else
                }
                break;
#endif
            case CharacterName.Mahboobeh:
                switch (state)
                {
                    case CharacterState.Normal:
                    case CharacterState.Smile:
                        return "Smile";
                    case CharacterState.Sad:
                        return "sad";
                    case CharacterState.Surprise:
                        return "Surprise";
                    case CharacterState.Angry:
                        return "angry";
                    case CharacterState.Phone:
                        return "phone";
                    case CharacterState.Laugh:
                        return "happy";
#if !UNITY_EDITOR
                    default:
                        return "Smile";
                }
#else
                }
                break;
#endif
            case CharacterName.BabyBoy:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "1";
                    case CharacterState.Smile:
                    case CharacterState.Laugh:
                        return "2";
                    case CharacterState.Sad:
                        return "4";
                    case CharacterState.Angry:
                        return "5";
                    case CharacterState.Think:
                        return "3";
#if !UNITY_EDITOR
                    default:
                        return "1";
                }
#else
                }
                break;
#endif
            case CharacterName.BabyGirl:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "1";
                    case CharacterState.Smile:
                    case CharacterState.Laugh:
                        return "2";
                    case CharacterState.Sad:
                        return "4";
                    case CharacterState.Angry:
                        return "5";
                    case CharacterState.Think:
                        return "3";
#if !UNITY_EDITOR
                    default:
                        return "1";
                }
#else
                }
                break;
#endif
            case CharacterName.Golmorad_Scifi:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal_1";
                    case CharacterState.Smile:
                        return "smile_1";
                    case CharacterState.Sad:
                        return "sad_1";
                    case CharacterState.Laugh:
                        return "laugh_1";
                    case CharacterState.Angry:
                        return (Random.value > .5f) ? "angry_1" : "angry_2";
                    case CharacterState.Think:
                    case CharacterState.Surprise:
                        return (Random.value > .5f) ? "think_1" : "think_2";
                    case CharacterState.Phone:
                        return "phone_1";
                    #if !UNITY_EDITOR
                    default:
                        return "normal_1";
                }
#else
                }
                break;
#endif
            case CharacterName.Pirate:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal";
                    case CharacterState.Smile:
                        return "happy";
                    case CharacterState.Laugh:
                        return "surprize";
                    case CharacterState.Sad:
                        return "sad";
                    case CharacterState.Angry:
                        return "angry";
                    case CharacterState.Think:
                        return "thinking";
                    case CharacterState.Surprise:
                        return "wonder";
                    case CharacterState.Phone:
                        return "phone";
#if !UNITY_EDITOR
                    default:
                        return "normal";
                }
#else
                }
                break;
#endif
            case CharacterName.FemaleTooti:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal";
                    case CharacterState.Smile:
                        return "happy";
                    case CharacterState.Sad:
                        return "sad";
                    case CharacterState.Laugh:
                        return "surprized";
                    case CharacterState.Surprise:
                        return "surprized";
                    case CharacterState.Angry:
                        return "angry";
                    case CharacterState.Think:
                        return "think";
#if !UNITY_EDITOR
                    default:
                        return "normal";
                }
#else
                }
                break;
#endif

            case CharacterName.Tooti:
                switch (state)
                {
                    case CharacterState.Normal:
                        return "normal";
                    case CharacterState.Smile:
                        return "happy";
                    case CharacterState.Sad:
                        return "sad";
                    case CharacterState.Laugh:
                        return "surprized";
                    case CharacterState.Surprise:
                        return "surprized";
                    case CharacterState.Angry:
                        return "angry";
                    case CharacterState.Think:
                        return "think";
#if !UNITY_EDITOR
                    default:
                        return "normal";
                }
#else
                }
                break;
#endif
        }

        return null;
    }

    public void SkipButtonCallback()
    {
        audioSource.Stop();
        onSkipCallback?.Invoke();
    }
}
