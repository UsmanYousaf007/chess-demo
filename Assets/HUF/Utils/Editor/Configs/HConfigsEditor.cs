using System;
using System.Collections.Generic;
using HUF.Utils.Assets.Editor;
using HUF.Utils.Extensions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace HUF.Utils.Configs.API.Editor
{
    public static class HConfigsEditor
    {
        static bool InstallersEnabled { get; set; } = true;
        static readonly List<ConfigInstallerData> dataToImport = new List<ConfigInstallerData>();

        [InitializeOnLoadMethod]
        static void Initialize()
        {
            AssetDatabase.importPackageCompleted -= ImportCompletedCallback;
            AssetDatabase.importPackageCompleted += ImportCompletedCallback;
        }

        static void ImportCompletedCallback(string packageName)
        {
            foreach (var configData in dataToImport)
            {
                TryInstallConfig(configData);
            }
            
            dataToImport.Clear();
            InstallersEnabled = true;
        }
        
        /// <summary>
        /// Look for config in given type. <para />
        /// If can't find any it gives option to create new config and open it in hierarchy window. <para />
        /// If find 1 config it gives option to open it in hierarchy window. <para />
        /// If there are more than 1 config it shows warning dialog. <para />
        /// </summary>
        /// <param name="featureName">Feature name visible in dialog box, e.g. UnityAds, Appsflyer, etc.</param>
        /// <param name="configType">Type of target abstract config to install</param>
        /// <param name="folderStructure">Where to put generated config. 
        /// If not set will be put under AssetsUtils.RESOURCES_FOLDER/HConfigs.CONFIGS_FOLDER by default</param>
        [PublicAPI]
        public static void AddConfigToInstallation(string featureName, Type configType, string folderStructure)
        {
            dataToImport.Add(new ConfigInstallerData
            {
                featureName = featureName,
                configType = configType,
                folderStructure = folderStructure
            });
        }

        static void TryInstallConfig(ConfigInstallerData data)
        {
            if (!InstallersEnabled)
                return;

            var configsGuids = AssetDatabase.FindAssets($"t: {data.configType.Name}");
            if (configsGuids.Length == 0)
            {
                ShowCreateConfigDialog(data);
            }
            else if (configsGuids.Length > 1)
            {
                ShowTooManyConfigsDialog(data);
            }
        }

        static void ShowCreateConfigDialog(ConfigInstallerData data)
        {
            var dialogStatus = EditorUtility.DisplayDialogComplex(
                "Create new config",
                $"Do you want to create and setup new config for {data.featureName} feature?",
                "Yes", "No", "Don't ask for others");
            ProcessDialog(dialogStatus, () => { CreateConfig(data.configType, data.folderStructure); }, () => { },
                () => InstallersEnabled = false);
        }

        static void ShowTooManyConfigsDialog(ConfigInstallerData data)
        {
            EditorUtility.DisplayDialog(
                $"Wrong {data.featureName} configuration",
                $"Found too many instances of {data.configType.Name} inside Resources. " +
                "Only one config per feature is allowed.",
                "Ok");
        }

        static void ProcessDialog(int dialogStatus, params UnityAction[] actions)
        {
            if (actions.Length > dialogStatus)
            {
                actions[dialogStatus].Dispatch();
            }
            else
            {
                Debug.LogError($"[{typeof(HConfigsEditor).Name}] Can't process chosen action");
            }
        }
        
        static void CreateConfig(Type configType, string folderStructure)
        {
            var defaultFolderStructure = $"{AssetsUtils.RESOURCES_FOLDER}/{HConfigs.CONFIGS_FOLDER}";
            var targetFolderStructure = folderStructure ?? defaultFolderStructure;
            AssetsUtils.CreateFolderStructure(targetFolderStructure);
            var config = ScriptableObject.CreateInstance(configType);
            
            var path = $"{AssetsUtils.ASSETS_ROOT}/{targetFolderStructure}/{configType.Name}.asset";
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            SetObjectActiveInInspector(config);
        }
        
        static void SetObjectActiveInInspector(UnityEngine.Object config)
        {
            if (config == null)
                return;
            
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(config);
            Selection.activeObject = config;
        }
    }
}