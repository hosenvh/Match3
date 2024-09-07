using System.Collections.Generic;
using Match3.Data.SocialAlbum;
using Match3.Data.Unity.PersistentTypes;
using Match3.Utility;
using UnityEngine;



namespace Match3.Game.SocialAlbum
{

    public class SocialAlbumController : MonoBehaviour
    {
        private const string SocialAlbumUnlockedPostDataKey = "SocialAlbumPost_Unlocked";
        
        public SocialAlbumPostsDataBase albumPostsDatabase;

        private PersistentBool IsPassedScenarioSocialAlbumPostsUnlocked;


        private void Start()
        {
            IsPassedScenarioSocialAlbumPostsUnlocked = new PersistentBool("IsPassedScenarioSocialAlbumPostsUnlocked");
            if (!IsPassedScenarioSocialAlbumPostsUnlocked.Get())
            {
                UnlockPassedScenariosSocialPosts();
                IsPassedScenarioSocialAlbumPostsUnlocked.Set(true);
            }
        }
        
        
        public void UnlockPost(string postID)
        {
            if (albumPostsDatabase.HasPostWithId(postID))
            {
                SaveUnlockFlag(postID);
                return;
            }

            Debug.LogError($"Post with ID {postID} to unlock has not found!");
        }

        
        public SocialAlbumPost[] GetAllPosts()
        {
            return albumPostsDatabase.albumPosts.ToArray();
        }
        
        public SocialAlbumPost[] GetUnlockedPosts()
        {
            List<SocialAlbumPost> unlockedPosts = new List<SocialAlbumPost>();
            foreach (var post in albumPostsDatabase.albumPosts)
            {
                if(post.isDefaultUnlocked || IsPostUnlocked(post.postId))
                    unlockedPosts.Add(post);
            }

            return unlockedPosts.ToArray();
        }

        public SocialAlbumPost TryGetPost(string postId)
        {
            return albumPostsDatabase.TryGetPostWithId(postId);
        }
        
        private void SaveUnlockFlag(string postId)
        {
            PlayerPrefsEx.SetBoolean(GetPostUnlockDataId(postId), true);
        }

        private bool IsPostUnlocked(string postId)
        {
            return PlayerPrefsEx.GetBoolean(GetPostUnlockDataId(postId), false);
        }

        private string GetPostUnlockDataId(string postId)
        {
            return $"{SocialAlbumUnlockedPostDataKey}_{postId}";
        }


        private void UnlockPassedScenariosSocialPosts()
        {
            var gameManager = Base.gameManager;
            var taskManager = gameManager.taskManager;
            HashSet<TaskConfig> allDoneTasks = new HashSet<TaskConfig>();
            taskManager.GetAllDoneTasks(ref allDoneTasks);
            foreach (var doneTask in allDoneTasks)
            {
                var scenarioItems = taskManager.GetScenarioItems(doneTask);
                foreach (var scenarioItem in scenarioItems)
                {
                    if(scenarioItem.scenarioType == ScenarioType.SocialAlbumPost)
                        UnlockPost(scenarioItem.string_0);
                }
            }
        }
        
        
    }


}