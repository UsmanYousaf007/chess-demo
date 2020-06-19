using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.CrossPromo.Runtime.Implementation;
using JetBrains.Annotations;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.CrossPromo.Runtime.API
{
    public static class HCrossPromo
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HCrossPromo) );

        public static CrossPromoService service;
        static bool isInitialized;
        static IPromise promise = null;

        /// <summary>
        /// Use this event to get information about panel being close
        /// at any point.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnCrossPromoPanelClosed;

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

        /// <summary>
        /// Use this method to show panel
        /// </summary>
        [PublicAPI]
        public static IPromise OpenPanel()
        {
            promise = new Promise();
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
                HLog.Log( logPrefix, "Service initialized" );
                return;
            }

            HLog.LogWarning( logPrefix, "Service already initialized" );
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

        public static void Fetch()
        {
            service.FetchRemoteConfigs();
        }
    }
}