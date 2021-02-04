using HUF.GenericDialog.Runtime.API;
using HUF.GenericDialog.Runtime.Configs;
// ReSharper disable once RedundantUsingDirective - ifdef
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Logging;
// ReSharper disable once RedundantUsingDirective - ifdef
using HUF.Utils.Runtime.UnityEvents;

namespace HUF.GenericDialog.Runtime.Implementations
{
    public class PreTrackingPopup : HGenericDialogInstance
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(PreTrackingPopup) );

        HGenericDialogConfig config;

        protected override HLogPrefix LogPrefix => logPrefix;

        protected override void HandleInitialization( HGenericDialogConfig config )
        {
            this.config = config;

#if !HUF_TRANSLATION_SYSTEM
            if ( !HConfigs.HasConfig<PreTrackingPopupTextOverride>() )
                return;

            var contentConfig = HConfigs.GetConfig<PreTrackingPopupTextOverride>();

            if ( string.IsNullOrWhiteSpace( contentConfig.text ) )
                return;

            OnContentTextOverride.Invoke( contentConfig.text );
#endif
        }

        protected override void HandlePrimaryButtonClick()
        {
            #if UNITY_IOS && HUF_ANALYTICS
            Analytics.Runtime.API.HAnalytics.CheckATTStatus( null );
            #endif
            HGenericDialog.SetAsHandled( config );
            OnClosePopup.Invoke();
        }

        protected override void HandleSecondaryButtonClick()
        {
            Postpone();

            OnClosePopup.Invoke();
        }
    }
}