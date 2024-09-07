using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;
using PandasCanPlay.HexaWord.Utility;
using Match3.Game;
using Match3.Data.Unity.PersistentTypes;
using I2.Loc;

[Serializable]
public class HiddenReward
{
    [Type(typeof(Reward), includeAbstracts: false, showPartialName: true)]
    public string reward;

    public int count;

    public Reward GetReward()
    {
        return CreateReward();

        Reward CreateReward()
        {
            Type rewardType = Type.GetType(reward);
            return Activator.CreateInstance(rewardType, count) as Reward;
        }
    }
}

[RequireComponent(typeof(MapItem_State), typeof(NonDrawingGraphic)), DisallowMultipleComponent]
public class MapItem_HiddenReward : Base, IPointerClickHandler
{
    [SerializeField] private List<HiddenReward> rewards;

    private PersistentBool isRewardClaimed;

    private bool IsRewardClaimed => isRewardClaimed.Get();

    private void Awake()
    {
        string sharedKeyWithId = $"HiddenReward_{this.GetComponent<MapItem_State>().GetItemId()}";
        isRewardClaimed = new PersistentBool($"{sharedKeyWithId}_Is_Claimed");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsRewardClaimed)
            return;

        gameManager.OpenPopup<Popup_ConfirmBox>().Setup(
            messageString: ScriptLocalization.Message_HiddenReward.FoundHiddenReward,
            confirmString: ScriptLocalization.UI_General.Ok,
            cancelString: null,
            closeOnConfirm: true,
            onResult: result => ApplyAllRewards()
        );

        void ApplyAllRewards()
        {
            ApplyAllRewardsLogically();
            MarkHiddenRewardAsClaimed();
            OpenClaimRewardPopup();
        }

        void ApplyAllRewardsLogically()
        {
            GetRewards().ForEach(reward => reward.Apply());
        }

        void OpenClaimRewardPopup()
        {
            gameManager.OpenPopup<Popup_ClaimReward>()
                .Setup(GetRewards())
                .StartPresentingRewards();
        }
    }

    private List<Reward> GetRewards()
    {
        return rewards.Select(reward => reward.GetReward()).ToList();
    }

    private void MarkHiddenRewardAsClaimed()
    {
        isRewardClaimed.Set(true);
    }
}