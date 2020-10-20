/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class InboxView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowBottomNavSignal showBottomNavSignal { get; set; }

        public Transform listContainer;
        public GameObject inBoxBarContainer;
        public GameObject sectionHeader;
        public GameObject inBoxBarPrefab;
        public GameObject emptyInBoxStrip;
        public Text emptyInboxLabel;
        public GameObject processing;
        public Text heading;
        public Text stripHeading;

        public Button bottomNavBackButton;

        // Player bar click signal
        [HideInInspector]
        public Signal<InboxBar> inBoxBarClickedSignal = new Signal<InboxBar>();
        private Dictionary<string, InboxBar> inBoxBars = new Dictionary<string, InboxBar>();
        private Dictionary<string, Action<InboxMessage>> AddInboxBarFnMap = new Dictionary<string, Action<InboxMessage>>();
        public Signal bottoNavBackButtonClickedSignal = new Signal();

        public void Init()
        {
            heading.text = localizationService.Get(LocalizationKey.INBOX_HEADING);
            stripHeading.text = localizationService.Get(LocalizationKey.INBOX_SECTION_HEADER_REWARDS);
            emptyInboxLabel.text = localizationService.Get(LocalizationKey.INBOX_EMPTY_INBOX_LABEL);

            inBoxBarContainer.gameObject.SetActive(false);
            emptyInBoxStrip.gameObject.SetActive(false);

            AddInboxBarFnMap.Add("RewardTournamentEnd", AddTournamentRewardBar);
            AddInboxBarFnMap.Add("RewardDailySubscription", AddDailySubscriptionRewardBar);
            AddInboxBarFnMap.Add("RewardDailyLeague", AddDailyLeagueRewardBar);
            AddInboxBarFnMap.Add("RewardLeaguePromotion", AddLeaguePromotionRewardBar);

            bottomNavBackButton.onClick.AddListener(() =>
            {
                bottoNavBackButtonClickedSignal.Dispatch();
                audioService.PlayStandardClick();
            });

            //Sort();
        }

        public void Show()
        {
            showBottomNavSignal.Dispatch(false);
            gameObject.SetActive(true);
            processing.SetActive(false);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Sort()
        {
            List<InboxBar> items = new List<InboxBar>();

            // Copy all player bars into a list
            foreach (KeyValuePair<string, InboxBar> item in inBoxBars)
            {
                items.Add(item.Value);
            }

            inBoxBarContainer.gameObject.SetActive(items.Count > 0);
            emptyInBoxStrip.gameObject.SetActive(items.Count == 0);

            items.Sort((x, y) => x.timeStamp > y.timeStamp ? -1 : ((x.timeStamp < y.timeStamp) ? 1 : 0));

            // Adust order
            int index = 0;
            sectionHeader.transform.SetSiblingIndex(index++);

            for (int i = 0; i < items.Count; i++)
            {
                items[i].transform.SetSiblingIndex(index++);
            }
        }

        public void AddTournamentRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = TournamentAssetsContainer.Load().GetThumb(msg.tournamentType);
            item.headingText.text = "Tournament Rewards";
            item.subHeadingText.text = $"Completed {TimeUtil.DateTimeToRelativeTime(DateTimeOffset.FromUnixTimeMilliseconds(msg.timeStamp).LocalDateTime)}";
            item.thumbnail.sprite = TournamentAssetsContainer.Load().GetSticker(msg.tournamentType);
            item.thumbnail.preserveAspect = true;

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.buttonText.text = "Collect";
            item.button.onClick.AddListener(() =>
            {
                inBoxBarClickedSignal.Dispatch(item);
                audioService.PlayStandardClick();
            });

            item.transform.SetParent(listContainer, false);
            item.skinLink.InitPrefabSkin();
            inBoxBars.Add(item.msgId, item);
        }

        public void AddDailySubscriptionRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = "Daily Subscription Rewards";
            item.subHeadingText.text = "Collect Now";
            item.thumbnail.sprite = SpriteBank.container.GetSprite("SubscriptionSticker");
            item.thumbnail.preserveAspect = false;

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.buttonText.text = "Collect";
            item.button.onClick.AddListener(() =>
            {
                inBoxBarClickedSignal.Dispatch(item);
                audioService.PlayStandardClick();
            });

            item.transform.SetParent(listContainer, false);
            item.skinLink.InitPrefabSkin();
            inBoxBars.Add(item.msgId, item);
        }

        public void AddDailyLeagueRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = "Daily League Rewards";
            item.subHeadingText.text = "Collect Now";
            item.thumbnail.sprite = SpriteBank.container.GetSprite("RankSticker");
            item.thumbnail.preserveAspect = false;

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.buttonText.text = "Collect";
            item.button.onClick.AddListener(() =>
            {
                inBoxBarClickedSignal.Dispatch(item);
                audioService.PlayStandardClick();
            });

            item.transform.SetParent(listContainer, false);
            item.skinLink.InitPrefabSkin();
            inBoxBars.Add(item.msgId, item);
        }

        public void AddLeaguePromotionRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = "Congratulations!";
            item.subHeadingText.text = "You've been Promoted!";
            item.thumbnail.sprite = SpriteBank.container.GetSprite("LeaguePromotionSticker");
            item.thumbnail.preserveAspect = false;

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.buttonText.text = "View";
            item.button.onClick.AddListener(() =>
            {
                inBoxBarClickedSignal.Dispatch(item);
                audioService.PlayStandardClick();
            });

            item.transform.SetParent(listContainer, false);
            item.skinLink.InitPrefabSkin();
            inBoxBars.Add(item.msgId, item);
        }

        public void AddMessages(Dictionary<string, InboxMessage> messages)
        {
            foreach (KeyValuePair<string, InboxMessage> obj in messages)
            {
                if (inBoxBars.ContainsKey(obj.Key))
                {
                    RemoveMessage(obj.Key);
                }

                InboxMessage msg = obj.Value;
                if (TimeUtil.unixTimestampMilliseconds >= msg.startTime &&
                    AddInboxBarFnMap.ContainsKey(msg.type))
                {
                    AddInboxBarFnMap[msg.type].Invoke(msg);
                }
            }

            Sort();
            //processing.SetActive(false);
        }

        public void RemoveMessage(string messageId)
        {
            InboxBar bar = inBoxBars[messageId];
            inBoxBars.Remove(messageId);
            GameObject.Destroy(bar.gameObject);
            Sort();
        }

        public void ClearInbox()
        {
            foreach (var bar in inBoxBars)
            {
                GameObject.Destroy(bar.Value.gameObject);
            }

            inBoxBars.Clear();
        }
    }
}
