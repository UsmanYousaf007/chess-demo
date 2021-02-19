using System;
using System.IO;
using System.Text;
using HUF.Storage.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using UnityEngine;

namespace HUF.Storage.Runtime.Implementation
{
    public static class StorageUtils
    {
        const string METADATA_PREFIX = "HUFStorageMetadata/";
        
        public static string GetLocalFilePath(string filePath)
        {
            filePath = filePath.Replace("://", "/");
            return new StringBuilder(Application.persistentDataPath).Append('/').Append(filePath).ToString();
        }

        public static string GetLocalDirectoryPath(string filePath)
        {
            return Path.GetDirectoryName(GetLocalFilePath(filePath));
        }

        public static FileStream GetNewLocalFile(string filePath)
        {
            return File.Open(GetLocalFilePath(filePath), FileMode.CreateNew);
        }

        public static FileStream GetLocalFile(string filePath)
        {
            return File.Open(GetLocalFilePath(filePath), FileMode.OpenOrCreate);
        }

        public static string GetDatabaseUrlPath(string databaseUrl, string filePath)
        {
            return new StringBuilder(databaseUrl).Append('/').Append(filePath).ToString();
        }

        public static string GetMetadataPrefsPath(string filePath)
        {
            return new StringBuilder(METADATA_PREFIX).Append(filePath).ToString();
        }
        
        public static bool TrySaveBytes(byte[] bytesToSave, string pathToFile)
        {
            try
            {
                var dirPath = GetLocalDirectoryPath(pathToFile);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                File.WriteAllBytes(GetLocalFilePath(pathToFile), bytesToSave);
            }
            catch (Exception ex)
            {
                Debug.Log($"{StorageErrorMessages.ERROR_DURING_SAVING}: {ex.Message}");
                return false;
            }
            return true;
        }

        public static bool TrySaveObject(object objectToSave,  string pathToFile)
        {
            byte[] byteArray;
            if (objectToSave is byte[])
                byteArray = (byte[])objectToSave;
            else
                byteArray = objectToSave.SerializeToByteArray();
            
            return TrySaveBytes(byteArray, pathToFile);
        }
    }
}