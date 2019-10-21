using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

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

        public override void Execute()
        {
            if (!preferencesModel.isCoachTooltipShown
                && moveVo.opponentScore > 0
                && moveVo.opponentScore > oldOpponentScore)
            {
                oldOpponentScore = moveVo.opponentScore;
                showCoachOnboardingTooltipSignal.Dispatch(true);
            }
            else if(!preferencesModel.isStrengthTooltipShown
                && moveVo.playerScore > 0
                && moveVo.playerScore > oldPlayerScore)
            {
                oldPlayerScore = moveVo.playerScore;
                showStrengthOnboardingTooltipSignal.Dispatch(true);
            }
        }
    }
}
