using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime
{
    public static class PathUtils
    {
        const string ASSETS = "Assets";
        
        /// <summary>
        /// Converts a path to a full path.
        /// </summary>
        /// <param name="path">A path to convert.</param>
        /// <returns>A full path</returns>
        [PublicAPI]
        public static string GetFullPath(string path)
        {
            if (path.StartsWith(Application.dataPath))
                return path;

            if (path.StartsWith(ASSETS))
                return Path.Combine(Directory.GetParent(Application.dataPath).FullName, path);

            return Path.Combine(Application.dataPath, path);
        }

        /// <summary>
        /// Converts a path to a local path.
        /// </summary>
        /// <param name="path">A path to convert.</param>
        /// <returns>A local path</returns>
        [PublicAPI]
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