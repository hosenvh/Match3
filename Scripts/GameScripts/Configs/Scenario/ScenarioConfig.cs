using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scenario")]
public class ScenarioConfig : ScriptableObject
{
    public string configName;
    public List<ScenarioItem> scenarioItems = new List<ScenarioItem>();
}