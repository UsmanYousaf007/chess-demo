using strange.extensions.command.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class LoadSelectTimeDlgCommand : Command
    {
        // parameter
        [Inject] public long betValue { get; set; }
        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        [Inject] public IAdsSettingsModel adsSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        //Services
        [Inject] public IAdsService adsService { get; set; }
        //Dispatch Signals
        [Inject] public UpdateTimeSelectDlgSignal updateTimeSelectDlgSignal { get; set; }

        private string shortCode = "SpecialItemPowerMode";

        public override void Execute()
        {
            var vo = new SelectTimeDlgVO();
            if (storeSettingsModel.items.ContainsKey(shortCode))
            {
                vo.storeItem = storeSettingsModel.items[shortCode];
            }
            vo.canSeeRewardedVideo = playerModel.gems < adsSettingsModel.minGemsRequiredforRV && playerModel.rvUnlockTimestamp > 0
                && !(adsSettingsModel.removeRVOnPurchase && playerModel.HasPurchased());
            //vo.rewardedVideoCoolDownInterval = preferencesModel.purchasesCount < adsSettingsModel.minPurchasesRequired ? adsSettingsModel.freemiumTimerCooldownTime : adsSettingsModel.premiumTimerCooldownTime;
            vo.coolDownTimeUTC = playerModel.rvUnlockTimestamp;
            vo.bet = betValue;

            updateTimeSelectDlgSignal.Dispatch(vo);
        }


    }
}
