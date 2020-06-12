using HUF.AnalyticsHBI.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Analytics/HBIConfig", fileName = "HBIAnalyticsConfig")]
    public class HBIAnalyticsConfig : FeatureConfigBase
    {
        const string MAX_PROJECT_LENGTH = "4";
        const string MAX_SKU_LENGTH = "16";

        public static int MaxProjectLength => int.Parse(MAX_PROJECT_LENGTH);

        public static int MaxSKULength => int.Parse(MAX_SKU_LENGTH);

        [Header("Max size: " + MAX_PROJECT_LENGTH)]
        [SerializeField]
        string projectName = string.Empty;

        public string ProjectName => projectName;

        [Space]
        [Header("Max size: " + MAX_SKU_LENGTH)]
        [SerializeField]
        string sku = string.Empty;

        public string Sku => sku;

        [Space]
        [Header("Build for Amazon Store")]
        [SerializeField]
        bool amazon = false;

        public bool Amazon => amazon;

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Analytics - HBI", HAnalyticsHBI.Init );
        }
    }
}