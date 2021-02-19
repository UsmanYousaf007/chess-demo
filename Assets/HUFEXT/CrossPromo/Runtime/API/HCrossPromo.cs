using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
using HUFEXT.CrossPromo.Runtime.Implementation;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace HUFEXT.CrossPromo.Runtime.API
{
    public static class HCrossPromo
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HCrossPromo) );

        public static CrossPromoService service;
        static bool isInitialized;

        /// <summary>
        /// Raised when the panel closes.
        /// </summary>
        [PublicAPI]
        public static event UnityAction OnCrossPromoPanelClosed;

        /// <summary>
        /// Returns an information whether or not the panel is visible. 
        /// </summary>
        [PublicAPI]
        public static bool IsPanelOpen() => isInitialized && service.IsPanelOpen();
        
        /// <summary>
        /// Shows the panel.
        /// </summary>
        [PublicAPI]
        public static void OpenPanel()
        {
            service.OpenPanel();
        }
        
        /// <summary>
        /// Closes the panel.
        /// </summary>
        [PublicAPI]
        public static void ClosePanel()
        {
            service.ClosePanel();
            OnCrossPromoPanelClosed?.Invoke();
        }


        /// <summary>
        /// Initializes the module.
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
        /// Sets the text of all buttons that appear when the promoted game is not installed.
        /// </summary>
        [PublicAPI]
        public static void SetNotInstalledStateButtonText(string text)
        {
            service.SetNotInstalledStateButtonText(text);
        }

        /// <summary>
        /// Sets the text of all buttons that appear when the promoted game is installed.
        /// </summary>
        [PublicAPI]
        public static void SetInstalledStateButtonText(string text)
        {
            service.SetInstalledStateButtonText(text);
        }

        /// <summary>
        /// Sets the text of all tile labels that appear when the promoted game is installed.
        /// </summary>
        [PublicAPI]
        public static void SetInstalledStateTileLabelText(string text)
        {
            service.SetInstalledStateTileLabelText(text);
        }

        /// <summary>
        /// Sets the text displayed on the close button in the bottom section.
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