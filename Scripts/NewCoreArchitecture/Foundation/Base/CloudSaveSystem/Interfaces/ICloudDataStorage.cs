
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.CloudSave
{
    public interface ICloudDataStorage
    {
        void SetInt(string key, int value);
        void SetFloat(string key, float value);
        void SetString(string key, string value);

        
        int GetInt(string key);
        int GetInt(string key, int defaultValue);
        float GetFloat(string key);
        float GetFloat(string key, float defaultValue);
        string GetString(string key);
        string GetString(string key, string defaultString);

        
        void Clear();
        
        string SerializeData();
        void DeserializeData(string data);
    }

}