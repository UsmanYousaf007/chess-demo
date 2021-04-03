using strange.extensions.promise.api;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public partial class ChessAiService : IChessAiService
    {
        private const int BLUNDER_RELATIVE_SCORE = 200;
        private const int MISTAKE_RELATIVE_SCORE = 40;
        private const int PERFECT_RELATIVE_SCORE = 0;

        public IPromise<FileRank, FileRank, string> AnalyseMove(AiMoveInputVO vo)
        {
            return AddToQueue(_GetAiMoveStrength, vo);
        }

        private void GetMoveAnalysis()
        {
            var strength = 0.0f;
            var moveQuality = MoveQuality.NORMAL;
            var from = aiMoveInputVO.lastPlayerMove.from;
            var to = aiMoveInputVO.lastPlayerMove.to;
            var totalMoveCount = aiSearchResultMovesList.Count;

            if (totalMoveCount > 0)
            {
                var moveString = aiMoveInputVO.lastPlayerMove.MoveToString(from, to);
                int moveFoundIndex = GetMoveIndex(moveString);
                var playerMadeTheBestMove = false;

                if (moveFoundIndex == -1)
                {
                    moveQuality = MoveQuality.BLUNDER;
                    strength = 0;
                    LogUtil.Log($"moveQuality: {moveQuality}", "yellow");
                }
                else if (moveFoundIndex == 0)
                {
                    moveQuality = MoveQuality.PERFECT;
                    strength = 1;
                    playerMadeTheBestMove = true;
                    LogUtil.Log($"moveQuality: {moveQuality}", "yellow");
                }
                else
                {
                    var relativeMoveScore = scores[0] - scores[moveFoundIndex];

                    if (relativeMoveScore == PERFECT_RELATIVE_SCORE)
                    {
                        moveQuality = MoveQuality.PERFECT;
                        playerMadeTheBestMove = true;
                    }
                    else if (relativeMoveScore > PERFECT_RELATIVE_SCORE && relativeMoveScore < MISTAKE_RELATIVE_SCORE)
                    {
                        moveQuality = MoveQuality.NORMAL;
                    }
                    else if (relativeMoveScore >= MISTAKE_RELATIVE_SCORE && relativeMoveScore < BLUNDER_RELATIVE_SCORE)
                    {
                        moveQuality = MoveQuality.MISTAKE;
                    }
                    else if (relativeMoveScore >= BLUNDER_RELATIVE_SCORE)
                    {
                        moveQuality = MoveQuality.BLUNDER;
                    }

                    strength = CalculateMoveStrength(moveFoundIndex);
                    LogUtil.Log($"relativeMoveScore: {relativeMoveScore} | moveQuality: {moveQuality}","yellow");
                }

                var bestMove = playerMadeTheBestMove ? aiSearchResultMovesList[moveFoundIndex] : aiSearchResultMovesList[0];
                from = chessService.GetFileRankLocation(bestMove[0], bestMove[1]);
                to = chessService.GetFileRankLocation(bestMove[2], bestMove[3]);
            }

            lastDequeuedMethod.promise.Dispatch(from, to, $"{moveQuality}|{strength}");
            lastDequeuedMethod.promise = null;
            lastDequeuedMethod = null;
        }
    }
}
