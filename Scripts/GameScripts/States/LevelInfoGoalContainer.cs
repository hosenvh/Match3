using System;
using SeganX;
using UnityEngine;

public class LevelInfoGoalContainer :MonoBehaviour
{
    public LocalText amountText;

    internal void Setup(GameObject goalPrefab, int goalAmount)
    {
        var goal = Instantiate(goalPrefab, this.transform, false);
        goal.transform.position = this.transform.position;
        goal.transform.SetAsFirstSibling();
        goal.SetActive(true);
        amountText.SetText(goalAmount.ToString());
    }
}
