using Match3.CharacterManagement.CharacterSkin.Overlay;
using Match3.CloudSave;
using Match3.Development.Base.DevelopmentConsole;
using Match3.Foundation.Base.ServiceLocating;
using Match3.UserManagement.Avatar.Overlay;
using UnityEngine;


namespace Match3.Development.DevOptions
{
    [DevOptionGroup(groupName: "Cloud Save", priority: 17)]
    public class CloudServiceDevOptions : DevelopmentOptionsDefinition
    {
        [DevOption(commandName: "Fake Load")]
        public static void FakeLoad(string data)
        {
            ServiceLocator.Find<CloudSaveService>().FakeLoad(data);
        }

        [DevOption(commandName: "Authenticate CloudSave")]
        public static void AuthenticateCloudSave()
        {
            var cloudService = ServiceLocator.Find<CloudSaveService>();
            cloudService.Authenticate(status => { Debug.Log("Cloud Save Authentication Status : " + status); });
        }

        [DevOption(commandName: "Save On Cloud")]
        public static void SaveOnCloud()
        {
            var cloudSaveService = ServiceLocator.Find<CloudSaveService>();
            cloudSaveService.Save(status => { Debug.Log("Cloud Save Status : " + status.ToString()); });
        }

        [DevOption(commandName: "Load from Cloud")]
        public static void LoadFromCloud(string authorizationCode)
        {
            var cloudSaveService = ServiceLocator.Find<CloudSaveService>();
            cloudSaveService.TryResolveAuthenticationAndLoad(authorizationCode,
                status => { Debug.Log($"Cloud Load Status : {status}"); });
        }


        [DevOption(commandName: "Collect Data With Handler")]
        public static void CollectData()
        {
            ICloudDataStorage jsonDataStorage = new JsonCloudDataStorage();

            var characterDataHandler = new CharactersDataHandler();
            var generalDataHandler = new GeneralDataHandler();
            var mapItemDataHandler = new MapItemDataHandler();
            var mapManagerDataHandler = new MapManagerDataHandler();
            var paidMapItemDataHandler = new PaidMapItemDataHandler();
            var taskManagerDataHandler = new TaskManagerDataHandler();
            var userInfoDataHandler = new UserInfoDataHandler();
            var tutorialDataHandler = new TutorialDataHandler();
            var miscDataHandler = new MiscDataHandler();
            var ncDataHandler = new NCDataHandler();
            var seasonPassDataHandler = new SeasonPassDataHandler();
            var characterSkinsDataHandler = new CharacterSkinsCloudDataHandler();
            var userAvatarsDataHandler = new UserAvatarsCloudDataHandler();

            userInfoDataHandler.CollectData(jsonDataStorage);
            generalDataHandler.CollectData(jsonDataStorage);
            characterDataHandler.CollectData(jsonDataStorage);
            paidMapItemDataHandler.CollectData(jsonDataStorage);
            mapItemDataHandler.CollectData(jsonDataStorage);
            taskManagerDataHandler.CollectData(jsonDataStorage);
            mapManagerDataHandler.CollectData(jsonDataStorage);
            tutorialDataHandler.CollectData(jsonDataStorage);
            miscDataHandler.CollectData(jsonDataStorage);
            ncDataHandler.CollectData(jsonDataStorage);
            seasonPassDataHandler.CollectData(jsonDataStorage);
            characterSkinsDataHandler.CollectData(jsonDataStorage);
            userAvatarsDataHandler.CollectData(jsonDataStorage);

            PlayerPrefs.SetString("dataHandler", jsonDataStorage.SerializeData());

            Debug.Log(jsonDataStorage.SerializeData());
        }

