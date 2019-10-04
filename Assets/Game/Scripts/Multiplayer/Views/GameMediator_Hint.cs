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

        private void OnRegisterHint()
        {
            view.InitHint();
            view.hintClickedSignal.AddListener(OnGetHint);
        }

        private void OnGetHint()
        {
            if (chessboardModel.isValidChallenge(matchInfoModel.activeChallengeId))
            {
                getHintSignal.Dispatch(false);
            }       
        }

        [ListensTo(typeof(RenderHintSignal))]
        public void OnRenderHint(HintVO vo)
        {
            if (!vo.isHindsight)
            {
                view.RenderHint(vo);
            }
        }

        [ListensTo(typeof(CancelHintSingal))]
        public void OnCancelHint()
        {
            view.CancelHint();
        }

        //[ListensTo(typeof(TurnSwapSignal))]
        public void OnToggleHintButton(bool isPlayerTurn)
        {
            view.ToggleHintButton(isPlayerTurn);
        }

        [ListensTo(typeof(UpdateHintCountSignal))]
        public void OnUpdateHintCount(int count)
        {
            if (view.IsVisible())
            {
                view.UpdateHintCount(count);
            }
        }

        [ListensTo(typeof(HintAvailableSignal))]
        public void OnHintAvailable(bool available)
        {
            if (available)
            {
                view.EnableHintButton();
            }
            else
            {
                view.DisableHintButton();
            }
        }
    }
}
