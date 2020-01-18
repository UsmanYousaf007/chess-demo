using HUF.Utils.Attributes;
using JetBrains.Annotations;
using UnityEngine;

namespace HUF.Utils.Configs.API
{
    public abstract class FeatureConfigBase : AbstractConfig
    {
        
        [SerializeField]
        [ConfigAutoInitWarning]
        bool autoInit = true;

        public bool AutoInit => autoInit;

    }
}