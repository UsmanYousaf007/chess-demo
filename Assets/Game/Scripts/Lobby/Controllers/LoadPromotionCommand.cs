using strange.extensions.command.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class LoadPromotionCommand : Command
    {
        //Singals
        [Inject] public ShowPromotionSignal showPromotionSignal { get; set; }

        //Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        private static string lastLoadedBanner = "none";

        public override void Execute()
        {
            if (!preferencesModel.isLobbyLoadedFirstTime)
            {
                preferencesModel.isLobbyLoadedFirstTime = true;
                return;
            }

            if (!preferencesModel.isCoachTooltipShown)
            {
                showPromotionSignal.Dispatch(LobbyPromotionKeys.COACH_BANNER);
                lastLoadedBanner = LobbyPromotionKeys.COACH_BANNER;
                return;
            }

            if (!preferencesModel.isStrengthTooltipShown)
            {
                showPromotionSignal.Dispatch(LobbyPromotionKeys.STRENGTH_BANNER);
                lastLoadedBanner = LobbyPromotionKeys.STRENGTH_BANNER;
                return;
            }

            if (!lastLoadedBanner.Equals("none"))
            {
                showPromotionSignal.Dispatch("none");
                lastLoadedBanner = "none";
                return;
            }
        }
    }
}
