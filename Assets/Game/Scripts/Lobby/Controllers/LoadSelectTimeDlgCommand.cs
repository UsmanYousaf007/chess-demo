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
            vo.canSeeRewardedVideo = adsService.IsPlayerQualifiedForRewarded(vo.storeItem.currency3Cost, adsSettingsModel.minPlayDaysRequired);
            vo.rewardedVideoCoolDownInterval = preferencesModel.purchasesCount < adsSettingsModel.minPurchasesRequired ? adsSettingsModel.freemiumTimerCooldownTime : adsSettingsModel.premiumTimerCooldownTime;
            vo.bet = betValue;

            updateTimeSelectDlgSignal.Dispatch(vo);
        }


    }
}
