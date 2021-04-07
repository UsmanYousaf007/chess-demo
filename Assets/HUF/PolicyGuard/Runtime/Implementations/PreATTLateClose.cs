using HUF.GenericDialog.Runtime.Configs;
using HUF.PolicyGuard.Runtime.API;
using HUF.PolicyGuard.Runtime.Configs;
using HUF.Utils.Runtime.Configs.API;

namespace HUF.PolicyGuard.Runtime.Implementations
{
    public class PreATTLateClose : GenericEventDialog
    {
        PolicyGuardConfig policyConfig;

        protected override void HandleInitialization( HGenericDialogConfig config )
        {
            policyConfig = HConfigs.GetConfig<PolicyGuardConfig>();

            if ( !policyConfig.DelayClosingATTPreOptInPopup )
                return;

            HPolicyGuard.OnATTNativePopupClosed += HandleATTNativePopupClosed;
            OnSecondaryButtonClicked += ForceClose;
        }

        void HandleATTNativePopupClosed( bool isAccepted )
        {
            ForceClose();
        }

        public override void Close()
        {
            if (!policyConfig.DelayClosingATTPreOptInPopup)
                base.Close();
        }

        void ForceClose()
        {
            HPolicyGuard.OnATTNativePopupClosed -= HandleATTNativePopupClosed;
            OnSecondaryButtonClicked -= ForceClose;
            base.Close();
        }
    }
}