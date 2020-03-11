// ReSharper disable CheckNamespace
using System;
using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.API.Data;
using UnityEditor;
using PackageConfig = HUFEXT.PackageManager.Editor.API.Data.PackageConfig;

namespace HUFEXT.PackageManager.Editor.Utils.Helpers
{
    [Serializable]
    public class Scope
    {
        public string name = string.Empty;
    }
    
    [Serializable]
    public class Link
    {
        public string url = string.Empty;
    }

    [Serializable]
    public class Version
    {
        public string version = string.Empty;
    }

    [Serializable]
    public class LatestItemData
    {
        public string name;
        public string scope;
        public string channel;
        public API.Data.PackageConfig config;
        public PackageManifest latestManifest;
    }
    
    [Serializable]
    public class Wrapper<T>
    {
        public List<T> Items = new List<T>();

        public void FromJson( string json )
        {
            EditorJsonUtility.FromJsonOverwrite( "{ \"Items\": " + json + "}", this );
        }

        public string ToJson()
        {
            return EditorJsonUtility.ToJson( this );
        }
    }
    
    public static class JsonHelper
    {
        public static T FromArray<T>( string json ) where T : new()
        {
            var items = new T();
            EditorJsonUtility.FromJsonOverwrite( "{ \"Items\": " + json + "}", items );
            return items;
        }
    }

    internal class PackageUpdateList : Wrapper<PackageManifest> {}
    
    [Serializable]
    public class ScopeList : Wrapper<Scope> {}

    [Serializable]
    public class ConfigList : Wrapper<API.Data.PackageConfig> {}

    [Serializable]
    public class ManifestList : Wrapper<PackageManifest> {}

    [Serializable]
    public class VersionList : Wrapper<Version> {}
    
    [Serializable]
    public class LatestItemsList : Wrapper<LatestItemData> {}
}
