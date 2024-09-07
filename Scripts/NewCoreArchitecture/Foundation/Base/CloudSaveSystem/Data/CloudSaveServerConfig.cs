using Match3;
using Match3.CloudSave;
using PandasCanPlay.HexaWord.Utility;


namespace CloudSave.Data.configuration
{
    [System.Serializable]
    public class CloudSaveServerConfig
    {
        [Type(typeof(ICloudSaveManager), false)]
        public string cloudSaveManagerType;
        [Type(typeof(ICloudSaveImplementationController), false)]
        public string cloudSaveImplementationControllerType;
    }

    [System.Serializable]
    public class CloudSaveCohortConfig : CohortConfigReplacer<CloudSaveServerConfig>
    {

    }
}


