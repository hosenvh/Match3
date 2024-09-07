using System.IO;
using Match3.Development.Base;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "User Cache", priority: 2)]
    public class UserCacheManagementDevOptions : DevelopmentOptionsDefinition
    {
        private static UserProfileManager UserProfileManager => Application.isPlaying ? ServiceLocator.Find<UserProfileManager>() : new UserProfileManager();

        [DevOption(commandName: "Clear PlayerPrefs Key")]
        public static void ClearPlayerPrefsKey(string key)
        {
            Debug.Log($"Going to Delete {key} which having key was: {PlayerPrefs.HasKey(key)}");
            PlayerPrefs.DeleteKey(key);
        }

        [DevOption(commandName: "Clear Data And Close")]
        public static void ClearData(int shouldKeepCurrentUserId)
        {
            ClearAll(shouldKeepUserId: shouldKeepCurrentUserId == 1);
            Application.Quit();
        }

        public static void ClearAll(bool shouldKeepUserId)
        {
            ClearPlayerPrefs(shouldKeepUserId);
            ClearFiles();
        }

        public static void ClearPlayerPrefs(bool shouldKeepUserId)
        {
            string cachedUserId = UserProfileManager.GlobalUserId;
            PlayerPrefs.DeleteAll();
            if (shouldKeepUserId)
                UserManagementDevOptions.SetUserId(cachedUserId);
        }

        public static void ClearFiles()
        {
            Caching.ClearCache();

            var files = Directory.GetFiles(Application.persistentDataPath);
            foreach (var item in files)
                try { File.Delete(item); } catch { }

            var dirs = Directory.GetDirectories(Application.persistentDataPath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); } catch { }

            files = Directory.GetFiles(Application.temporaryCachePath);
            foreach (var item in files)
                try { File.Delete(item); } catch { }

            dirs = Directory.GetDirectories(Application.temporaryCachePath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); } catch { }
        }
    }
}