using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using System;

namespace TurboLabz.CPU
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

        private static int oldOpponentScore;
        private static int oldPlayerScore;

        private const int POWERUP_USE_LIMIT = 7;
        private const int POWERUP_TRAINING_DAYS_LIMIT = 30;

        public override void Execute()
        {
            if (!preferencesModel.isCoachTooltipShown
               && moveVo.opponentScore > 0
               && moveVo.opponentScore > oldOpponentScore
               && preferencesModel.coachUsedCount < POWERUP_USE_LIMIT
               && (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays < POWERUP_TRAINING_DAYS_LIMIT)
            {
                oldOpponentScore = moveVo.opponentScore;
                showCoachOnboardingTooltipSignal.Dispatch(true);
                preferencesModel.isCoachTooltipShown = true;
            }
            else if (!preferencesModel.isStrengthTooltipShown
                && moveVo.playerScore > 0
                && moveVo.playerScore > oldPlayerScore
                && preferencesModel.strengthUsedCount < POWERUP_USE_LIMIT
                && (int)(DateTime.Now - preferencesModel.timeAtLobbyLoadedFirstTime).TotalDays < POWERUP_TRAINING_DAYS_LIMIT)
            {
                oldPlayerScore = moveVo.playerScore;
                showStrengthOnboardingTooltipSignal.Dispatch(true);
                preferencesModel.isStrengthTooltipShown = true;
            }
        }
    }
}
