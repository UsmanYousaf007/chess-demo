using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using GameAnalyticsSDK;

namespace TurboLabz.Chess
{
    public partial class ChessAiService : IChessAiService
    {
        private const int BLUNDER_RELATIVE_SCORE = 900;
        private const int MISTAKE_RELATIVE_SCORE = 120;
        private const int PERFECT_RELATIVE_SCORE = 0;

        public IPromise<FileRank, FileRank, string> AnalyseMove(AiMoveInputVO vo)
        {
            return AddToQueue(_AnalyseMove, vo, 1);
        }

        private void _AnalyseMove(AiMoveInputVO vo)
        {
            NewGame(ChessAiConfig.SF_ANALYSIS_MULTIPV);
            SetPosition(vo.fen);
            aiMoveInputVO = vo;
            routineRunner.StartCoroutine(GetAiResult());
        }

        private void GetMoveAnalysis()
        {
            try
            {
                var strength = 0.0f;
                var moveQuality = MoveQuality.NORMAL;
                var from = aiMoveInputVO.lastPlayerMove.from;
                var to = aiMoveInputVO.lastPlayerMove.to;
                var totalMoveCount = aiSearchResultMovesList.Count;
                var advantageScore = 0;
                var playerMoveScore = 0;
                var bestMoveScore = 0;

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

                        //strength = CalculateMoveStrength(moveFoundIndex);
                        LogUtil.Log($"relativeMoveScore: {relativeMoveScore} | moveQuality: {moveQuality}", "yellow");
                    }

                    advantageScore = moveFoundIndex != -1 ? scores[moveFoundIndex] : scores[scores.Count - 1];
                    bestMoveScore = scores[0];
                    playerMoveScore = moveFoundIndex != -1 ? scores[moveFoundIndex] : 0;
                    var bestMove = playerMadeTheBestMove ? aiSearchResultMovesList[moveFoundIndex] : aiSearchResultMovesList[0];
                    from = chessService.GetFileRankLocation(bestMove[0], bestMove[1]);
                    to = chessService.GetFileRankLocation(bestMove[2], bestMove[3]);
                }

                lastDequeuedMethod.promise.Dispatch(from, to, $"{moveQuality}|{strength}|{advantageScore}|{playerMoveScore}|{bestMoveScore}");
            }
            catch (System.Exception ex)
            {
                lastDequeuedMethod.promise.Dispatch(aiMoveInputVO.lastPlayerMove.from, aiMoveInputVO.lastPlayerMove.to, $"{MoveQuality.NORMAL}|-1|0|0|0");
            }
            finally
            {
                lastDequeuedMethod.promise = null;
                lastDequeuedMethod = null;
            }
        }
    }
}
