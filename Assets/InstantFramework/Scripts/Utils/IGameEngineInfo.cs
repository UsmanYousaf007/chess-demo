/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-14 13:36:04 UTC+05:00
/// 
/// @description
/// This provides information about the game engine.

namespace TurboLabz.TLUtils
{
    public interface IGameEngineInfo
    {
        bool isInternetReachable { get; }
    }
}
