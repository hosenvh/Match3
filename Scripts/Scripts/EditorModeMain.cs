#if UNITY_EDITOR
using Match3.Foundation.Base.Configuration;
using Match3.Foundation.Base.ServiceLocating;
using Match3.Foundation.Unity.Configuration;
using Match3.CharacterManagement.Character.Game;
using UnityEditor;

namespace Match3.Main
{
    [InitializeOnLoad]
    public static class EditorModeMain
    {
        private const string configurationMasterPath = "Assets/Configs/ConfigurationMaster.asset";

        private static BasicConfigurationManager configurationManager;

        static EditorModeMain()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode == false)
                SetupServices();

        }


        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if(state == PlayModeStateChange.EnteredEditMode)
            {
                SetupServices();
            }
        }

        private static void SetupServices()
        {
            UnityEngine.Debug.Log("Setting up editor services");

            ServiceLocator.Clear();
            ServiceLocator.Init();

            configurationManager = new BasicConfigurationManager();
            AssetDatabase.LoadAssetAtPath<UnityConfigurationMaster>(configurationMasterPath).RegisterSelf(configurationManager);

            ServiceLocator.Register(configurationManager);

            ServiceLocator.Register(new CharactersManagerFactory().CreateCharactersManager());
        }
    }
}
#endif