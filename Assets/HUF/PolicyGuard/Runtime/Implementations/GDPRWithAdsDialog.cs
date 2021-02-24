using System;
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

        public override void Close()
        {
            OnAdsConsentSet.Dispatch( personalizedAdsToggle.isOn );
            OnAdsConsentSet = null;
            base.Close( );
        }
    }
}