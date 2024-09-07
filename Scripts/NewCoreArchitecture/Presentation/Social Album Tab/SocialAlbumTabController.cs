using System;
using System.Collections.Generic;
using Match3.CharacterManagement.Character.Base;
using Match3.Data.SocialAlbum;
using Match3.Presentation.TextAdapting;
using PandasCanPlay.HexaWord.Utility;
using UnityEngine;
using UnityEngine.UI;


namespace Match3.Presentation.SocialAlbum
{

    public class SocialAlbumTabController : MonoBehaviour
    {
        public SocialAlbumPostPresenter postPresenter;
        
        [Space(8)]
        public GameObject albumSectionObject;
        public GameObject albumEmptyDescriptionObject;
        public Transform albumContentTransform;
        
        [Space(8)]
        public GameObject postPageSectionObject;
        public Image postPictureImage;
        public Image postWriterPicImage;
        public TextAdapter descriptionText;
        public Text likeCountText;
        public SocialAlbumPostCommentPresenter[] commentPresenters;

        [Space(8)] 
        [ArrayElementTitle(nameof(ProfilePicture.person))]
        public ProfilePicture[] profilePictures;
        
        
        private Dictionary<CharacterName, Sprite> profilePicturesDic = new Dictionary<CharacterName, Sprite>();
        private bool isAlbumPostsCreated = false;


        
        private void Start()
        {
            foreach (var profilePicture in profilePictures)
            {
                profilePicturesDic.Add(profilePicture.person, profilePicture.picture);
            }
        }


        public void OpenAlbum()
        {
            postPageSectionObject.SetActive(false);
            albumSectionObject.SetActive(true);

            if (isAlbumPostsCreated) return;
            isAlbumPostsCreated = true;

            var posts = Base.gameManager.socialAlbumController.GetUnlockedPosts();

            if (DoesNotAnyPostExist())
            {
                albumEmptyDescriptionObject.SetActive(true);
                return;
            }

            foreach (var post in posts)
            {
                Instantiate(postPresenter, albumContentTransform).Setup(post, OpenPost);
            }
            
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(albumContentTransform.AsRectTransform());

            bool DoesNotAnyPostExist() => posts.Length == 0;
        }

        private void OpenPost(SocialAlbumPost post)
        {
            postPageSectionObject.SetActive(true);
            albumSectionObject.SetActive(false);

            postPictureImage.sprite = post.picture.Load();
            postWriterPicImage.sprite = TryGetProfilePicture(post.postWriter);
            descriptionText.SetText(post.description.ToString());
            likeCountText.SetText(post.likes.ToString());
            DeactivateAllCommentPresenters();
            LoadPostComments();

            void DeactivateAllCommentPresenters()
            {
                for (int i = 0; i < commentPresenters.Length; i++)
                    commentPresenters[i].commentSectionObject.SetActive(false);
            }

            void LoadPostComments()
            {
                for (int i = 0; i < post.comments.Count; i++)
                {
                    commentPresenters[i].commentSectionObject.SetActive(true);
                    commentPresenters[i].commentText.SetText(post.comments[i].comment);
                    commentPresenters[i].commentWriterPicImage.sprite = TryGetProfilePicture(post.comments[i].person);
                }
            }
        }

        public void ClosePost()
        {
            postPageSectionObject.SetActive(false);
            albumSectionObject.SetActive(true);
        }

        private Sprite TryGetProfilePicture(CharacterName person)
        {
            return profilePicturesDic.ContainsKey(person) ? profilePicturesDic[person] : null;
        }
        
        [Serializable]
        public class SocialAlbumPostCommentPresenter
        {
            public GameObject commentSectionObject;
            public TextAdapter commentText;
            public Image commentWriterPicImage;
        }

        [Serializable]
        public class ProfilePicture
        {
            public CharacterName person;
            public Sprite picture;
        }
    }

}