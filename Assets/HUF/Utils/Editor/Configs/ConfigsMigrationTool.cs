using System.IO;
using System.Text;
using HUF.Utils.Runtime.Configs.API;
using UnityEditor;
using UnityEngine;

namespace HUF.Utils.Editor.Configs
{
    public static class ConfigsMigrationTool
    {
        [MenuItem("HUF/Tools/Move Configs to HUFConfigs subfolder")]
        static void MoveConfigs()
        {
            var targetFolder = HConfigs.CONFIGS_FOLDER;
                
            string[] configsGuids = AssetDatabase.FindAssets("t: AbstractConfig"); 
            foreach (string guid in configsGuids)
            {
                var configPath = AssetDatabase.GUIDToAssetPath(guid);
                var currentDirectory = Path.GetDirectoryName(configPath);
                var indexToInsertTargetFolder = currentDirectory.IndexOf(AssetsUtils.RESOURCES_FOLDER) +
                                                AssetsUtils.RESOURCES_FOLDER.Length;
                var targetDirectory = currentDirectory.Insert(indexToInsertTargetFolder, $"/{targetFolder}"); 
                var configName = Path.GetFileName(configPath);
                
                if (!currentDirectory.Contains(targetFolder))
                {
                    var dialogStatus = EditorUtility.DisplayDialogComplex(
                        "Move config",
                        $"Move {configName} from '{currentDirectory}' to '{targetDirectory}'?",
                        "Yes", "Cancel", "No");

                    switch (dialogStatus)
                    {
                        case 0:
                            MoveConfig(configPath, Path.Combine(targetDirectory, configName));
                            break;
                        case 1:
                            return;
                    }
                }
            }
        }
        
        static void MoveConfig(string from, string to) 
        {
            string targetFolderStructure = Path.GetDirectoryName(to).Substring(AssetsUtils.ASSETS_ROOT.Length + 1);
            AssetsUtils.CreateFolderStructure(targetFolderStructure);
            
            AssetDatabase.MoveAsset(from, to);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        
        [MenuItem("HUF/Tools/Log all configs paths")]
        static void LogPaths()
        {
            string[] configsGuids = AssetDatabase.FindAssets("t: AbstractConfig");
            
            var paths = new StringBuilder();
            foreach (string guid in configsGuids)
            {
                paths.AppendLine(AssetDatabase.GUIDToAssetPath(guid));
            }

            Debug.Log(paths);
        }
    }
}