using System;
using System.Collections;
using System.Collections.Generic;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUFEXT.ModuleStarter.Runtime.Config;
using HUFEXT.ModuleStarter.Runtime.Data;
using JetBrains.Annotations;
using UnityEngine;
using static HUF.Utils.Runtime.Configs.API.FeatureConfigBase;

namespace HUFEXT.ModuleStarter.Runtime.API
{
    public static class HInitializationPipeline
    {
        static readonly Action voidAction = () => { };
        static readonly Action releaseAction = () => isAwaitingModuleInit = false;

        static bool isAwaitingModuleInit;

        /// <summary>
        /// Event for reporting initialization progress (from 0 to 1)
        /// </summary>
        [PublicAPI]
        public static event Action<float> OnInitializationProgress;

        /// <summary>
        /// Used to manually run the initialization pipeline
        /// </summary>
        [PublicAPI]
        public static void RunPipeline()
        {
            ModuleInitializer.ReloadAll();
            ModuleInitializerConfig.SortEntries();
            CoroutineManager.StartCoroutine( InitRoutine( ModuleInitializerConfig.Entries ) );
        }

        public static void Register( string uniqueName, InitCall initializer )
        {
            ModuleInitializer.Register( initializer, uniqueName );
        }

        public static void Register( string uniqueName, [NotNull] Action initializer )
        {
            ModuleInitializer.Register( initializer, uniqueName );
        }

        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void AutoInit()
        {
            if ( !HConfigs.HasConfig<HInitializationPipelineConfig>()
                 || !HConfigs.GetConfig<HInitializationPipelineConfig>().AutoInit )
            {
                return;
            }

            RunPipeline();
        }

        static IEnumerator InitRoutine( IEnumerable<ModuleInitializerConfig.OrderEntry> entries )
        {
            float total = ModuleInitializer.ModulesCount;
            int current = 0;
            OnInitializationProgress.Dispatch( 0 );

            foreach ( ModuleInitializerConfig.OrderEntry entry in entries )
            {
                var module = ModuleInitializer.Get( entry.id );

                if ( module == null || entry.isSkipped )
                    continue;

                yield return null;

                isAwaitingModuleInit = true;

                if ( entry.isAsync )
                {
                    module.Initializer( voidAction );
                    isAwaitingModuleInit = false;
                }
                else
                {
                    module.Initializer( releaseAction );
                }

                while ( isAwaitingModuleInit )
                    yield return null;

                OnInitializationProgress.Dispatch( ++current/total );
            }
        }
    }
}