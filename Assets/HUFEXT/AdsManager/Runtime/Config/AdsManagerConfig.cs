using HUF.Utils.Runtime.Configs.API;
using HUFEXT.AdsManager.Runtime.AdManagers;
using HUFEXT.AdsManager.Runtime.API;
using UnityEngine;

namespace HUFEXT.AdsManager.Runtime.Config
{
    [CreateAssetMenu( menuName = "HUFEXT/Ads/AdsManagerConfig", fileName = "AdsManagerConfig" )]
    public class AdsManagerConfig : FeatureConfigBase
    {
        [SerializeField] int fetchShortTimes = 3;
        [SerializeField] int delayBetweenFetchShort = 0;
        [SerializeField] int delayBetweenFetchLong = 30;
        [SerializeField] AdsMediator adsProvider = default;

        public int FetchShortTimes => fetchShortTimes;
        public int DelayBetweenFetchShort => delayBetweenFetchShort;
        public int DelayBetweenFetchLong => delayBetweenFetchLong;
        public AdsMediator AdsProvider => adsProvider;

        public override void RegisterManualInitializers()
        {
            AddManualSynchronousInitializer( "Ads Manager", HAdsManager.Init );
        }
    }
}