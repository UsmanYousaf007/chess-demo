/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-20 12:29:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

using UnityEngine;

using System.Collections.Generic;

using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public partial class ChessAiService
    {
        private List<string> aiSearchResultScoresList;
        private List<string> aiSearchResultMovesList;
        private List<int> scores;

        /// <summary>
        /// This is called when the search result strings from the plugin are ready
        /// </summary>
        private void SelectMove()
        {
            AiLog("AI selecting move.");
            ParseResults();

            // For any other move, emulate a human player by thinking
            // 1 dimensionally.
            if (MakeOpeningMoves() ||
                MakeOnlyMoveAvailable() ||
                MakePanicMove() ||
                MakeReactionaryCaptureMove() ||
                MakeReactionaryEvasiveMove())
            {
                return;
            }

            AiLog("No special situation, making the worst move.");
            DispatchMove(scores.Count - 1);
        }


        /// <summary>
        /// We parse the move and dispatch the move
        /// </summary>
        private void DispatchMove(int index, bool panicMove = false)
        {
            index = Math.Min(aiSearchResultMovesList.Count - 1, index);
            string selectedMove = aiSearchResultMovesList[index];

            FileRank from = chessService.GetFileRankLocation(selectedMove[0], selectedMove[1]);
            FileRank to = chessService.GetFileRankLocation(selectedMove[2], selectedMove[3]);;

            AiLog("[" + chessService.GetAlgebraicLocation(from) + " " + chessService.GetAlgebraicLocation(to) + "]");

            string promo = null;

            if (selectedMove.Length == 5)
            {
                promo = selectedMove[4].ToString();
            }

            // If we are not making the best possible Ai move, then we must
            // check for certain events that we want to disallow across the board.
            // If such an event does occur, we make the best move instead.
            if (overrideStrength != AiOverrideStrength.STUPID && index > 0 && !panicMove) 
            {
                if (CancelMoveDueToFeedsOrWeakExchanges(from, to, promo, index) ||
                    CancelMoveDueToFreeCaptureAvailable(to, index) ||
                    CancelMoveDueToWeakKingMove(from, index) ||
                    CancelMoveDueToRookRestriction(from, index) ||
                    CancelMoveDueToNonPromotion(promo))
                {
                    DispatchMove(index - 1);
                    return;
                }
            }

            aiMovePromise.Dispatch(from, to, promo);
        }

        private bool CancelMoveDueToNonPromotion(string promo)
        {
            // Ai Move list has queen promotion and you are not making it
            if (promo == null || promo.ToLower() != ChessPieceName.BLACK_QUEEN)
            {
                for (int i = 0; i < aiSearchResultMovesList.Count - 1; ++i)
                {
                    if (aiSearchResultMovesList[i].Length == 5)
                    {
                        string pieceName = aiSearchResultMovesList[i][4].ToString();

                        if (pieceName.ToLower() == "q")
                        {
                            AiLog("Queen promo available. Cancelling.");
                            return true;           
                        }
                    }
                }
            }

            return false;
        }

        private bool CancelMoveDueToFreeCaptureAvailable(FileRank to, int index)
        {
            List<FileRank> freeCaptureLocations = chessService.GetProfitableCapturesAvailable(aiMoveInputVO.aiColor);

            if (freeCaptureLocations.Count > 0)
            {
                // First check if we're already making a move to one of the free capture locations.
                foreach (FileRank location in freeCaptureLocations)
                {
                    if (location.file == to.file &&
                        location.rank == to.rank)
                    {
                        return false;
                    }
                }

                // Looks like there is a free capture and we're not making it. So lets select another move.
                AiLog("Profitable capture. Cancelling.");
                return true;
            }

            return false;
        }

        private bool CancelMoveDueToFeedsOrWeakExchanges(FileRank from, FileRank to, string promo, int index)
        {
            if (chessService.WillMoveCauseWeakExchangeOrFeed(from, to, promo))
            {
                AiLog("Weak exchange or feed. " + chessService.GetAlgebraicLocation(from) + " " + chessService.GetAlgebraicLocation(to));
                return true;
            }

            return false;
        }

        private bool CancelMoveDueToWeakKingMove(FileRank from, int index)
        {
            if (IsMovingPiece(from, ChessPieceName.BLACK_KING))
            {
                AiLog("King was moved non-optimally.");
                return true;
            }

            return false;
        }

        private bool CancelMoveDueToRookRestriction(FileRank from, int index)
        {
            if (aiMoveInputVO.aiMoveNumber <= AiConfig.ROOK_RESTRICTION_MOVE_COUNT &&
                IsMovingPiece(from, ChessPieceName.BLACK_ROOK))
            {
                AiLog("Rook was moved non-optimally.");
                return true;
            }

            return false;
        }

        private bool MakeReactionaryCaptureMove()
        {
            // Let's examine the immediate enemy move like an average player would...
            bool landingDefended = chessService.IsSquareDefended(aiMoveInputVO.lastPlayerMove.to,
                                                                 aiMoveInputVO.playerColor);
            
            ChessMove cheapestAttackingMove = chessService.GetCheapestAttackingMoveToSquare(aiMoveInputVO.lastPlayerMove.to);

            /////////////////////////////////////////////////////////////////
            // Consider attacking the square where the enemy moved

            // Has that piece landed on an undefended square
            if (!landingDefended)
            {
                // If I can attack, Then make the capture using the cheapest piece
                if (cheapestAttackingMove != null)
                {
                    AiLog("Captured undefended at [" + chessService.GetAlgebraicLocation(aiMoveInputVO.lastPlayerMove.to) + "]");
                    int index = GetAiMoveIndex(cheapestAttackingMove);
                    DispatchMove(index);
                    return true;
                }
            }
            else
            {
                // Has that piece landed on a square that I can attack with a piece
                // of a cheaper or equal value? If so, then attack!
                if (cheapestAttackingMove != null)
                {
                    int attackingPieceValue = GetValueForPiece(cheapestAttackingMove.piece.name);
                    int victimPieceValue = GetValueForPiece(aiMoveInputVO.lastPlayerMove.piece.name);

                    // A chance of making the exchange if values are equal
                    bool exchange = false;
                    exchange = (attackingPieceValue < victimPieceValue) ||
                        ((attackingPieceValue == victimPieceValue) && RollPercentageDice(AiConfig.PIECE_EXCHANCE_CHANCE));

                    if (exchange)
                    {
                        AiLog("Made exchange.");
                        int index = GetAiMoveIndex(cheapestAttackingMove);
                        DispatchMove(index);
                        return true;
                    }
                }
            }

            return false;
        }
            
        private bool MakeReactionaryEvasiveMove()
        {
            // Find all the enemy moves where a capture is possible
            List<ChessMove> enemyCaptureMoves = chessService.GetCaptureMoves(aiMoveInputVO.lastPlayerMove.to);

            // Lets go through these moves and see if the enemy has an advantage exchange or simply can
            // can capture an undefended piece
            foreach(ChessMove enemyCaptureMove in enemyCaptureMoves)
            {
                bool isPieceDefendedByMe = chessService.IsSquareDefended(enemyCaptureMove.to, aiMoveInputVO.aiColor);
                bool evade = false;

                // Check for advantage exchange
                if (isPieceDefendedByMe)
                {
                    int attackingPieceValue = GetValueForPiece(enemyCaptureMove.piece.name);
                    int myPieceValue = GetValueForPiece(
                        aiMoveInputVO.squares[enemyCaptureMove.to.file, enemyCaptureMove.to.rank].piece.name);

                    if (attackingPieceValue < myPieceValue)
                    {
                        AiLog("Enemy can capture better piece. [" + chessService.GetAlgebraicLocation(enemyCaptureMove.to) + "] Evading!");
                        evade = true;
                    }
                }
                // Escape attack on undefended piece
                else
                {
                    AiLog("Enemy can capture undefended piece. [" + chessService.GetAlgebraicLocation(enemyCaptureMove.to) + "] Evading!");
                    evade = true;
                }

                if (evade)
                {
                    int aiEvasionMoveIndex = GetAiMoveIndexByMoveOrigin(enemyCaptureMove.to);

                    if (aiEvasionMoveIndex != -1)
                    {
                        AiLog("Found evasion index.");
                        DispatchMove(aiEvasionMoveIndex);
                    }
                    else
                    {
                        AiLog("No evasion index.");
                        DispatchMove(0);
                    }

                    return true;
                }
            }

            return false;
        }

        private bool MakeOpeningMoves()
        {
            // For the first 4 moves we select a random opening move or one of 3 'pro' opening
            // moves based on if we rolled a best move
            if (aiMoveInputVO.aiMoveNumber <= AiConfig.OPENING_MOVES_COUNT)
            {
                int selectCount = Math.Min(scores.Count, AiConfig.OPENING_MOVES_SELECT_COUNT);
                int openingMoveIndex = UnityEngine.Random.Range(0, selectCount);

                AiLog("Made opening move.");
                DispatchMove(openingMoveIndex);
                return true;
            }

            return false;
        }

        private bool MakeOnlyMoveAvailable()
        {
            // If there is just one move, make it and get out
            if (scores.Count == 1)
            {
                AiLog("Just 1 move available.");
                DispatchMove(0);
                return true;
            }

            return false;
        }

        private bool MakePanicMove()
        {
            // Apply panic attack and leave early. This means that we also
            // ignore our silly move filters below and make a move regardless
            // since the player is 'paniced'.
            bool panic = false;
            double clockSeconds = aiMoveInputVO.opponentTimer.TotalSeconds;

            // Panic also applies at 1 min and 30 seconds for ONE MINUTE games
            // because of their inherent mindset.
            if (aiMoveInputVO.timeControl == AiTimeControl.ONE_MINUTE)
            {
                if (clockSeconds < 10 )
                {
                    panic = RollPercentageDice(AiConfig.TEN_SECOND_PANIC_CHANCE);
                }
                else if (clockSeconds < 30)
                {
                    panic = RollPercentageDice(AiConfig.THIRTY_SECOND_PANIC_CHANCE);
                }
                else if (clockSeconds < 60)
                {
                    panic = RollPercentageDice(AiConfig.ONE_MIN_PANIC_CHANCE);
                }
            }
            else
            {
                // It applies to the rest of the time controls only at 10 seconds due 
                // to the mindset when the clock is so low.
                if (clockSeconds < 10 )
                {
                    panic = RollPercentageDice(AiConfig.TEN_SECOND_PANIC_CHANCE);
                }
            }

            if (panic)
            {
                AiLog("Rolled a panic move.");
                DispatchMove(AiConfig.PANIC_MOVE_INDEX, true);
                return true;
            }

            return false;
        }

        private bool IsMovingPiece(FileRank from, string pieceName)
        {
            ChessSquare square = aiMoveInputVO.squares[from.file, from.rank];
            ChessPiece piece = square.piece;

            if (piece == null)
            {
                return false;
            }

            if (piece.name.ToLower() == pieceName)
            { 
                return true;
            }

            return false;
        }

        private int GetAiMoveIndexByMoveOrigin(FileRank origin)
        {
            for (int i = aiSearchResultMovesList.Count - 1; i >= 0; --i)
            {
                string potentialMatch = aiSearchResultMovesList[i];

                FileRank aiOrigin = chessService.GetFileRankLocation(potentialMatch[0], potentialMatch[1]);

                if (aiOrigin.file == origin.file &&
                    aiOrigin.rank == origin.rank)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetAiMoveIndex(ChessMove move)
        {
            for (int i = 0; i < aiSearchResultMovesList.Count; ++i)
            {
                string potentialMatch = aiSearchResultMovesList[i];

                FileRank from = chessService.GetFileRankLocation(potentialMatch[0], potentialMatch[1]);
                FileRank to = chessService.GetFileRankLocation(potentialMatch[2], potentialMatch[3]);

                if (move.from.file == from.file &&
                    move.from.rank == from.rank &&
                    move.to.file == to.file &&
                    move.to.rank == to.rank)
                {
                    return i;
                }
            }

            AiLog("Not in Ai list.");
            return 0;
        }

        private void ParseResults()
        {
            // Read the scores returned
            aiSearchResultScoresList = new List<string>(aiSearchResultScoresStr.Split(','));
            aiSearchResultScoresList.RemoveAt(0); // Gets rid of the label
            scores = new List<int>();

            foreach (string score in aiSearchResultScoresList)
            {
                scores.Add(int.Parse(score));
            }

            // Read the moves returned
            aiSearchResultMovesList = new List<string>(aiSearchResultMovesStr.Split(','));
            aiSearchResultMovesList.RemoveAt(0); // Gets rid of the label
        }

        private int GetValueForPiece(string pieceName)
        {
            pieceName = pieceName.ToLower();

            if (pieceName == ChessPieceName.BLACK_KING)
            {
                return AiConfig.KING_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_QUEEN)
            {
                return AiConfig.QUEEN_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_ROOK)
            {
                return AiConfig.ROOK_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_BISHOP)
            {
                return AiConfig.BISHOP_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_KNIGHT)
            {
                return AiConfig.KNIGHT_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_PAWN)
            {
                return AiConfig.PAWN_VALUE;
            }

            Assertions.Assert(false, "Unknown piece name.");
            return 0;
        }

        private bool RollPercentageDice(int percentChance)
        {
            return (UnityEngine.Random.Range(0, 100) < percentChance);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void AiLog(string message)
        {
            LogUtil.Log(this.GetType().Name + ": AI Log: " + message, "yellow");
        }
    }
}
