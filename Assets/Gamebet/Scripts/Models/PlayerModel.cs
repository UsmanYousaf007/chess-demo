/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 15:14:14 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using System.Collections.Generic;

namespace TurboLabz.Gamebet
{
    public class PlayerModel : IPlayerModel
    {
        public string id { get; set; }
        public string tag { get; set; }
        public string name { get; set; }
        public string countryId { get; set; }
        public Sprite profilePicture { get; set; }
        public Sprite profilePictureBorder { get; set; } 
        public Sprite profilePictureFB { get; set; }
        public long currency1 { get; set; } // coins
        public long currency2 { get; set; } // bucks
        public long currency1Winnings { get; set; }
        public int xp { get; set; }
        public int level { get; set; }
        public string leagueId { get; set; }
        public string eloDivision { get; set; }
        public int eloScore { get; set; }

        // The keys of the dictionary are the IDs of the rooms.
        public IDictionary<string, RoomRecord> roomRecords { get; set; }

        public bool isSocialNameSet { get; set; }
        public IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

        public bool hasExternalAuth
        {
            get
            {
                return (externalAuthentications.Count > 0);
            }
        }

        public int totalGamesWon
        {
            get
            {
                int count = 0;

                foreach (var roomRecord in roomRecords.Values)
                {
                    count += roomRecord.gamesWon;
                }

                return count;
            }
        }

        public int totalGamesLost
        {
            get
            {
                int count = 0;

                foreach (var roomRecord in roomRecords.Values)
                {
                    count += roomRecord.gamesLost;
                }

                return count;
            }
        }

        public int totalGamesDrawn
        {
            get
            {
                int count = 0;

                foreach (var roomRecord in roomRecords.Values)
                {
                    count += roomRecord.gamesDrawn;
                }

                return count;
            }
        }

        public int totalGames
        {
            get
            {
                int count = 0;

                foreach (var roomRecord in roomRecords.Values)
                {
                    count += roomRecord.gamesWon + roomRecord.gamesLost + roomRecord.gamesDrawn;
                }

                return count;
            }
        }

        public PublicProfile publicProfile
        {
            get
            {
                PublicProfile profile;
                profile.id = id;
                profile.name = name;
                profile.countryId = countryId;
                profile.level = level;
                profile.leagueId = leagueId;
                profile.roomRecords = roomRecords;
                profile.externalAuthentications = externalAuthentications;
                profile.profilePicture = profilePicture;
                profile.profilePictureBorder = profilePictureBorder;
                profile.eloDivision = eloDivision;
                profile.eloScore = eloScore;

                return profile;
            }
        }
    }

    // TODO: Evaluate converting PlayerProfile into a class rather than a
    // struct.
    public struct PublicProfile
    {
        public string id;
        public string name;
        public string countryId;
        public int level;
        public string leagueId;
        public string eloDivision;
        public int eloScore;
  
        // The keys of the dictionary are the IDs of the rooms.
        public IDictionary<string, RoomRecord> roomRecords;

        public Sprite profilePicture;
        public Sprite profilePictureBorder;

        public IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications { get; set; }

        public bool hasExternalAuth
        {
            get
            {
                return (externalAuthentications.Count > 0);
            }
        }
    }

    // The id field is also present in the RoomRecord since a room record must
    // always be able to refer to the room it belongs to from within itself.
    public struct RoomRecord
    {
        public string id;
        public int gamesWon;
        public int gamesLost;
        public int gamesDrawn;
        public int trophiesWon;
        public string roomTitleId;
    }

    public struct ExternalAuthData
    {
        public string id;
    }
}
