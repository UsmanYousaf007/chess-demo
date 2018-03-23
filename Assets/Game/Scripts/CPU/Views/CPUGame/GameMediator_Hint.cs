/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:07 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;

namespace TurboLabz.InstantChess
{
    public partial class GameMediator
    {
        //Dispatch Signals
        [Inject] public GetHintSignal getHintSignal { get; set; }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderHint(HintVO vo)
        {
            view.RenderHint(vo);
        }

        [ListensTo(typeof(UpdateHintCountSignal))]
        public void OnUpdateHintCount(int count)
        {
            view.UpdateHintCount(count);
        }

        [ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleHintButton(bool isPlayerTurn)
        {
            view.ToggleHintButton(isPlayerTurn);
        }

        [ListensTo(typeof(DisableHintButtonSignal))]
        public void OnDisableHintButton()
        {
            view.DisableHintButton();
        }

        private void OnRegisterHint()
        {
            view.InitHint();
            view.hintClickedSignal.AddListener(OnGetHint);
        }

        private void OnRemoveHint()
        {
            view.hintClickedSignal.RemoveAllListeners();
        }

        private void OnGetHint()
        {
            getHintSignal.Dispatch();
        }
    }
}
