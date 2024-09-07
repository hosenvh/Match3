using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;


namespace Medrick.Development.Unity.BuildManagement
{

    public enum DSPBufferSize
    {
        Default = 1024,
        BestLatency = 256,
        GoodLatency = 512,
        BestPerformance = 1024,
    }

    // The enum indexes are from EditorUserBuildSettings.bindings.cs
    public enum BuildCompression
    {
        None = 0,
        Lz4 = 2,
        Lz4HC = 3,
    }


    [CreateAssetMenu(menuName = "Medrick/Build Management/Build Actions/Golmorad Build Action")]
    public class GolmoradCustomBuildAction : ScriptableBuildAction
    {

        public DSPBufferSize DSPBufferSize;
        public BuildCompression buildCompression;

        public string keyStorePassword;
        public string keyAliasPassword;
        public string keyStoreRelativePath;
        
        public override void Execute()
        {
            SetDSPBufferSize();
            SetBuildCompression();
            SetKeyData();
        }

        private void SetDSPBufferSize()
        {
            var audioManager = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/AudioManager.asset");
            var serObj = new SerializedObject(audioManager);
            serObj.Update();
            var spatializerProperty = serObj.FindProperty("m_DSPBufferSize");
            spatializerProperty.intValue = (int)DSPBufferSize;
            serObj.ApplyModifiedProperties();
        }

        private void SetBuildCompression()
        {
            try
            {
                var target = EditorUserBuildSettings.activeBuildTarget;
                var group = BuildPipeline.GetBuildTargetGroup(target);

                MethodInfo setCompressionTypeMethodInfo = typeof(EditorUserBuildSettings).GetMethod("SetCompressionType", BindingFlags.NonPublic | BindingFlags.Static);

                var invoke = setCompressionTypeMethodInfo.Invoke(null, new object[] { group, (int)buildCompression });
            }
            catch(Exception e)
            {
                Debug.Log($"[BuildAction] Couldn't set build compression:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private void SetKeyData()
        {
            PlayerSettings.Android.keystorePass = keyStorePassword;
            PlayerSettings.Android.keyaliasPass = keyAliasPassword;
            PlayerSettings.Android.keystoreName = Path.Combine(Directory.GetCurrentDirectory(), keyStoreRelativePath);
            PlayerSettings.SplashScreen.showUnityLogo = false;
        }

        public override void Revert()
        {
            
        }
    }
}