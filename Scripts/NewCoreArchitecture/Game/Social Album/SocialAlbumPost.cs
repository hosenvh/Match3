using System;
using System.Collections.Generic;
using I2.Loc;
using Match3.CharacterManagement.Character.Base;


namespace Match3.Data.SocialAlbum
{

    [Serializable]
    public class SocialAlbumPost
    {
        public string postId;
        public bool isDefaultUnlocked = false;
        public CharacterName postWriter;
        public LocalizedSpriteTerm picture;
        public LocalizedStringTerm description;
        public List<SocialAlbumComment> comments = new List<SocialAlbumComment>();
        public int likes;
    }

    [Serializable]
    public class SocialAlbumComment
    {
        public CharacterName person;
        public LocalizedStringTerm comment;
    }

}


