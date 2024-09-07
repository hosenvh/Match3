using I2.Loc;
using Match3.CharacterManagement.Character.Base;
using Match3.Data.SocialAlbum;
using SeganX;
using UnityEditor;
using UnityEngine;



namespace Match3.EditorTools.Editor.CustomInspectors
{
    [CustomEditor(typeof(SocialAlbumPostsDataBase))]
    public class SocialAlbumPostsDatabaseEditor : UnityEditor.Editor
    {
        private const int FixedLabelWidth = 70;
        private const int MaxPostComment = 3;
        
        private SocialAlbumPostsDataBase myTarget;
        private string newPostId;

        private LanguageSourceAsset languageSource;
        private GUIContent postIdTextContent = new GUIContent("Post Id:");

        private void OnEnable()
        {
            myTarget = (SocialAlbumPostsDataBase) target;
            languageSource = Resources.Load<LanguageSourceAsset>(LocalizationManager.SocialAlbumLanguageSource);
        }

        private void OnDisable()
        {
            languageSource = null;
            Resources.UnloadUnusedAssets();
        }

        
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var posts = myTarget.albumPosts;
            for (int i = 0; i < posts.Count; i++)
            {
                DrawPost(posts[i], i);
                
                if (i < posts.Count-1)
                    DrawSeparator();
                else
                    GUILayout.Space(20);
            }

            DrawAddPostSection();
            
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(myTarget);
            EditorApplication.update.Invoke();
        }

        private void DrawPost(SocialAlbumPost post, int postIndex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(postIdTextContent, GUILayout.Width(FixedLabelWidth));
            GUILayout.Label(post.postId, GetLabelCustomStyle(TextAnchor.MiddleLeft, new Color(0.28f, 0.71f, 1f)));
            GUILayout.EndHorizontal();
            
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Is Unlocked", GUILayout.Width(FixedLabelWidth));
            post.isDefaultUnlocked = GUILayout.Toggle(post.isDefaultUnlocked, GUIContent.none);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Post Writer", GUILayout.Width(FixedLabelWidth));
            post.postWriter = (CharacterName) EditorGUILayout.EnumPopup(GUIContent.none, post.postWriter);
            GUILayout.EndHorizontal();
            
            var pictureProperty = serializedObject.FindProperty("albumPosts").GetArrayElementAtIndex(postIndex)
                .FindPropertyRelative("picture");
            EditorGUIUtility.labelWidth = FixedLabelWidth;
            EditorGUILayout.PropertyField(pictureProperty);
            
            GUILayout.Space(6);
            
            GUILayout.Label("Description:");
            DrawLocalizedString(post.description.mTerm);

            GUILayout.Space(6);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Likes:", GUILayout.Width(FixedLabelWidth));
            post.likes = EditorGUILayout.IntField(post.likes);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            
            DrawPostCommentsSection(post);
            
            if (ColoredButton(Color.red, "Remove Post"))
            {
                RemoveTerm(post.description.mTerm);
                foreach (var comment in post.comments)
                {
                    RemoveTerm(comment.comment.mTerm);
                }
                myTarget.albumPosts.RemoveAt(postIndex);
            }

        }

        void DrawSeparator()
        {
            GUILayout.Space(5);
            GUILayout.Label("-------------------------------------------------------",
                GetLabelCustomStyle(TextAnchor.MiddleCenter, Color.magenta));
            GUILayout.Space(5);
        }
        
        private void DrawPostCommentsSection(SocialAlbumPost post)
        {
            GUILayout.Box("Comments", GetBoxCustomStyle(new Color(1f, 0.53f, 0.25f)));

            for (int i = 0; i < post.comments.Count; ++i)
            {
                var comment = post.comments[i];
                
                GUILayout.BeginHorizontal();
                if (ColoredButton(new Color(0.78f, 0.24f, 0.11f, 0.69f),  "Remove Comment"))
                {
                    RemoveTerm(comment.comment.mTerm);
                    post.comments.RemoveAt(i);
                    RebuildPostCommentsTerms(post);
                    comment = null;
                }
                
                if (comment != null)
                    comment.person = (CharacterName) EditorGUILayout.EnumPopup("Person", comment.person);

                GUILayout.EndHorizontal();
                
                if (comment != null)
                    DrawLocalizedString(comment.comment.mTerm);

                if(i < post.comments.Count - 1)
                    GUILayout.Space(4);
            }  
            
            GUILayout.BeginHorizontal();
            if (post.comments.Count < MaxPostComment)
            {
                if (ColoredButton(new Color(0.32f, 0.7f, 0.35f, 0.81f), "Add Comment")) // GUILayout.Button("Add Comment"))
                {
                    var commentTerm = BuildCommentTerm(post.postId, post.comments.Count);
                    post.comments.Add(new SocialAlbumComment() {comment = commentTerm.Term});
                }
            }
            GUILayout.EndHorizontal();
        }

