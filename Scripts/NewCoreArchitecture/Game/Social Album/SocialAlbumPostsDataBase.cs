using System.Collections.Generic;
using System.Linq;
using UnityEngine;



namespace Match3.Data.SocialAlbum
{

    [CreateAssetMenu(menuName = "SocialAlbum/SocialAlbumPosts", fileName = "SocialAlbumPostsDatabase")]
    public class SocialAlbumPostsDataBase : ScriptableObject
    {
        public List<SocialAlbumPost> albumPosts;

        public bool HasPostWithId(string id)
        {
            return albumPosts.Any(post => post.postId == id);
        }

        public SocialAlbumPost TryGetPostWithId(string id)
        {
            foreach (var post in albumPosts)
            {
                if (post.postId == id)
                    return post;
            }

            return null;
        }
    }

}