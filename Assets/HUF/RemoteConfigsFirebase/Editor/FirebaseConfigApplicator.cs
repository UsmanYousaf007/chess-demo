using System.Collections.Generic;
using HUF.InitFirebase.Runtime.API;
using HUF.RemoteConfigs.Runtime.API;
using HUF.RemoteConfigsFirebase.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HUF.RemoteConfigsFirebase.Editor {
    internal class FirebaseConfigApplicator
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(FirebaseConfigApplicator) );
        enum Step
        {
            InitializeFirebase = 1,
            InitializerRemoteConfig,
            Fetch,
            Apply,
            End
        }

        static readonly Dictionary<Step, string> messages = new Dictionary<Step, string>
        {
            {Step.InitializeFirebase, "Initializing Firebase"},
            {Step.InitializerRemoteConfig, "Initializing Remote Configs"},
            {Step.Fetch, "Fetching Remote Configs"},
            {Step.Apply, "Applying downloaded configs"},
        };

        [MenuItem("HUF/Tools/Apply Configs from Firebase")]
        [PublicAPI]
        public static void PatchLocalConfigs()
        {
            NotifyStep( Step.InitializeFirebase );
            if ( HInitFirebase.IsInitialized )
            {
                HandleFirebaseInit();
            }
            else
            {
                HInitFirebase.Init( HandleFirebaseInit );
            }
        }

        static void NotifyStep( Step step )
        {
            const float TOTAL = (int)Step.End;
            var frac = (int)step;
            EditorUtility.DisplayProgressBar( "Config Patching", messages[step], frac/TOTAL );
        }

        static void HandleFirebaseInit()
        {
            if ( HInitFirebase.IsInitialized )
            {
                NotifyStep( Step.InitializerRemoteConfig );

                HRemoteConfigsFirebase.Init( HandleRemoteConfigsInit );
                return;
            }

            HLog.LogError( logPrefix, "Unable to init Firebase" );

            EditorUtility.ClearProgressBar();
        }

        static void HandleRemoteConfigsInit()
        {
            if ( HRemoteConfigs.IsInitialized )
            {
                NotifyStep( Step.Fetch );
                HRemoteConfigs.OnFetchComplete += HandleFetchSuccess;
                HRemoteConfigs.OnFetchFail += HandleFetchFailure;

                HRemoteConfigs.Fetch();
                return;
            }

            HLog.LogError( logPrefix, "Unable to init Firebase Remote Configs" );
            EditorUtility.ClearProgressBar();
        }

        static void HandleFetchFailure()
        {
            HLog.LogError( logPrefix, "Fetching failed" );
            EditorUtility.ClearProgressBar();
        }

        static void HandleFetchSuccess()
        {
            NotifyStep( Step.Apply );
            OverrideLocalConfigs();
            EditorUtility.ClearProgressBar();
            AssetDatabase.SaveAssets();
            HLog.Log( logPrefix, "Configs downloaded" );
        }

        static void OverrideLocalConfigs()
        {
            var configs = Resources.LoadAll<AbstractConfig>( HConfigs.CONFIGS_FOLDER );

            for ( int i = 0; i < configs.Length; i++ )
                HRemoteConfigs.ApplyConfig( ref configs[i] );
        }
    }
}