using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public partial class ChessAiService
    {
        readonly int[] DELTA_CLASSIFICATIONS = { 50000, 1000, 500, 100, 0 };
        readonly float[] DELTA_PERCENTILES = { 0.0f, 0.0f, 0.25f, 0.5f, 0.75f };
        const float PERCENTILE_DELTA = 0.25f;
        const float MIN_PERCENTAGE = 0.1f;
        const float MAX_PERCENTAGE = 1.0f;

        private void GetMoveStrength()
        {
            float percentage = 0.0f;
            FileRank from = aiMoveInputVO.lastPlayerMove.from;
            FileRank to = aiMoveInputVO.lastPlayerMove.to;
            int totolMoveCount = aiSearchResultMovesList.Count - 1;

            if (totolMoveCount > 0)
            {
                string moveString = aiMoveInputVO.lastPlayerMove.MoveToString(from, to);
                int moveFoundIndex = -1;

                for (int i = 0; i <= totolMoveCount; ++i)
                {
                    LogUtil.Log("j:" + i + " MOVES : " + aiSearchResultMovesList[i] + " SCORE : " + scores[i]);

                    if (string.Equals(moveString, aiSearchResultMovesList[i]))
                    {
                        moveFoundIndex = i;
                        break;
                    }
                }

                LogUtil.Log("moveString : " + moveString + " totolMoveCount : " + totolMoveCount + " moveFoundIndex:" + moveFoundIndex);

                if (moveFoundIndex == -1)
                {
                    percentage = MIN_PERCENTAGE;
                }
                else if (moveFoundIndex == 0)
                {
                    percentage = MAX_PERCENTAGE;
                }
                else
                {
                    int playerMoveClassificationIndex = 0;
                    for (int i = 0; i < DELTA_CLASSIFICATIONS.Length; i++)
                    {
                        if (scores[0] - scores[moveFoundIndex] >= DELTA_CLASSIFICATIONS[i])
                        {
                            playerMoveClassificationIndex = i;
                            break;
                        }
                    }

                    var classifiedScoresList = new List<int>();
                    foreach (var score in scores)
                    {
                        if (scores[0] - score >= DELTA_CLASSIFICATIONS[playerMoveClassificationIndex] &&
                            playerMoveClassificationIndex > 0 &&
                            scores[0] - score < DELTA_CLASSIFICATIONS[playerMoveClassificationIndex - 1])
                        {
                            classifiedScoresList.Add(score);
                        }
                    }

                    var playerClassifiedScoreListIndex = classifiedScoresList.FindIndex(s => s == scores[moveFoundIndex]);

                    LogUtil.Log(string.Format("playerMoveClassificationIndex: {0} classifiedScoresList.Count: {1} playerClassifiedScoreListIndex: {2}",
                        playerMoveClassificationIndex, classifiedScoresList.Count, playerClassifiedScoreListIndex));

                    percentage = DELTA_PERCENTILES[playerMoveClassificationIndex] +
                        (((float)(classifiedScoresList.Count - playerClassifiedScoreListIndex) /
                        (float)(classifiedScoresList.Count)) *
                        PERCENTILE_DELTA);
                    percentage = percentage < MIN_PERCENTAGE ? MIN_PERCENTAGE : percentage;
                }
            }

            lastDequeuedMethod.promise.Dispatch(from, to, percentage.ToString());
            lastDequeuedMethod.promise = null;
            lastDequeuedMethod = null;
        }

        private void GetBestMove()
        {
            string moveString = aiMoveInputVO.lastPlayerMove.MoveToString(aiMoveInputVO.lastPlayerMove.from, aiMoveInputVO.lastPlayerMove.to);
            int moveFoundIndex = -1;

            for (int i = 0; i < aiSearchResultMovesList.Count; ++i)
            {
                LogUtil.Log("j:" + i + " MOVES : " + aiSearchResultMovesList[i] + " SCORE : " + scores[i]);

                if (string.Equals(moveString, aiSearchResultMovesList[i]))
                {
                    moveFoundIndex = i;
                    break;
                }
            }

            LogUtil.Log("moveString : " + moveString + " totolMoveCount : " + aiSearchResultMovesList.Count + " moveFoundIndex:" + moveFoundIndex);

            bool playerMadeTheBestMove = false;
            if (moveFoundIndex >= 0
                && scores[0] == scores[moveFoundIndex])
            {
                playerMadeTheBestMove = true;
            }

            var selectedMove = playerMadeTheBestMove ? aiSearchResultMovesList[moveFoundIndex] : aiSearchResultMovesList[0];
            var from = chessService.GetFileRankLocation(selectedMove[0], selectedMove[1]);
            var to = chessService.GetFileRankLocation(selectedMove[2], selectedMove[3]);
            var piece = chessService.GetPieceAtLocation(from);

            //if piece is null then it means player had moved it in his last move
            //getting piece info from player's last move
            if (piece == null)
            {
                piece = aiMoveInputVO.squares[aiMoveInputVO.lastPlayerMove.to.file, aiMoveInputVO.lastPlayerMove.to.rank].piece;
            }

            var pieceName = piece.name;
            pieceName = string.Format("{0}{1}", piece.color == ChessColor.BLACK ? 'b' : 'W', piece.name.ToLower());

            lastDequeuedMethod.promise.Dispatch(from, to, pieceName);
            lastDequeuedMethod.promise = null;
            lastDequeuedMethod = null;
        }
    }
}
