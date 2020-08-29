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

        public Transform listContainer;
        public GameObject sectionHeader;
        public GameObject inBoxBarPrefab;
        public GameObject emptyInBoxStrip;
        public Text emptyInboxLabel;

        public Text heading;
        public Text stripHeading;

        // Player bar click signal
        [HideInInspector]
        public Signal<InboxBar> inBoxBarClickedSignal = new Signal<InboxBar>();
        private Dictionary<string, InboxBar> inBoxBars = new Dictionary<string, InboxBar>();
        private Dictionary<string, Action<InboxMessage>> AddInboxBarFnMap = new Dictionary<string, Action<InboxMessage>>();

        public void Init()
        {
            heading.text = localizationService.Get(LocalizationKey.INBOX_HEADING);
            stripHeading.text = localizationService.Get(LocalizationKey.INBOX_SECTION_HEADER_REWARDS);
            emptyInboxLabel.text = localizationService.Get(LocalizationKey.INBOX_EMPTY_INBOX_LABEL);

            sectionHeader.gameObject.SetActive(false);
            emptyInBoxStrip.gameObject.SetActive(false);

            AddInboxBarFnMap.Add("RewardTournamentEnd", AddTournamentRewardBar);
            AddInboxBarFnMap.Add("RewardDailySubscription", AddDailySubscriptionRewardBar);
            AddInboxBarFnMap.Add("RewardDailyLeague", AddDailyLeagueRewardBar);
            AddInboxBarFnMap.Add("RewardLeaguePromotion", AddLeaguePromotionRewardBar);

            Sort();
        }

        public void Show()
        {
            gameObject.SetActive(true);
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

            sectionHeader.gameObject.SetActive(items.Count > 0);
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

            item.thumbnailBg.sprite = TournamentAssetsContainer.Load().GetTile(msg.tournamentType);
            item.headingText.text = msg.heading;
            item.subHeadingText.text = msg.subHeading;
            item.thumbnail.sprite = TournamentAssetsContainer.Load().GetSticker(msg.tournamentType);

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.button.onClick.AddListener(() => inBoxBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            inBoxBars.Add(item.msgId, item);
        }

        public void AddDailySubscriptionRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = msg.heading;
            item.subHeadingText.text = msg.subHeading;
            item.thumbnail.sprite = SpriteBank.container.GetSprite("SubscriptionSticker");

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.button.onClick.AddListener(() => inBoxBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            inBoxBars.Add(item.msgId, item);
        }

        public void AddDailyLeagueRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;

            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = msg.heading;
            item.subHeadingText.text = msg.subHeading;
            item.thumbnail.sprite = SpriteBank.container.GetSprite("RankSticker");

            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.button.onClick.AddListener(() => inBoxBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
            inBoxBars.Add(item.msgId, item);
        }

        public void AddLeaguePromotionRewardBar(InboxMessage msg)
        {
            GameObject obj = GameObject.Instantiate(inBoxBarPrefab);
            InboxBar item = obj.GetComponent<InboxBar>();

            item.timeStamp = 0;
            
            item.thumbnailBg.sprite = null;// (Resources.Load("PK") as Image).sprite;
            item.headingText.text = msg.heading;
            item.subHeadingText.text = msg.subHeading;
            item.thumbnail.sprite = SpriteBank.container.GetSprite("LeaguePromotionSticker");
            
            item.timeStamp = msg.timeStamp;
            item.msgId = msg.id;

            item.button.onClick.AddListener(() => inBoxBarClickedSignal.Dispatch(item));

            item.transform.SetParent(listContainer, false);
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
        }

        public void RemoveMessage(string messageId)
        {
            InboxBar bar = inBoxBars[messageId];
            inBoxBars.Remove(messageId);
            GameObject.Destroy(bar.gameObject);
            Sort();
        }
    }
}
