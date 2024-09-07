using System;

namespace Match3.MedrickCloudSave
{
    [Serializable]
    public class MedrickCloudSaveCredentials
    {
        public string access;
        public string refresh;
    }

    [Serializable]
    public class MedrickLoginInfo
    {
        public string username;
        public string password;
    }
}