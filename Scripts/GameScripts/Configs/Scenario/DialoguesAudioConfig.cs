using System;


namespace Match3.Scenario
{
    [Serializable]
    public class ScenarioDialoguesAudioServerConfig
    {
        public bool playDialogueAudio;
    }

    [Serializable]
    public class ScenarioDialoguesAudioServerCohortConfig : CohortConfigReplacer<ScenarioDialoguesAudioServerConfig>
    {
    }
}