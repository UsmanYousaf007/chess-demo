using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUF.AnalyticsHBI.Implementation
{
    [CreateAssetMenu(menuName = "HUF/Analytics/HBIConfig", fileName = "HBIAnalyticsConfig.asset")]
    public class HBIAnalyticsConfig : FeatureConfigBase
    {
        const string MAX_PROJECT_LENGTH = "4";
        const string MAX_SKU_LENGTH = "16";
        
        public static int MaxProjectLength
        {
            get { return int.Parse(MAX_PROJECT_LENGTH); }
        }      

        public static int MaxSKULength
        {
            get { return int.Parse(MAX_SKU_LENGTH); }
        }

        [Header("Max size: " + MAX_PROJECT_LENGTH)]
        [SerializeField]
        string projectName = string.Empty;

        public string ProjectName => projectName;

        [Space]
        [Header("Max size: " + MAX_SKU_LENGTH)]
        [SerializeField]
        string sku = string.Empty;

        public string Sku => sku;
    }
}