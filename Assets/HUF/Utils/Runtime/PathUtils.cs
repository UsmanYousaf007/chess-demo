using System.IO;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public static class PathUtils
    {
        const string ASSETS = "Assets";
        
        public static string GetFullPath(string path)
        {
            if (path.StartsWith(Application.dataPath))
                return path;

            if (path.StartsWith(ASSETS))
                return Path.Combine(Directory.GetParent(Application.dataPath).FullName, path);

            return Path.Combine(Application.dataPath, path);
        }

        public static string GetLocalPath(string path)
        {
            if (path.StartsWith(Application.dataPath))
                return Path.Combine(ASSETS, path.Remove(0, Application.dataPath.Length + 1));

            if (path.StartsWith(ASSETS))
                return path;

            return Path.Combine(ASSETS, path);
        }
    }
}