        private void DrawAddPostSection()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(postIdTextContent, GUILayout.Width(FixedLabelWidth));
            newPostId = GUILayout.TextField(newPostId);
            GUILayout.EndHorizontal();
            if (ColoredButton(Color.green, "Create Post"))
            {
                if (!newPostId.IsNullOrEmpty() && !myTarget.HasPostWithId(newPostId))
                {
                    var newPost = new SocialAlbumPost() {postId = newPostId};
                    var newTerm = BuildDescriptionTerm(newPostId);
                    newPost.description.mTerm = newTerm.Term;
                    myTarget.albumPosts.Add(newPost);
                    newPostId = "";
                }
                else
                {
                    Debug.LogError("Can't add post with empty or duplicate ID");
                }
            }
        }

        void DrawLocalizedString(string term)
        {
            GUILayout.BeginHorizontal();
            var termData = GetTermData(term);
            var text = GUILayout.TextArea(GetPersianTranslation(termData));
            UpdatePersianTranslation(termData, text);
            GUILayout.Label(text.Persian(), GetLabelCustomStyle(TextAnchor.MiddleCenter, Color.yellow));
            GUILayout.EndHorizontal();
        }
        
        private GUIStyle GetLabelCustomStyle(TextAnchor alignment, Color textColor)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = alignment;
            style.normal.textColor = textColor;
            return style;
        }

        private GUIStyle GetBoxCustomStyle(Color textColor)
        {
            var style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = textColor;
            return style;
        }
        
        private bool ColoredButton(Color color, string buttonText)
        {
            var defColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            var buttonResult = GUILayout.Button(buttonText);
            GUI.backgroundColor = defColor;
            return buttonResult;
        }

        private void RebuildPostCommentsTerms(SocialAlbumPost post)
        {
            for (int i = 0; i < post.comments.Count; ++i)
            {
                var comment = post.comments[i];
                var commentTermData = GetTermData(comment.comment.mTerm);
                var clonedTermData = commentTermData.DeepClone();
                RemoveTerm(commentTermData.Term);
                clonedTermData.Term = GetPostCommentTermName(post.postId, i);
                comment.comment.mTerm = clonedTermData.Term;
                InsertTermDataIntoTheLanguageSource(clonedTermData);
            }
        }

        #region Language Source Utility

        private TermData BuildDescriptionTerm(string postId)
        {
            return BuildTermInSocialAlbumLanguageSource(GetPostDescriptionTermName(postId));
        }

        private TermData BuildCommentTerm(string postId, int commentIndex)
        {
            return BuildTermInSocialAlbumLanguageSource(GetPostCommentTermName(postId, commentIndex));
        }

        private TermData BuildTermInSocialAlbumLanguageSource(string termName)
        {
            var term = languageSource.mSource.AddTerm(termName, eTermType.Text,
                false);
            EditorUtility.SetDirty(languageSource);
            return term;
        }

        private void InsertTermDataIntoTheLanguageSource(TermData termData)
        {
            languageSource.mSource.InsertTerm(termData, false);
            EditorUtility.SetDirty(languageSource);
        }
        
        private TermData GetTermData(string termName)
        {
            var termData = languageSource.mSource.GetTermData(termName);
            if (termData == null)
            {
                termData = BuildTermInSocialAlbumLanguageSource(termName);
                Debug.LogError($"{termName} term was deleted in a wrong way, please check its translations!");
                return termData;
            }

            return termData;
        }
        
        private string GetPersianTranslation(TermData termData)
        {
            return termData.GetTranslation(GetPersianLanguageIndex());
        }
        
        private void UpdatePersianTranslation(TermData termData, string persianTranslation)
        {
            termData.SetTranslation(GetPersianLanguageIndex(), persianTranslation);
            EditorUtility.SetDirty (languageSource);
        }
        
        private void RemoveTerm(string term)
        {
            languageSource.mSource.RemoveTerm(term);
            LocalizationEditor.RemoveParsedTerm(term);
            LocalizationEditor.mSelectedKeys.Remove(term);
            EditorUtility.SetDirty (languageSource);
        }

        private string GetPostDescriptionTermName(string postId)
        {
            return $"SocialAlbum/Descriptions/{postId}_Desc";
        }

        private string GetPostCommentTermName(string postId, int commentIndex)
        {
            return $"SocialAlbum/Comments/{postId}_comment_{commentIndex.ToString()}";
        }
        
        private int GetPersianLanguageIndex()
        {
            return languageSource.mSource.GetLanguageIndex("Persian", true, false);
        }
        
        #endregion
        
        
    }

}