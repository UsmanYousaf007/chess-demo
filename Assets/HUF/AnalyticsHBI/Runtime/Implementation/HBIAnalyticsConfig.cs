using System;
using HUF.AnalyticsHBI.Runtime.API;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.NetworkRequests;
using UnityEngine;

namespace HUF.AnalyticsHBI.Runtime.Implementation
{
    [Serializable]
    public class PlayerAttributesRoutingTable : IRoutingTableBase
    {
        [Header( " Player Attributes Services" )]
        public string playerAttributes;
    }

    [CreateAssetMenu( menuName = "HUF/Analytics/HBIConfig", fileName = "HBIAnalyticsConfig" )]
    public class HBIAnalyticsConfig : GameServerRoutingConfig<PlayerAttributesRoutingTable>
    {
        const string MAX_PROJECT_LENGTH = "4";
        const string MAX_SKU_LENGTH = "16";

        [Header( "Max size: " + MAX_PROJECT_LENGTH )]
        [SerializeField]
        string projectName = string.Empty;

        [Space]
        [Header( "Max size: " + MAX_SKU_LENGTH )]
        [SerializeField]
        string sku = string.Empty;

        public string Sku => sku;

        [Space] [SerializeField] string authorizationUsername = string.Empty;

        [SerializeField] float[] playerRevenueLevelsInDollars;

        [Tooltip( "Used after reaching value playerRevenueLevelsInDollars level" )]
        [SerializeField]
        float revenueLevelRangeInDollars = 0.1f;

        [Tooltip( "Infinite levels if set to zero" )]
        [SerializeField]
        int maximumRevenueLevel = 0;

        [SerializeField] float refreshPlayerAttributesDistance = 300;

        [Space]
        [Header( "Build for Amazon Store" )]
        [SerializeField]
        bool amazon = false;

        public static int MaxProjectLength => int.Parse( MAX_PROJECT_LENGTH );

        public static int MaxSKULength => int.Parse( MAX_SKU_LENGTH );

        public string ProjectName => projectName;

        public string AuthorizationUsername => authorizationUsername;

        public float[] PlayerRevenueLevelsInDollars => playerRevenueLevelsInDollars;
        public float RevenueLevelRangeInDollars => revenueLevelRangeInDollars;
        public int MaximumRevenueLevel => maximumRevenueLevel;

        public float RefreshPlayerAttributesDistanceInSeconds => refreshPlayerAttributesDistance;

        public bool Amazon
        {
            get
            {
#if HUF_AMAZON
                return true;
#else
                return amazon;
#endif
            }
        }

        public override void RegisterManualInitializers()
        {
            AddManualInitializer( "Analytics - HBI", HAnalyticsHBI.Init );
        }
    }
}