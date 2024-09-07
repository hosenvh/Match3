using System;

namespace Match3.MedrickCloudSave
{
    [Serializable]
    public class MedrickCloudSaveServerPlayerData
    {
        public int id;
        public MedrickCloudSaveCredentials credentials;
        public MedrickLoginInfo login_info;
        public string auth_data;
        public string save_data;
    }
}