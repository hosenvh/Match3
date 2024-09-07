using System;
using NiceJson;
using SeganX;
using UnityEngine;


namespace Match3.CloudSave
{

    public class JsonCloudDataStorage : ICloudDataStorage
    {

        private const string INT = "int";
        private const string FLOAT = "float";
        private const string STRING = "string";
        
        private JsonObject jsonObject;
        
        public JsonCloudDataStorage()
        {
            jsonObject = new JsonObject();
        }
        
        
        public void SetInt(string key, int value)
        {
            jsonObject.Add(key, new JsonObject {{"value", value}, {"type", "int"}});
        }

        public void SetFloat(string key, float value)
        {
            jsonObject.Add(key, new JsonObject {{"value", value}, {"type", "float"}});
        }

        public void SetString(string key, string value)
        {
            jsonObject.Add(key, new JsonObject {{"value", value}, {"type", "string"}});
        }

        
        public int GetInt(string key)
        {
            if (JsonObjectHasValue(jsonObject, key, INT))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }
            
            throw new ExpectedCloudDataNotExistsException(key);
        }

        public int GetInt(string key, int defaultValue)
        {
            if (JsonObjectHasValue(jsonObject, key, INT))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }

            return defaultValue;
        }


        public float GetFloat(string key)
        {
            if (JsonObjectHasValue(jsonObject, key, FLOAT))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }
            
            throw new ExpectedCloudDataNotExistsException(key);
        }

        public float GetFloat(string key, float defaultValue)
        {
            if (JsonObjectHasValue(jsonObject, key, FLOAT))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }

            return defaultValue;
        }


        public string GetString(string key)
        {
            if (JsonObjectHasValue(jsonObject, key, STRING))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }
            
            throw new ExpectedCloudDataNotExistsException(key);
        }

        public string GetString(string key, string defaultString)
        {
            if (JsonObjectHasValue(jsonObject, key, STRING))
            {
                return (JsonBasic) jsonObject[key]["value"];
            }

            return defaultString;
        }


        public void Clear()
        {
            jsonObject?.Clear();
        }

        public string SerializeData()
        {
            return jsonObject != null ? jsonObject.ToJsonString() : "";
        }

        public void DeserializeData(string data)
        {
            jsonObject = (JsonObject) JsonNode.ParseJsonString(data);
            if(jsonObject==null) 
                jsonObject = new JsonObject();
        }
        
        
        private bool JsonObjectHasValue(JsonObject jObject, string key, string type)
        {
            return jObject!=null && jObject.ContainsKey(key) && jObject[key]["type"].ToString().Equals(type);
        }
        
        
    }

}


