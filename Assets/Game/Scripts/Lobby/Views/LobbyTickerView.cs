/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LobbyTickerView : View
    {
        public TMP_Text onlinePlayersCount;
        public TMP_Text gamesTodayCount;
        public Ticker ticker;

        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Signals

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        public GameObject tickerItemPrefab;
        private GameObjectsPool itemsPool;
        public Transform parent;
        public TMP_Text noPlayersText;

        private List<TickerItem> playerItems = new List<TickerItem>();

        public void Init()
        {
        }

        public void UpdateView(LobbyVO lobbyVO)
        {
            onlinePlayersCount.text = "Active Players " + FormatUtil.AbbreviateNumber(lobbyVO.onlineCount);
            gamesTodayCount.text = "Games Today " + FormatUtil.AbbreviateNumber(lobbyVO.gamesTodayCount);
            //TODO send the parameter
            //PopulateTicker();
        }

        void PopulateTicker()
        {
            //TODO send the parameter
            //Populate();
            ticker.StartTicker(playerItems);
        }

        private TickerItem AddPlayer()
        {
            GameObject obj = itemsPool.GetObject();
            TickerItem item = obj.GetComponent<TickerItem>();
            item.transform.SetParent(parent, false);
            return item;
        }

        public void Populate(int count)
        {
            if (count == 0)
            {
                noPlayersText.enabled = true;
            }
            else
            {
                noPlayersText.enabled = false;
                itemsPool = new GameObjectsPool(tickerItemPrefab, count);
            }
            //TODO send the parameter
            //PopulateEntries();
        }

        public void PopulateEntries(JoinedTournamentData joinedTournament)
        {
            ClearBars();

            int itemBarsCount = playerItems.Count;
            if (itemBarsCount < joinedTournament.entries.Count)
            {
                for (int i = itemBarsCount; i < joinedTournament.entries.Count; i++)
                {
                    playerItems.Add(AddPlayer());
                }
            }

            for (int i = 0; i < joinedTournament.entries.Count; i++)
            {
                var playerItem = playerItems[i];
                PopulateBar(playerItem);
            }
        }

        public void ClearBars()
        {
            for (int i = 0; i < playerItems.Count; i++)
            {
                playerItems[i].name.text = "";
                playerItems[i].place.text = "";
                itemsPool.ReturnObject(playerItems[i].gameObject);
            }

            playerItems.Clear();
        }

        private void PopulateBar(TickerItem playerBar)
        {
            //TODO send item value to populate  
            playerBar.Populate();
        }
    }
}
