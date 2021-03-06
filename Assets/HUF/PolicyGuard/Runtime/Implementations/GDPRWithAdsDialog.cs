using System;
using HUF.Ads.Runtime.API;
using HUF.Utils.Runtime.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace HUF.PolicyGuard.Runtime.Implementations
{
    public class GDPRWithAdsDialog : GenericEventDialog
    {
        [SerializeField] Toggle personalizedAdsToggle;

        /// <summary>
        /// Raised after popup closes with ads consent.
        /// </summary>
        [PublicAPI]
        public event Action<bool> OnAdsConsentSet;

        protected override void HandlePrimaryButtonClick()
        {
            HAds.CollectSensitiveData( personalizedAdsToggle.isOn );
            base.HandlePrimaryButtonClick();
        }

        public override void Close()
        {
            OnAdsConsentSet.Dispatch( personalizedAdsToggle.isOn );
            OnAdsConsentSet = null;
            base.Close( );
        }
    }
}