/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class CPUStatsMediator : Mediator
    {
        // Dispatch signals
        //[Inject] public LoadGameSignal loadGameSignal { get; set; }

        // View injection
        [Inject] public CPUStatsView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
            view.decDurationButtonClickedSignal.AddListener(OnDecDurationButtonClicked);
            view.incDurationButtonClickedSignal.AddListener(OnIncDurationButtonClicked);
        }

        public override void OnRemove()
        {
            view.backButtonClickedSignal.RemoveAllListeners();
            view.CleanUp();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateStatsSignal))]
        public void OnUpdateStats(CPUStatsVO vo)
        {
            view.UpdateView(vo);
        }

        private void OnBackButtonClicked()
        {
            //loadGameSignal.Dispatch();
        }

        private void OnDecDurationButtonClicked()
        {
        }

        private void OnIncDurationButtonClicked()
        {
        }
    }
}
