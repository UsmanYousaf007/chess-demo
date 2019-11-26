using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using System;

namespace TurboLabz.Multiplayer
{
    public class OnboardingTooltipCommand : Command
    {
        // Parameters
        [Inject] public MoveVO moveVo { get; set; }

        // Models
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Signals
        [Inject] public ShowStrengthOnboardingTooltipSignal showStrengthOnboardingTooltipSignal { get; set; }
        [Inject] public ShowCoachOnboardingTooltipSignal showCoachOnboardingTooltipSignal { get; set; }

        public static int oldOpponentScore;
        public static int oldPlayerScore;

        public override void Execute()
        {
            if (!preferencesModel.isCoachTooltipShown
               && moveVo.opponentScore > oldOpponentScore)
            {
                showCoachOnboardingTooltipSignal.Dispatch(true);
                showStrengthOnboardingTooltipSignal.Dispatch(false);
                preferencesModel.isCoachTooltipShown = true;
            }
            else if (!preferencesModel.isStrengthTooltipShown
                && moveVo.playerScore > oldPlayerScore)
            {
                showStrengthOnboardingTooltipSignal.Dispatch(true);
                showCoachOnboardingTooltipSignal.Dispatch(false);
                preferencesModel.isStrengthTooltipShown = true;
            }

            oldOpponentScore = moveVo.opponentScore;
            oldPlayerScore = moveVo.playerScore;
        }
    }
}
