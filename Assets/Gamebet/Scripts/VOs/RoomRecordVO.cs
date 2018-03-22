/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-28 23:31:10 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    // The id field is also present in the RoomRecordVO since a room record must
    // always be able to refer to the room it belongs to from within itself.
    //
    // Also note that we have another RoomRecord class but we are not using it
    // here. This is because we need some aditional info to pass to the view and
    // the RoomRecord class does not contain that. Neither should we modify that
    // class to add in the extra information because the RoomRecord class is
    // solely for the purpose of keeping player specific room data. Views might
    // need arbitrary extra information hence views are passed value
    // objects (VOs).
    public struct RoomRecordVO
    {
        public string id;
        public long gameDuration;
        public int gamesWon;
        public int gamesLost;
        public int gamesDrawn;
        public int trophiesWon;
        public string roomTitleId;
    }
}
