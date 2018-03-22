/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-30 12:31:17 UTC+05:00
/// 
/// @description
/// [add_description_here]

// .NET namespaces

// Unity namespaces

// Library namespaces

// User defined namespaces

namespace TurboLabz.Gamebet
{
    public struct PreMatchmakingVO
    {
        public IPlayerModel playerModel;

        public long currency1;
        public long currency2;
        public RoomSetting roomInfo;
        public PublicProfile playerPublicProfile;
    }
}
