/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public StepSignal stepSignal { get; set; }

        private void OnRegisterStep()
        {
            view.InitStep();
            view.stepBackwardClickedSignal.AddListener(OnStepBackward);
            view.stepForwardClickedSignal.AddListener(OnStepForward);
        }

        private void OnStepBackward()
        {
            stepSignal.Dispatch(false);
        }

        private void OnStepForward()
        {
            stepSignal.Dispatch(true);
        }

        [ListensTo(typeof(ToggleStepBackwardSignal))]
        public void OnToggleStepBackward(bool enable)
        {
            view.ToggleStepBackward(enable);
        }

        [ListensTo(typeof(ToggleStepForwardSignal))]
        public void OnToggleStepForward(bool enable)
        {
            view.ToggleStepForward(enable);
        }
    }
}
