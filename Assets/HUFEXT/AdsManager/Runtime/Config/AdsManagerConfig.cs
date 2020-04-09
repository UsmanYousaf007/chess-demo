using HUF.Utils.Configs.API;
using HUFEXT.AdsManager.Runtime.AdManagers;
using UnityEngine;

namespace HUFEXT.AdsManager.Runtime.Config
{
    [CreateAssetMenu( menuName = "HUFEXT/Ads/AdsManagerConfig", fileName = "AdsManagerConfig.asset" )]
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
    }
}