        [DevOption(commandName: "Load Collected Data")]
        public static void LoadCollectedData()
        {
            ICloudDataStorage jsonDataStorage = new JsonCloudDataStorage();
            var data = PlayerPrefs.GetString("dataHandler");
            Debug.Log(data);
            jsonDataStorage.DeserializeData(data);

            var userInfoDataHandler = new UserInfoDataHandler();
            var generalDataHandler = new GeneralDataHandler();
            var characterDataHandler = new CharactersDataHandler();
            var paidMapItemDataHandler = new PaidMapItemDataHandler();
            var mapItemDataHandler = new MapItemDataHandler();
            var taskManagerDataHandler = new TaskManagerDataHandler();
            var mapManagerDataHandler = new MapManagerDataHandler();
            var tutorialDataHandler = new TutorialDataHandler();
            var miscDataHandler = new MiscDataHandler();
            var nCDataHandler = new NCDataHandler();
            var seasonPassDataHandler = new SeasonPassDataHandler();
            var characterSkiDataHandler = new CharacterSkinsCloudDataHandler();
            var userAvatarsDataHandler = new UserAvatarsCloudDataHandler();

            userInfoDataHandler.SpreadData(jsonDataStorage);
            generalDataHandler.SpreadData(jsonDataStorage);
            characterDataHandler.SpreadData(jsonDataStorage);
            paidMapItemDataHandler.SpreadData(jsonDataStorage);
            mapItemDataHandler.SpreadData(jsonDataStorage);
            taskManagerDataHandler.SpreadData(jsonDataStorage);
            mapManagerDataHandler.SpreadData(jsonDataStorage);
            tutorialDataHandler.SpreadData(jsonDataStorage);
            miscDataHandler.SpreadData(jsonDataStorage);
            nCDataHandler.SpreadData(jsonDataStorage);
            seasonPassDataHandler.SpreadData(jsonDataStorage);
            characterSkiDataHandler.SpreadData(jsonDataStorage);
            userAvatarsDataHandler.SpreadData(jsonDataStorage);


            Debug.Log("Spreading Done!");
        }

        [DevOption(commandName: "Spread Data With Handler")]
        public static void SpreadData(string data)
        {
            ICloudDataStorage jsonDataStorage = new JsonCloudDataStorage();
            jsonDataStorage.DeserializeData(data);

            var userInfoDataHandler = new UserInfoDataHandler();
            var generalDataHandler = new GeneralDataHandler();
            var characterDataHandler = new CharactersDataHandler();
            var paidMapItemDataHandler = new PaidMapItemDataHandler();
            var mapItemDataHandler = new MapItemDataHandler();
            var taskManagerDataHandler = new TaskManagerDataHandler();
            var mapManagerDataHandler = new MapManagerDataHandler();
            var tutorialDataHandler = new TutorialDataHandler();
            var miscDataHandler = new MiscDataHandler();
            var nCDataHandler = new NCDataHandler();
            var seasonPassDataHandler = new SeasonPassDataHandler();
            var characterSkinsDataHandler = new CharacterSkinsCloudDataHandler();
            var userAvatarsDataHandler = new UserAvatarsCloudDataHandler();

            userInfoDataHandler.SpreadData(jsonDataStorage);
            generalDataHandler.SpreadData(jsonDataStorage);
            characterDataHandler.SpreadData(jsonDataStorage);
            paidMapItemDataHandler.SpreadData(jsonDataStorage);
            mapItemDataHandler.SpreadData(jsonDataStorage);
            taskManagerDataHandler.SpreadData(jsonDataStorage);
            mapManagerDataHandler.SpreadData(jsonDataStorage);
            tutorialDataHandler.SpreadData(jsonDataStorage);
            miscDataHandler.SpreadData(jsonDataStorage);
            nCDataHandler.SpreadData(jsonDataStorage);
            seasonPassDataHandler.SpreadData(jsonDataStorage);
            characterSkinsDataHandler.SpreadData(jsonDataStorage);
            userAvatarsDataHandler.SpreadData(jsonDataStorage);

            Debug.Log("Spreading Done!");
        }
    }
}