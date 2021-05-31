#if HUF_GAMESERVER
using System;
using System.IO;
using HUF.GameServer.Runtime.API;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HUF.PolicyGuard.Runtime.Implementations
{
    internal static class AnonymizationHandler
    {
        const string DEFAULT_BLOCKER_PATH = "PolicyGuard/DefaultAnonymizationBlocker";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void HookEvents()
        {
            HGameServer.OnPlayerAnonymized -= HandleAnonymization;
            HGameServer.OnPlayerAnonymized += HandleAnonymization;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterAssembliesLoaded )]
        static void ConfigurationCheck()
        {
            if ( HConfigs.HasConfig<PolicyGuardConfig>()
                 && HConfigs.GetConfig<PolicyGuardConfig>().anonymizationBlocker != null )
                return;

            HLog.LogError( new HLogPrefix( HPolicyGuard.logPrefix, nameof(AnonymizationHandler) ),
                $"Unable to find {nameof(PolicyGuardConfig.anonymizationBlocker)} in {nameof(PolicyGuardConfig)}" );
        }
#endif

        static void HandleAnonymization()
        {
            GameObject blockerPopup = null;

            ClearScenes();

            CoroutineManager.StopAllCoroutines();
#if HUF_ANALYTICS
            Analytics.Runtime.API.HAnalytics.CollectSensitiveData( false );
#endif
#if HUFEXT_ADS_MANAGER_HADS
            Ads.Runtime.API.HAds.CollectSensitiveData( false );
#endif
            PlayerPrefs.DeleteAll();
            Caching.ClearCache();
            Time.timeScale = 0;

            if ( HConfigs.HasConfig<PolicyGuardConfig>() )
                blockerPopup = HConfigs.GetConfig<PolicyGuardConfig>().anonymizationBlocker;

            if ( blockerPopup != null )
            {
                Object.Instantiate( blockerPopup ).SetActive( true );
            }

            ClearStorage();
        }

        static void ClearScenes()
        {
            var empty = SceneManager.CreateScene( "Limbo" );
            SceneManager.SetActiveScene( empty );

            int count = SceneManager.sceneCount;

            for ( int i = 0; i < count; i++ )
            {
                var scene = SceneManager.GetSceneAt( i );
                SceneManager.UnloadSceneAsync( scene );
            }
        }

        static void ClearStorage()
        {
            DirectoryInfo root = new DirectoryInfo( Application.persistentDataPath );

            foreach ( FileInfo file in root.EnumerateFiles() )
            {
                try
                {
                    file.Delete();
                }
                catch ( Exception e ) { }
            }

            foreach ( DirectoryInfo dir in root.EnumerateDirectories() )
            {
                try
                {
                    dir.Delete( true );
                }
                catch ( Exception e ) { }
            }
        }
    }
}
#endif
