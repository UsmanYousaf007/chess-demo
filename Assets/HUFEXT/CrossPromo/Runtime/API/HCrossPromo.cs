using HUF.Utils.Configs.API;
using HUFEXT.CrossPromo.Implementation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using TurboLabz.InstantFramework;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;

namespace HUFEXT.CrossPromo.API
{
    public static class HCrossPromo
    {
        /// <summary>
        /// Use this event to get information about panel being close
        /// at any point.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnCrossPromoPanelClosed;

        [Inject] public static ToggleBannerSignal toggleBannerSignal { get; set; }

        public static CrossPromoService service;
        static bool isInitialized;

        /// <summary>
        /// Use this method to close panel explicitly
        /// </summary>
        [PublicAPI]
        public static void ClosePanel()
        {
            promise.Dispatch();
            promise = null;
            service.ClosePanel();
            OnCrossPromoPanelClosed?.Invoke();
        }

        static IPromise promise = new Promise();
        /// <summary>
        /// Use this method to show panel
        /// </summary>
        [PublicAPI]
        public static IPromise OpenPanel()
        {
            service.OpenPanel();
            return promise;
        }

        /// <summary>
        /// Use this method in order to initialize module manually
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            if (!isInitialized)
            {
                service = new CrossPromoService();
                isInitialized = true;
            }
        }

        /// <summary>
        /// Use this method to set text for all buttons that appear when
        /// promoted game is not installed.
        /// </summary>
        [PublicAPI]
        public static void SetNotInstalledStateButtonText(string text)
        {
            service.SetNotInstalledStateButtonText(text);
        }

        /// <summary>
        /// Use this method to set text for all buttons that appear
        /// when promoted game is installed.
        /// </summary>
        [PublicAPI]
        public static void SetInstalledStateButtonText(string text)
        {
            service.SetInstalledStateButtonText(text);
        }

        /// <summary>
        /// Use this method to set text for all tile labels that appear
        /// whenever promoted game is installed.
        /// </summary>
        [PublicAPI]
        public static void SetInstalledStateTileLabelText(string text)
        {
            service.SetInstalledStateTileLabelText(text);
        }

        /// <summary>
        /// Use this method to set text displayed on bottom section close button.
        /// </summary>
        [PublicAPI]
        public static void SetCloseButtonText(string text)
        {
            service.SetCloseButtonText(text);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void AutoInit()
        {
            if (HConfigs.HasConfig<CrossPromoRemoteConfig>())
            {
                var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
                if (config != null && config.AutoInit)
                {
                    Init();
                }   
            }
        }
    }
}