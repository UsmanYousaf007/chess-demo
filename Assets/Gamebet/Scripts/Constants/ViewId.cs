/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-11 15:46:11 UTC+05:00
/// 
/// @description
/// [add_description_here]

namespace TurboLabz.Gamebet
{
    public enum ViewId
    {
        // Gamebet view IDs
        NONE = -1,
        SPLASH,
        LOADING,
        AUTHENTICATION,
        SET_PLAYER_SOCIAL_NAME,
        RETRY_CONNECTION,
        UPDATE_APP,
        LOBBY,
        PLAYER_PROFILE,
        SHOP,
        INVENTORY,
        ROOMS,
        MATCHMAKING,
        END_GAME,
        MP_GAME,
        CPU_MENU,
        CPU_GAME,
        CPU_STATS
    }

    public enum SubShopViewId
    {
        LOOTBOXES,
        AVATARS,
        CHESSSKINS,
        CHAT,
        CURRENCY
    }

    public enum SubInventoryViewId
    {
        AVATARS,
        CHESSSKINS,
        LOOT
    }
}
