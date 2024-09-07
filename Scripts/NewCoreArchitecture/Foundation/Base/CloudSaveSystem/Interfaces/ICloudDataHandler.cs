
namespace Match3.CloudSave
{

    public interface ICloudDataHandler 
    {
        void CollectData(ICloudDataStorage cloudStorage);
        void SpreadData(ICloudDataStorage cloudStorage);
    }

}


