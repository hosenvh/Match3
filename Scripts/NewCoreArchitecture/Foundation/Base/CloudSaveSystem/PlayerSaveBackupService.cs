using System;
using Match3.CloudSave;
using Match3.Foundation.Base.ServiceLocating;
using UnityEngine;


public enum BackupRestoreRequestStatus
{
    Successful,
    BackupVersionNotAvailable,
    BackupForAppVersionNotAvailable,
    BackupNumberNotAvailable
}

public class PlayerSaveBackupService : Service
{

    private const string PlayerSaveBackUpKey = "playerSaveBackup_";
    private const string BackupVersionKey = "BackUpVersion";
    private const string AppVersionKey = "AppVersion";
    private const string BackupNumberKey = "BackupNumber_";
    private const string BackupNumberForAppVersionKey = "BackupNumberForAppVersion";
    
    public int LastBackupVersion
    {
        get => PlayerPrefs.GetInt("PlayerSaveBackupVersion", 0);
        private set => PlayerPrefs.SetInt("PlayerSaveBackupVersion", value);
    }
    
    
    private readonly ICloudDataHandler[] dataHandlers;
    private readonly JsonCloudDataStorage dataStorage = new JsonCloudDataStorage(); 
    
    
    
    public PlayerSaveBackupService(params ICloudDataHandler[] dataHandlers)
    {
        this.dataHandlers = dataHandlers;
    }


    
    public bool IsBackupVersionAvailable(int version)
    {
        var lastBackupVersion = LastBackupVersion;
        if (lastBackupVersion == 0 || version > lastBackupVersion) return false;
        return PlayerPrefs.HasKey(PlayerSaveBackUpKey + version);
    }


    public int GetBackupVersionForAppVersion(string appVersion, int backupNumber)
    {
        return PlayerPrefs.GetInt($"BackUpVersionForAppVersion_{appVersion}_{backupNumber}", 0);
    }

    private void SetBackupVersionForAppVersion(string appVersion, int backupNumber, int backupVersion)
    {
        PlayerPrefs.SetInt($"BackUpVersionForAppVersion_{appVersion}_{backupNumber}", backupVersion);
    }
    
    public int GetLastBackupNumberForAppVersion(string appVersion)
    {
        return PlayerPrefs.GetInt(BackupNumberKey + appVersion, 0);
    }

    private void SetBackupNumberForAppAversion(string appVersion, int backupNumber)
    {
        PlayerPrefs.SetInt(BackupNumberKey+appVersion, backupNumber);
    }
    
    
    public void StoreNewBackup()
    {

        dataStorage.Clear();
        
        foreach (var dataHandler in dataHandlers)
        {
            dataHandler.CollectData(dataStorage);
        }

        var lastBackupNumberForCurrentVersion = GetLastBackupNumberForAppVersion(Application.version);
        lastBackupNumberForCurrentVersion++;

        dataStorage.SetInt(BackupVersionKey, ++LastBackupVersion);
        dataStorage.SetInt(BackupNumberForAppVersionKey, lastBackupNumberForCurrentVersion);
        dataStorage.SetString(AppVersionKey, Application.version);
        
        PlayerPrefs.SetString(PlayerSaveBackUpKey+LastBackupVersion, dataStorage.SerializeData());
        
        SetBackupNumberForAppAversion(Application.version, lastBackupNumberForCurrentVersion);
        SetBackupVersionForAppVersion(Application.version, lastBackupNumberForCurrentVersion, LastBackupVersion);
    }
    

    public void RestoreBackup(int backupVersion, Action<BackupRestoreRequestStatus> onRestoreDone)
    {
        if (!IsBackupVersionAvailable(backupVersion))
        {
            onRestoreDone(BackupRestoreRequestStatus.BackupVersionNotAvailable);
            return;
        }

        var serializedDataString = PlayerPrefs.GetString(PlayerSaveBackUpKey + backupVersion, "");
        dataStorage.Clear();
        dataStorage.DeserializeData(serializedDataString);

        foreach (var dataHandler in dataHandlers)
        {
            dataHandler.SpreadData(dataStorage);
        }

        onRestoreDone(BackupRestoreRequestStatus.Successful);
    }

    
    
    public void RestoreLastBackup(Action<BackupRestoreRequestStatus> onRestoreDone)
    {
        RestoreBackup(LastBackupVersion, onRestoreDone);
    }


    public void RestoreBackupWithAppVersion(string appVersion, int backupNumber, Action<BackupRestoreRequestStatus> onBackupDone)
    {
        var lastBackupNumber = GetLastBackupNumberForAppVersion(appVersion);
        if (backupNumber > lastBackupNumber)
        {
            if (lastBackupNumber == 0)
                onBackupDone(BackupRestoreRequestStatus.BackupForAppVersionNotAvailable);
            else
                onBackupDone(BackupRestoreRequestStatus.BackupNumberNotAvailable);
            
            return;
        }

        var backupVersion = GetBackupVersionForAppVersion(appVersion, backupNumber);
        RestoreBackup(backupVersion, onBackupDone);
    }
    
}
