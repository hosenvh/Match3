namespace Match3.Game.MapItemResoration
{
    // NOTE: These utitliy methods shoud become part Task Management.
    public static class MapItemRestorationUtilities
    {
        public delegate void MapItemScenarioAction(ScenarioItem scenarioItem, string mapID);

        public static void ForEachStateScenarioItem(this TaskConfig taskConfig, string defaultMapId, MapItemScenarioAction action)
        {
            ForEachMapScenarioItemIn(taskConfig.preScenarioConfig, defaultMapId, ScenarioType.MapItem_SetSate, action);
            ForEachMapScenarioItemIn(taskConfig.postScenarioConfig, defaultMapId, ScenarioType.MapItem_SetSate, action);
        }

        public static void ForEachUserSelectScenarioItem(this TaskConfig taskConfig, string defaultMapId, MapItemScenarioAction action)
        {
            ForEachMapScenarioItemIn(taskConfig.preScenarioConfig, defaultMapId, ScenarioType.MapItem_Selector, action);
            ForEachMapScenarioItemIn(taskConfig.postScenarioConfig, defaultMapId, ScenarioType.MapItem_Selector, action);
        }

        private static void ForEachMapScenarioItemIn(ScenarioConfig scenarioConfig, string defaultMapId, ScenarioType scenarioType, MapItemScenarioAction action)
        {
            if (scenarioConfig == null)
                return;

            var currentMapId = defaultMapId;

            foreach (var item in scenarioConfig.scenarioItems)
            {
                if (item.scenarioType == scenarioType)
                    action.Invoke(item, currentMapId);
                else if (item.scenarioType == ScenarioType.ChangeMap)
                    currentMapId = item.string_0;
            }
        }

        public static MapItemIdentifer ExtractMapItemIdenfitifer(this ScenarioItem scenarioItem, string mapID)
        {
            if (scenarioItem.scenarioType != ScenarioType.MapItem_Selector && scenarioItem.scenarioType != ScenarioType.MapItem_SetSate)
                throw new System.Exception($"Map Item Identifer is not defined for scenario type:{scenarioItem.scenarioType}");

            return new MapItemIdentifer(scenarioItem.int_0, mapID);
        }
    }
}