/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class RewardDlgView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public GameObject rewardDailySubscriptionDlgPrefab;
        public GameObject rewardDailyLeagueDlgPrefab;
        public GameObject rewardLeaguePromotionDlgPrefab;
        public GameObject rewardTournamentEndDlgPrefab;

        private GameObject activeDlg;
        private string messageId;
        public Signal<string> buttonClickedSignal = new Signal<string>();
        private Dictionary<string, Action<RewardDlgVO>> initFnMap = new Dictionary<string, Action<RewardDlgVO>>();
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

        public void Init()
        {
            initFnMap.Add("RewardDailySubscription", InitRewardDailySubscriptionDlgPrefab);
            initFnMap.Add("RewardDailyLeague", InitRewardDailyLeagueDlgPrefab);
            initFnMap.Add("RewardLeaguePromotion", InitRewardLeaguePromotionDlgPrefab);
            initFnMap.Add("RewardTournamentEnd", InitRewardTournamentEndDlgPrefab);

            prefabs.Add("RewardDailySubscription", rewardDailySubscriptionDlgPrefab);
            prefabs.Add("RewardDailyLeague", rewardDailyLeagueDlgPrefab);
            prefabs.Add("RewardLeaguePromotion", rewardLeaguePromotionDlgPrefab);
            prefabs.Add("RewardTournamentEnd", rewardTournamentEndDlgPrefab);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void InitRewardDailySubscriptionDlgPrefab(RewardDlgVO vo)
        {
            RewardDailySubscriptionDlg p = activeDlg.GetComponent<RewardDailySubscriptionDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch(vo.msgId));

            p.headingText.text = "Daily Subscription Reward!";
            p.buttonText.text = "Collect";

            for (int i = 0; i < vo.GetRewardItemsCount(); i++)
            {
                int qty = vo.GetRewardItemQty(i);
                p.itemImages[i].transform.parent.gameObject.SetActive(qty > 0);
                if (qty > 0)
                {
                    p.itemImages[i].sprite = vo.GetRewardImage(i);
                    p.itemTexts[i].text = $"x{qty}";
                }
            }
        }

        public void InitRewardDailyLeagueDlgPrefab(RewardDlgVO vo)
        {
            RewardDailyLeagueDlg p = activeDlg.GetComponent<RewardDailyLeagueDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch(vo.msgId));

            p.headingText.text = vo.league;
            p.subHeadingText.text = "Daily League Reward!";
            p.buttonText.text = "Collect";

            for (int i = 0; i < vo.GetRewardItemsCount(); i++)
            {
                int qty = vo.GetRewardItemQty(i);
                p.itemImages[i].transform.parent.gameObject.SetActive(qty > 0);
                if (qty > 0)
                {
                    p.itemImages[i].sprite = vo.GetRewardImage(i);
                    p.itemTexts[i].text = $"x{qty}";
                }
            }
        }

        public void InitRewardLeaguePromotionDlgPrefab(RewardDlgVO vo)
        {
            RewardLeaguePromotionDlg p = activeDlg.GetComponent<RewardLeaguePromotionDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch(vo.msgId));

            p.headingText.text = "Congratulations!";
            p.subHeadingText.text = "You have been promoted!";
            p.buttonText.text = "Got it";

            p.leagueTitleText.text = vo.league;
            p.rewardsSubHeadingText.text = "Your Daily Perks";

            for (int i = 0; i < vo.GetRewardItemsCount(); i++)
            {
                int qty = vo.GetRewardItemQty(i);
                p.itemImages[i].transform.parent.gameObject.SetActive(qty > 0);
                if (qty > 0)
                {
                    p.itemImages[i].sprite = vo.GetRewardImage(i);
                    p.itemTexts[i].text = $"x{qty}";
                }
            }
        }

        public void InitRewardTournamentEndDlgPrefab(RewardDlgVO vo)
        {
            RewardTournamentEndDlg p = activeDlg.GetComponent<RewardTournamentEndDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch(vo.msgId));

            p.headingText.text = vo.tournamentName + " Reward!";
            p.subHeadtingLabel.text = "Rank Achieved";
            p.rankCountText.text = vo.rankCount.ToString();
            p.trophiesCountText.text = vo.trophiesCount.ToString();
            p.chestImage.sprite = vo.chestImage;
            p.chestText.text = vo.chestName;
            p.chestSection.SetActive(vo.chestName != null);

            int rewardCount = 0;
            for (int i = 0; i < vo.GetRewardItemsCount(); i++)
            {
                int qty = vo.GetRewardItemQty(i);
                p.itemImages[i].transform.parent.gameObject.SetActive(qty > 0);
                if (qty > 0)
                {
                    p.itemImages[i].sprite = vo.GetRewardImage(i);
                    p.itemTexts[i].text = $"x{qty}";
                    rewardCount++;
                }
            }

            p.rewardsSection.SetActive(rewardCount > 0);
            p.buttonText.text = p.chestSection.activeSelf || p.rewardsSection.activeSelf ? "Collect" : "Ok";
            LayoutRebuilder.ForceRebuildLayoutImmediate(p.layout);
        }

        public void OnUpdate(RewardDlgVO vo)
        {
            if (initFnMap.ContainsKey(vo.type) == false)
                return;
            
            if (activeDlg != null)
            {
                GameObject.Destroy(activeDlg);
            }
            activeDlg = GameObject.Instantiate(prefabs[vo.type]);

            SkinLink[] objects = activeDlg.GetComponentsInChildren<SkinLink>();
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].InitPrefabSkin();
            }

            activeDlg.transform.SetParent(this.transform, false);
                
            initFnMap[vo.type].Invoke(vo);
        }
    }
}
