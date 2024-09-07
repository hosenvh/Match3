using System;
using Match3.Data.SocialAlbum;
using Match3.Presentation.TextAdapting;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.SocialAlbum
{

    public class SocialAlbumPostPresenter : MonoBehaviour
    {
        public Image postPictureImage;
        public TextAdapter likeCountText;
        public TextAdapter commentCountText;

        private SocialAlbumPost myPost;
        private Action<SocialAlbumPost> onPostClicked;
        
        
        public void Setup(SocialAlbumPost post, Action<SocialAlbumPost> onPostClicked)
        {
            myPost = post;
            this.onPostClicked = onPostClicked;
            
            postPictureImage.sprite = post.picture.Load();
            likeCountText.SetText(post.likes.ToString());
            commentCountText.SetText(post.comments.Count.ToString());
        }

        public void OnPostClicked()
        {
            onPostClicked(myPost);
        }
        
    }
    
}


