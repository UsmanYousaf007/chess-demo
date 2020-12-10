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
    [System.Serializable]
    public struct PlayerEntry{
        public string id;
        public TMP_Text name;
        public TMP_Text place;
    }

    [CLSCompliant(false)]
    public class LobbyTickerView : View
    {
        public TMP_Text onlinePlayersCount;
        public TMP_Text gamesTodayCount;

        public PlayerEntry[] entries;
        
        //Models
        [Inject] public ILeaguesModel leaguesModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        //Signals

        //Services
        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
        }

        public void UpdateView(LobbyVO lobbyVO)
        {
            onlinePlayersCount.text = "Active Players " + FormatUtil.AbbreviateNumber(lobbyVO.onlineCount);
            gamesTodayCount.text = "Games Today " + FormatUtil.AbbreviateNumber(lobbyVO.onlineCount);
        }

    }
}
