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

        private const int POWERUP_USE_LIMIT = 1;
        private const int POWERUP_TRAINING_DAYS_LIMIT = 30;

        public override void Execute()
        {
            if (!preferencesModel.isCoachTooltipShown
               && moveVo.opponentScore > 0
               && moveVo.opponentScore > oldOpponentScore)
            {
                oldOpponentScore = moveVo.opponentScore;
                showCoachOnboardingTooltipSignal.Dispatch(true);
                showStrengthOnboardingTooltipSignal.Dispatch(false);
                preferencesModel.isCoachTooltipShown = true;
            }
            else if (!preferencesModel.isStrengthTooltipShown
                && moveVo.playerScore > 0
                && moveVo.playerScore > oldPlayerScore)
            {
                oldPlayerScore = moveVo.playerScore;
                showStrengthOnboardingTooltipSignal.Dispatch(true);
                showCoachOnboardingTooltipSignal.Dispatch(false);
                preferencesModel.isStrengthTooltipShown = true;
            }
        }
    }
}
