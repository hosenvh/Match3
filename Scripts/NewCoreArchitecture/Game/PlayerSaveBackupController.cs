using System.Collections;
using System.Collections.Generic;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;

public class PlayerSaveBackupController : MonoBehaviour
{

    private bool IsBackupForCurrentVersionAvailable
    {
        get => PlayerPrefs.GetInt("BackupTookForVersion_"+Application.version, 0) == 1;
        set => PlayerPrefs.SetInt("BackupTookForVersion_" + Application.version, value ? 1 : 0);
    }
    
    IEnumerator Start()
    {
        // waiting for map items to get registered
        yield return null;
        
        if (!IsBackupForCurrentVersionAvailable)
        {
            ServiceLocator.Find<PlayerSaveBackupService>().StoreNewBackup();
            IsBackupForCurrentVersionAvailable = true;
        }
    }

}
