/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        //Dispatch Signals
        [Inject] public GetHintSignal getHintSignal { get; set; }
        [Inject] public NavigatorIgnoreEventSignal navigatorIgnoreEventSignal { get; set; }

        private void OnRegisterHint()
        {
            view.InitHint();
            view.hintClickedSignal.AddListener(OnGetHint);
        }

        private void OnGetHint(bool isHindsight)
        {
            navigatorIgnoreEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            getHintSignal.Dispatch(isHindsight);
        }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderHint(HintVO vo)
        {
            view.RenderHint(vo);
            navigatorIgnoreEventSignal.Dispatch(NavigatorEvent.NONE);
        }

        [ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleHintButton(bool isPlayerTurn)
        {
            view.ToggleHintButton(isPlayerTurn);
        }

        [ListensTo(typeof(UpdateHintCountSignal))]
        public void OnUpdateHintCount(int count)
        {
            view.UpdateHintCount(count);
        }


        /*

        [ListensTo(typeof(DisableHintButtonSignal))]
        public void OnDisableHintButton()
        {
            view.DisableHintButton();
        }

       

        private void OnRemoveHint()
        {
            view.hintClickedSignal.RemoveAllListeners();
        }
        */

    }
}
