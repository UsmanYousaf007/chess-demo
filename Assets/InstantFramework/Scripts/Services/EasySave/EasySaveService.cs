/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-10 10:52:49 UTC+05:00
/// 
/// @description
/// Check out the easy save docs for all its capabilities that can extend this service:
/// http://docs.moodkie.com/product/easy-save-2/

using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public partial class EasySaveService : ILocalDataService
    {
        public ILocalDataWriter OpenWriter(string filename)
        {
            return new LocalDataWriter(filename);
        }

        public ILocalDataReader OpenReader(string filename)
        {
            return new LocalDataReader(filename);
        }

        public bool FileExists(string filename)
        {
            return ES2.Exists(filename);
        }

        public void DeleteFile(string filename)
        {
            if (ES2.Exists(filename))
            {
                ES2.Delete(filename);
            }
        }
    }

    public class LocalDataWriter : ILocalDataWriter
    {
        readonly ES2Writer writer;

        public LocalDataWriter(string filename)
        {
            writer = ES2Writer.Create(filename);
        }

        public void Write<T>(string key, T value)
        {
            writer.Write<T>(value, key);   
        }

        public void WriteList<T>(string key, List<T> value)
        {
            writer.Write(value, key);
        }

        public void WriteDictionary<TKey,TValue>(string key, Dictionary<TKey,TValue> value)
        {
            writer.Write(value, key);
        }

        public void Close()
        {
            writer.Save();
            writer.Dispose();
        }
    }

    public class LocalDataReader : ILocalDataReader
    {
        readonly ES2Data es2Data;

        public LocalDataReader(string filename) 
        {
            es2Data = ES2.LoadAll(filename);
        }

        public T Read<T>(string key)
        {
            if (!es2Data.TagExists(key))
            {
                throw new KeyNotFoundException();
            }

            return es2Data.Load<T>(key);
        }

        public List<T> ReadList<T>(string key)
        {
            if (!es2Data.TagExists(key))
            {
                throw new KeyNotFoundException();
            }

            return es2Data.LoadList<T>(key);
        }

        public Dictionary<TKey,TValue> ReadDictionary<TKey,TValue>(string key)
        {
            if (!es2Data.TagExists(key))
            {
                throw new KeyNotFoundException();
            }

            return es2Data.LoadDictionary<TKey,TValue>(key);
        }

        public bool HasKey(string key)
        {
            return es2Data.TagExists(key);
        }

        public void Close()
        {
            // nothing to do at this time but we will keep this here as
            // a convention in case we ever use a ES2Reader in the future.
        }
    }

    public class KeyNotFoundException : Exception {}
}