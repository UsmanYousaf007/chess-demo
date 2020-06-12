using System;
using System.Collections.Generic;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.ModuleStarter.Runtime.Config;
using JetBrains.Annotations;
using static HUF.Utils.Runtime.Configs.API.FeatureConfigBase;

namespace HUFEXT.ModuleStarter.Runtime.Data
{
    public class ModuleInitializer
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(ModuleInitializer) );

        static readonly Dictionary<string, ModuleInitializer> initializers = new Dictionary<string, ModuleInitializer>();

        public static bool HasAnyModules => initializers.Count > 0;
        public static int ModulesCount => initializers.Count;

        public InitCall Initializer { get; private set; }
        readonly string name;
        readonly int order;
        public readonly bool asyncOnly;

        ModuleInitializer( string name, int order, InitCall initializer, bool asyncOnly )
        {
            Initializer = initializer;
            this.name = name;
            this.order = order;
            this.asyncOnly = asyncOnly;
        }

        public static ModuleInitializer Get( string id )
        {
            if ( initializers.TryGetValue( id, out ModuleInitializer initializer ) )
                return initializer;

            return null;
        }

        public static void Register( InitCall initializer, string name )
        {
            Register( initializer, name, false );
        }

        public static void Register( [NotNull] Action initializer, string name )
        {
            void InstantInitializer( Action callback )
            {
                initializer();
                callback();
            }

            Register( InstantInitializer, name, true );
        }

        public static void ReloadAll()
        {
            initializers.Clear();
            foreach ( var config in HConfigs.GetConfigsByBaseClass<FeatureConfigBase>() )
            {
                if ( !config.AutoInit )
                    config.RegisterManualInitializers();
            }
        }

        static void Register( InitCall initializer, string name, bool asyncOnly )
        {
            if ( initializers.ContainsKey( name ) )
            {
                HLog.LogError( logPrefix, $"Initializer {name} already registered" );
                return;
            }

            var module = new ModuleInitializer( name,
                ModuleInitializerConfig.ProcessOrder( name ),
                initializer,
                asyncOnly );

            initializers.Add( name, module );
        }
    }
}