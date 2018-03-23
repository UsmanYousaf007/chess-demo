/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 10:39:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.promise.api;

namespace TurboLabz.Chess
{
    public interface IChessAiService
    {
        void NewGame();
        void Shutdown();
        void SetPosition(string FEN);
        IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo, AiOverrideStrength overrideStrength);
        IPromise<FileRank, FileRank, string> GetAiMove(AiMoveInputVO vo);
        bool isInitialized { get; set; }
    }
}
