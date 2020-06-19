using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor
{
    public class LibrariesImporter : AssetPostprocessor
    {
        const string VERSIONS_PATTERN = "^Assets\\/HUF\\/(.+)\\/version.txt$";
        
        const string LIBRARY_PATTERN_PREFIX = "Assets/HUF/{0}/HUF_{0}-";
        const string RELEASE_LIBRARY_PATTERN = LIBRARY_PATTERN_PREFIX + "Release.dll";
        const string EDITOR_LIBRARY_PATTERN = LIBRARY_PATTERN_PREFIX + "Editor.dll";
        
        static readonly HashSet<string> importedLibraries = new HashSet<string>();

        [InitializeOnLoadMethod]
        static void Init()
        {
            var libraries = GetInstalledLibrariesNames();
            foreach (var libraryName in libraries)
            {
                importedLibraries.Add(libraryName);
            }
        }
        
        static IEnumerable<string> GetInstalledLibrariesNames()
        {
            var assetsGUIDs = AssetDatabase.FindAssets("version");
            var assetsPaths = assetsGUIDs.Select(AssetDatabase.GUIDToAssetPath).ToList();
            var librariesNames = assetsPaths
                .Where(q => Regex.IsMatch(q, VERSIONS_PATTERN, RegexOptions.Compiled))
                .Select(q => Trim(q, "Assets/HUF/", "/version.txt"))
                .ToList();
            return librariesNames;
        }

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromPaths)
        {
            TryReimportAllLibraries();
        }

        [MenuItem("HUF/Tools/Reimport All HUF Libraries")]
        static void ForceReimportAllLibraries()
        {
            importedLibraries.Clear();
            TryReimportAllLibraries();
        }

        static void TryReimportAllLibraries()
        {
            var importedAnyLibrary = false;
            var libraries = GetInstalledLibrariesNames();
            foreach (var libraryName in libraries)
            {
                if (!importedLibraries.Contains(libraryName))
                {
                    importedAnyLibrary |= TryReimportLibrary(libraryName, RELEASE_LIBRARY_PATTERN);
                    importedAnyLibrary |= TryReimportLibrary(libraryName, EDITOR_LIBRARY_PATTERN);
                    importedLibraries.Add(libraryName);
                }
            }

            if (importedAnyLibrary)
            {
                AssetDatabase.Refresh();
            }
        }
        
        static bool TryReimportLibrary(string libraryName, string pathPattern)
        {
            var path = GetLibraryPath(libraryName, pathPattern);
            if (AssetExists(path))
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                return true;
            }
            return false;
        }
        
        static bool AssetExists(string path)
        {
            return File.Exists($"{Application.dataPath}/{TrimStart(path, "Assets/")}");
        }

        static string GetLibraryPath(string libraryName, string pathPattern)
        {
            return string.Format(pathPattern, libraryName);
        }
        
        static string TrimStart(string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) 
                return target;

            var result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }
        
        static string TrimEnd(string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) 
                return target;

            var result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        static string Trim(string target, string trimStart, string trimEnd)
        {
            var result = target;
            result = TrimStart(result, trimStart);
            result = TrimEnd(result, trimEnd);
            return result;
        }
    }
}