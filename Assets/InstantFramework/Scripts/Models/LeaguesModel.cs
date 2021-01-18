/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;
using System.Linq;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeaguesModel : ILeaguesModel
    {
        public Dictionary<string, League> leagues { get; set; }

        [Inject] public IPlayerModel playerModel { get; set; }


        [PostConstruct]
        public void PostConstruct()
        {
            leagues = new Dictionary<string, League>();
        }

        public League GetCurrentLeagueInfo()
        {
            var rv = new League();
            var playerLeague = playerModel.league.ToString();

            if (leagues.ContainsKey(playerLeague))
            {
                rv = leagues[playerLeague];
            }

            return rv;
        }

        public League GetLeagueInfo(int league)
        {
            var rv = new League();
            var leagueStr = league.ToString();

            if (leagues.ContainsKey(leagueStr))
            {
                rv = leagues[leagueStr];
            }

            return rv;
        }

    }
}
