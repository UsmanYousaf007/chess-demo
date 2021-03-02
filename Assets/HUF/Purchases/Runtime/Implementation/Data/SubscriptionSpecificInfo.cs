using System;
using UnityEngine;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    [Serializable]
    public class SubscriptionSpecificInfo
    {
        [SerializeField] int androidTrialPeriodInDays = default;
        [SerializeField] int androidPeriodInDays = default;
        
        [SerializeField] int iOSTrialPeriodInDays = default;
        [SerializeField] int iOSPeriodInDays = default;

        public int PeriodInDays
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        return iOSPeriodInDays;
                    case RuntimePlatform.Android:
                        return androidPeriodInDays;
                    default:
                        return 0;
                }
            }
        }
        
        public int TrialPeriodInDays
        {
            get
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.IPhonePlayer:
                        return iOSTrialPeriodInDays;
                    case RuntimePlatform.Android:
                        return androidTrialPeriodInDays;
                    default:
                        return 0;
                }
            }
        }

        public void SetAndroidInfo( int trialPeroidInDays, int peroidInDays )
        {
            androidPeriodInDays = peroidInDays;
            androidTrialPeriodInDays = trialPeroidInDays;
        }
        
        public void SetIOSInfo( int trialPeroidInDays, int peroidInDays )
        {
            iOSPeriodInDays      = peroidInDays;
            iOSTrialPeriodInDays = trialPeroidInDays;
        }
    }
}