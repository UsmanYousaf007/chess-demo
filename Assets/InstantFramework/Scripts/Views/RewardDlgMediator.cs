/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class RewardDlgMediator : Mediator
    {
        // View injection
        [Inject] public RewardDlgView view { get; set; }

        [Inject] public IBackendService backendService { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadInboxSignal loadInboxSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.buttonClickedSignal.AddListener(OnButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.REWARD_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.REWARD_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateRewardDlgViewSignal))]
        public void OnUpdate(RewardDlgVO vo)
        {
            //view.Show();
            view.OnUpdate(vo);
        }

        public void OnButtonClicked()
        {
            TLUtils.LogUtil.Log("RewardDlgMediator::OnButtonClicked()");
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            loadInboxSignal.Dispatch();
        }

    }
}
