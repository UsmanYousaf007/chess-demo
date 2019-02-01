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
        private void OnRegisterHindsight()
        {
            view.InitHindsight();
            view.hindsightClickedSignal.AddListener(OnGetHindsight);
        }

        private void OnGetHindsight()
        {
            getHintSignal.Dispatch(true);
        }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderHindsight(HintVO vo)
        {
            if (vo.isHindsight)
            {
                view.RenderHindsight(vo);
            }
        }

        [ListensTo(typeof(HindsightAvailableSignal))]
        public void OnHindSightAvailable(bool available)
        {
            view.ToggleHindsightButton(available);
        }

        [ListensTo(typeof(UpdateHindsightCountSignal))]
        public void OnUpdateHindsightCount(int count)
        {
            if (view.IsVisible())
            {
                view.UpdateHindsightCount(count);
            }
        }
    }
}
