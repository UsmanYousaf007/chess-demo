/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
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
        [Inject] public INotificationsModel notificationsModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }

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
            p.button.onClick.AddListener(() =>
            {
                analyticsService.Event(AnalyticsEventId.inbox_subscription_reward_collected);
                buttonClickedSignal.Dispatch(vo.msgId);
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
                notificationsModel.UnregisterNotifications("subscription");

                var notification = new Notification();
                notification.title = localizationService.Get(LocalizationKey.NOTIFICATION_SUBSCRIPTION_REWARD_TITLE);
                notification.body = localizationService.Get(LocalizationKey.NOTIFICATION_SUBSCRIPTION_REWARD_BODY);
                notification.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1));
                notification.sender = "subscription";
                notificationsModel.RegisterNotification(notification);

                var reminder = new Notification();
                reminder.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
                reminder.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
                reminder.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(settingsModel.dailyNotificationDeadlineHour).ToUniversalTime());
                reminder.sender = "subscription";
                notificationsModel.RegisterNotification(reminder);
            });

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
            p.button.onClick.AddListener(() =>
            {
                analyticsService.Event(AnalyticsEventId.inbox_daily_league_reward_collected);
                buttonClickedSignal.Dispatch(vo.msgId);
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
                notificationsModel.UnregisterNotifications("league");

                var notification = new Notification();
                notification.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
                notification.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
                notification.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1));
                notification.sender = "league";
                notificationsModel.RegisterNotification(notification);

                LogUtil.Log(DateTime.Today.ToString());

                var reminder = new Notification();
                reminder.title = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_TITLE);
                reminder.body = localizationService.Get(LocalizationKey.NOTIFICATION_DAILY_REWARD_BODY);
                reminder.timestamp = TimeUtil.ToUnixTimestamp(DateTime.Today.AddDays(1).AddHours(settingsModel.dailyNotificationDeadlineHour).ToUniversalTime());
                reminder.sender = "league";
                notificationsModel.RegisterNotification(reminder);
            });

            p.headingText.text = vo.league.ToUpper();
            p.leagueGradient.sprite = vo.leagueGradient;
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
            p.button.onClick.AddListener(() =>
            {
                buttonClickedSignal.Dispatch(vo.msgId);
                audioService.PlayStandardClick();
            });

            p.headingText.text = "Congratulations!";
            p.subHeadingText.text = "You have been promoted!";
            p.buttonText.text = "Got it";

            p.leagueTitleText.text = vo.league.ToUpper();
            p.leagueGradient.sprite = vo.leagueGradient;
            p.rewardsSubHeadingText.text = "Your Daily Perks";

            p.playerPic.UpdateView(vo.playerProfile);

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
            p.button.enabled = true;
            p.button.onClick.AddListener(() =>
            {
                analyticsService.Event(AnalyticsEventId.inbox_tournament_reward_collected);
                audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
                p.PlayParticleAnimation();
                StartCoroutine(TournamentRewardAnimationEnded(vo.msgId));
                p.button.enabled = false;
            });

            p.headingText.text = vo.tournamentName + " Reward";
            p.subHeadtingLabel.text = "Rank Achieved";
            p.trophiesAlreadyAdded.text = localizationService.Get(LocalizationKey.TOURNAMENT_REWARD_DLG_TROPHIES_ADDED);
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

        private IEnumerator TournamentRewardAnimationEnded(string msgId)
        {
            yield return new WaitForSeconds(1.3f);
            buttonClickedSignal.Dispatch(msgId);
        }
    }
}
