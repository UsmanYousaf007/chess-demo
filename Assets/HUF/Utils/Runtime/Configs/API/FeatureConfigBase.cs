using System;
using HUF.Utils.Runtime.Attributes;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Runtime.Configs.API
{
    public abstract class FeatureConfigBase : AbstractConfig
    {
        [SerializeField]
        [ConfigAutoInitWarning]
        bool autoInit = true;

        public delegate void InitCall( [NotNull] Action finishCallback );

        public bool AutoInit => autoInit;

        protected static void AddManualSynchronousInitializer( string uniqueName, InitCall initializer )
        {
#if HUFEXT_MODULE_STARTER
            HUFEXT.ModuleStarter.Runtime.API.HInitializationPipeline.Register( uniqueName, initializer );
#endif
        }

        protected static void AddManualInitializer( string uniqueName, [NotNull] Action initializer )
        {
#if HUFEXT_MODULE_STARTER
            HUFEXT.ModuleStarter.Runtime.API.HInitializationPipeline.Register( uniqueName, initializer );
#endif
        }

        public virtual void RegisterManualInitializers() { }
    }
}