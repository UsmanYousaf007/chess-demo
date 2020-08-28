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

        public GameObject RewardDailySubscriptionDlgPrefab;
        public GameObject RewardDailyLeadueDlgPrefab;
        public GameObject RewardLeaguePromotionDlgPrefab;
        public GameObject RewardTournamentEndDlgPrefab;


        private GameObject activeDlg;
        public Signal buttonClickedSignal = new Signal();
        private Dictionary<string, Action<RewardDlgVO>> initFnMap = new Dictionary<string, Action<RewardDlgVO>>();
        private Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();

        public void Init()
        {
            string test = RewardDailySubscriptionDlgPrefab.ToString();

            initFnMap.Add("RewardDailySubscriptionDlg", InitRewardDailySubscriptionDlgPrefab);
            initFnMap.Add("RewardDailyLeadueDlg", InitRewardDailyLeadueDlgPrefab);
            initFnMap.Add("RewardLeaguePromotionDlg", InitRewardLeaguePromotionDlgPrefab);
            initFnMap.Add("RewardTournamentEndDlg", InitRewardTournamentEndDlgPrefab);

            prefabs.Add("RewardDailySubscriptionDlg", RewardDailySubscriptionDlgPrefab);
            prefabs.Add("RewardDailyLeadueDlg", RewardDailyLeadueDlgPrefab);
            prefabs.Add("RewardLeaguePromotionDlg", RewardLeaguePromotionDlgPrefab);
            prefabs.Add("RewardTournamentEndDlg", RewardTournamentEndDlgPrefab);


            RewardDlgVO vo = new RewardDlgVO();
            vo.type = "RewardDailySubscriptionDlg";
            vo.rewardQty1 = 5;
            vo.rewardQty2 = 6;
            vo.rewardQty3 = 7;
            vo.rewardShortCode1 = "SpecialItemTournamentTicket";
            vo.rewardShortCode2 = "gems";
            vo.rewardShortCode3 = "SpecialItemRatingBoost";
            OnUpdate(vo);
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
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch());

            p.headingText.text = "Daily Subscription Reward!";
            p.buttonText.text = "Collect";

            p.rewardText1.text = "x" + vo.rewardQty1.ToString();
            p.rewardText2.text = "x" + vo.rewardQty2.ToString();
            p.rewardText3.text = "x" + vo.rewardQty3.ToString();

            //p.rewardImage1.sprite = blah(vo.rewardShortCode1);
            //p.rewardImage2.sprite = blah(vo.rewardShortCode2);
            //p.rewardImage3.sprite = blah(vo.rewardShortCode3);
        }

        public void InitRewardDailyLeadueDlgPrefab(RewardDlgVO vo)
        {
            RewardDailyLeagueDlg p = activeDlg.GetComponent<RewardDailyLeagueDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch());

            p.headingText.text = vo.league;
            p.subHeadingText.text = "Daily League Reward!";
            p.buttonText.text = "Collect";

            p.rewardText1.text = "x" + vo.rewardQty1.ToString();
            p.rewardText2.text = "x" + vo.rewardQty2.ToString();
            p.rewardText3.text = "x" + vo.rewardQty3.ToString();

            //p.rewardImage1.sprite = blah(vo.rewardShortCode1);
            //p.rewardImage2.sprite = blah(vo.rewardShortCode2);
            //p.rewardImage3.sprite = blah(vo.rewardShortCode3);
        }

        public void InitRewardLeaguePromotionDlgPrefab(RewardDlgVO vo)
        {
            RewardLeaguePromotionDlg p = activeDlg.GetComponent<RewardLeaguePromotionDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch());

            p.headingText.text = "Congratulations!";
            p.subHeadingText.text = "You have been promoted!";
            p.buttonText.text = "Got it";

            p.leagueTitleText.text = vo.league;
            p.rewardsSubHeadingText.text = "Your Daily Perks";

            p.rewardText1.text = "x" + vo.rewardQty1.ToString();
            p.rewardText2.text = "x" + vo.rewardQty2.ToString();
            p.rewardText3.text = "x" + vo.rewardQty3.ToString();

            //p.rewardImage1.sprite = blah(vo.rewardShortCode1);
            //p.rewardImage2.sprite = blah(vo.rewardShortCode2);
            //p.rewardImage3.sprite = blah(vo.rewardShortCode3);
        }

        public void InitRewardTournamentEndDlgPrefab(RewardDlgVO vo)
        {
            RewardTournamentEndDlg p = activeDlg.GetComponent<RewardTournamentEndDlg>();
            p.button.onClick.AddListener(() => buttonClickedSignal.Dispatch());

            p.headingText.text = vo.tournamentName + " Reward!";
            p.buttonText.text = "Collect";

            // p.chestImage = blah(vo.chestShortCode);
            p.chestText.text = vo.chestName;

            p.rewardText1.text = "x" + vo.rewardQty1.ToString();
            p.rewardText2.text = "x" + vo.rewardQty2.ToString();
            p.rewardText3.text = "x" + vo.rewardQty3.ToString();

            //p.rewardImage1.sprite = blah(vo.rewardShortCode1);
            //p.rewardImage2.sprite = blah(vo.rewardShortCode2);
            //p.rewardImage3.sprite = blah(vo.rewardShortCode3);

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
