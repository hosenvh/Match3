using Match3.CloudSave;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


public class AccountTabController : MonoBehaviour
{
    [SerializeField] private GameObject googleSaveHandlerPage;
    [SerializeField] private GameObject medrickSaveHandlerPage;

    private void Start()
    {
        ActivateSavePageBasedOnCloudSaveImplementationType();
    }

    private void ActivateSavePageBasedOnCloudSaveImplementationType()
    {
        googleSaveHandlerPage.SetActive(false);
        medrickSaveHandlerPage.SetActive(false);

        var cloudSaveImplementationType = ServiceLocator.Find<CloudSaveService>().GetCloudSaveImplementationType();
        if(cloudSaveImplementationType == typeof(GoogleCloudSaveImplementationController))
            googleSaveHandlerPage.SetActive(true);
        else
            medrickSaveHandlerPage.SetActive(true);
    }
}
