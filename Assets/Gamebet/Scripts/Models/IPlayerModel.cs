/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 15:09:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public interface IPlayerModel
    {
        string id { get; set; }
        string tag { get; set; }
        string name { get; set; }
        string countryId { get; set; }
        Sprite profilePicture { get; set; }
        Sprite profilePictureBorder { get; set; } 
        Sprite profilePictureFB { get; set; }
        long currency1 { get; set; }
        long currency2 { get; set; }
        long currency1Winnings { get; set; }
        int xp { get; set; }
        int level { get; set; }
        string leagueId { get; set; }

        // The keys of the dictionary are the IDs of the rooms.
        IDictionary<string, RoomRecord> roomRecords { get; set; }

        bool isSocialNameSet { get; set; }
        IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }
        bool hasExternalAuth { get; }
        int totalGamesWon { get; }
        int totalGamesLost { get; }
        int totalGamesDrawn { get; }
        int totalGames { get; }
        PublicProfile publicProfile { get; }
    }
}
