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
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public partial class ChessAiService
    {
        private List<string> aiSearchResultScoresList;
        private List<string> aiSearchResultMovesList;
        private List<int> scores;

        private bool filterMoves;

        private int GetSearchDepth()
        {
            // Upto 60% cpu strength will use a min search depth.
            // After that we will increase the search depth to 
            // somewhat match AiFactory's max stated ELO at max difficulty 12 (ELO 2100)
            // We get our ELO to depth from https://chess.stackexchange.com/questions/8123/stockfish-elo-vs-search-depth

            if (aiMoveInputVO.cpuStrengthPct < 0.6f)
            {
                return ChessAiConfig.SF_MIN_SEARCH_DEPTH;
            }

            int searchDepthRange = ChessAiConfig.SF_MAX_SEARCH_DEPTH - ChessAiConfig.SF_MIN_SEARCH_DEPTH;
            int searchDepth = ChessAiConfig.SF_MIN_SEARCH_DEPTH + Mathf.FloorToInt(aiMoveInputVO.cpuStrengthPct * searchDepthRange);

            return searchDepth;
        }

        /// <summary>
        /// This is called when the search result strings from the plugin are ready
        /// </summary>
        private void SelectMove()
        {
            if (aiMoveInputVO.isHint)
            {
                AiLog("Hint request, returning the best move");
                DispatchMove(0);
                return;
            }
            else if (aiMoveInputVO.cpuStrengthPct >= 1f)
            {
                AiLog("Epic max strength best move.");
                DispatchMove(0);
                return;
            }

            // There are two common sense filters:
            // 1) Premove filters that emulate a human reactionary move
            // 2) Postmove filters that remove stupid looking computer selected moves via our random move selection 
            //    in the case where a reactionary move is not made

            // These filters happen to create difficulty for beginner players. Therefore we will reduce the probability
            // of triggering these filters upto 50% computer strength.
            filterMoves = true;

            float filterOffProb = Mathf.Max((0.5f - aiMoveInputVO.cpuStrengthPct) * 2f, 0f);
            filterMoves = !RollPercentageDice(Mathf.FloorToInt(filterOffProb * 100));

            // For any other move, emulate a human player by thinking
            // 1 dimensionally.
            if (filterMoves)
            {
                AiLog("Filtering moves");
                if (
                    MakeOpeningMoves() ||
                    MakeOnlyMoveAvailable() ||
                    MakePanicMove() ||
                    MakeEmptyBoardMove() ||
                    MakeReactionaryCaptureMove() ||
                    MakeReactionaryEvasiveMove())
                {
                    return;
                }
                AiLog("No filter triggered");
            }
            else if (aiMoveInputVO.isBot && MakeReactionaryQueenCaptureMove())
            {
                return;
            }

            // We will apply a bit of variance offset to the move index
            int windowDice = UnityEngine.Random.Range(ChessAiConfig.DIFFICULTY_VARIANCE * -1, ChessAiConfig.DIFFICULTY_VARIANCE + 1);

            // Find the move index
            // Since the indexes are inverted with the strongest first, we will invert the cpu strength
            float invertedStr = 1 - aiMoveInputVO.cpuStrengthPct;

            int index = Mathf.FloorToInt(scores.Count * invertedStr);

            // Now apply the variance dice
            int variedIndex = Mathf.Clamp(index + windowDice, 0, scores.Count - 1);

            DispatchMove(variedIndex);
        }

        /// <summary>
        /// We parse the move and dispatch the move
        /// </summary>
        private void DispatchMove(int index, bool panicMove = false)
        {
            AiLog(string.Format("index : {0} panic move : {1}", index, panicMove));
            index = Math.Min(aiSearchResultMovesList.Count - 1, index);
            string selectedMove = aiSearchResultMovesList[index];

            FileRank from = chessService.GetFileRankLocation(selectedMove[0], selectedMove[1]);
            FileRank to = chessService.GetFileRankLocation(selectedMove[2], selectedMove[3]);

            string promo = null;

            if (selectedMove.Length == 5)
            {
                promo = selectedMove[4].ToString();
            }

            // Off looking moves can still get to this point. After the initial reactionary filters
            // the computer selects a random unfiltered move. This could be a very "off" looking move.
            // So unless we have rolled the best move possible and we're not panicing at this time,
            // we filter out this off move by move up one index of the scores count.
            if (index > 0 && !panicMove && filterMoves)
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
            else if (index > 0 && !panicMove && aiMoveInputVO.isBot)
            {
                if (CancelMoveDueToQueenFeedOrWeakExchange(from, to, promo, index) ||
                    CancelMoveDueToRookRestriction(from, index) ||
                    CancelMoveDueToNonPromotion(promo))
                {
                    DispatchMove(index - 1);
                    return;
                }
            }

            // TODO: There is some case where a null expection occurs here. However the game continued seemingly fine.
            // Placing a check here may ignore a lingering request, but there is a chance that the crash may occur in another spot.
            // Further invenstigation is needed!
            if (lastDequeuedMethod != null && lastDequeuedMethod.promise != null)
            {
                lastDequeuedMethod.promise.Dispatch(from, to, promo);
                lastDequeuedMethod.promise = null;
                lastDequeuedMethod = null;
            }
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
                return true;
            }

            return false;
        }

        private bool CancelMoveDueToQueenFeedOrWeakExchange(FileRank from, FileRank to, string promo, int index)
        {
            var piece = chessService.GetPieceAtLocation(from);
            if (piece == null ||
                !piece.name.ToLower().Equals(ChessPieceName.BLACK_QUEEN))
            {
                return false;
            }

            return CancelMoveDueToFeedsOrWeakExchanges(from, to, promo, index);
        }

        private bool CancelMoveDueToFeedsOrWeakExchanges(FileRank from, FileRank to, string promo, int index)
        {
            if (chessService.WillMoveCauseWeakExchangeOrFeed(from, to, promo))
            {
                AiLog("detected weak exchange");
                return true;
            }
            AiLog("Did not detect weak exchange");
            return false;
        }

        private bool CancelMoveDueToWeakKingMove(FileRank from, int index)
        {
            if (IsMovingPiece(from, ChessPieceName.BLACK_KING))
            {
                return true;
            }

            return false;
        }

        private bool CancelMoveDueToRookRestriction(FileRank from, int index)
        {
            if (aiMoveInputVO.aiMoveNumber <= ChessAiConfig.ROOK_RESTRICTION_MOVE_COUNT &&
                IsMovingPiece(from, ChessPieceName.BLACK_ROOK))
            {
                return true;
            }

            return false;
        }

        public bool IsReactionaryCaptureAvailable()
        {
            var cheapestAttackingMove = chessService.GetCheapestAttackingMoveToSquare(aiMoveInputVO.lastPlayerMove.to);

            if (cheapestAttackingMove == null)
            {
                return false;
            }

            var landingDefended = chessService.IsSquareDefended(aiMoveInputVO.lastPlayerMove.to, aiMoveInputVO.playerColor);
            var attackingPieceValue = GetValueForPiece(cheapestAttackingMove.piece.name);
            var victimPieceValue = GetValueForPiece(aiMoveInputVO.lastPlayerMove.piece.name);
            var exchange = (attackingPieceValue < victimPieceValue) ||
                ((attackingPieceValue == victimPieceValue) && RollPercentageDice(ChessAiConfig.PIECE_EXCHANCE_CHANCE));

            return !landingDefended || exchange;
        }

        private bool MakeReactionaryQueenCaptureMove()
        {
            if (aiMoveInputVO.lastPlayerMove == null ||
                aiMoveInputVO.lastPlayerMove.piece == null ||
                !aiMoveInputVO.lastPlayerMove.piece.name.ToLower().Equals(ChessPieceName.BLACK_QUEEN))
                return false;

            return MakeReactionaryCaptureMove();
        }

        private bool MakeReactionaryCaptureMove()
        {
            if (aiMoveInputVO.lastPlayerMove == null)
            {
                return false;
            }

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
                    AiLog("Make reactionary move 1");
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
                    if (cheapestAttackingMove.piece == null)
                    {
                        throw new Exception("cheapestAttackingMove.piece is null");
                    }
                    
                    int attackingPieceValue = GetValueForPiece(cheapestAttackingMove.piece.name);
                    int victimPieceValue = GetValueForPiece(aiMoveInputVO.lastPlayerMove.piece.name);

                    // A chance of making the exchange if values are equal
                    bool exchange = false;
                    exchange = (attackingPieceValue < victimPieceValue) ||
                        ((attackingPieceValue == victimPieceValue) && RollPercentageDice(ChessAiConfig.PIECE_EXCHANCE_CHANCE));

                    if (exchange)
                    {
                        AiLog("Make reactionary move 2");
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
            if (aiMoveInputVO.lastPlayerMove == null)
            {
                return false;
            }

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
                        evade = true;
                    }
                }
                // Escape attack on undefended piece
                else
                {
                    evade = true;
                }

                if (evade)
                {
                    int aiEvasionMoveIndex = GetAiMoveIndexByMoveOrigin(enemyCaptureMove.to);

                    if (aiEvasionMoveIndex != -1)
                    {
                        DispatchMove(aiEvasionMoveIndex);
                    }
                    else
                    {
                        DispatchMove(0);
                    }
                    AiLog("MakeReactionaryEvasiveMove");
                    return true;
                }
            }

            return false;
        }

        private bool MakeOpeningMoves()
        {
            // First roll the overall chance of making a proper opening move
            bool rollOpening = UnityEngine.Random.Range(0.0f, 1.0f) < ChessAiConfig.OPENING_MOVE_PROPER_CHANCE;
            if (!rollOpening)
            {
                return false;
            }

            // Make the proper opening move
            if (aiMoveInputVO.aiMoveNumber <= ChessAiConfig.OPENING_MOVES_COUNT)
            {
                int selectCount = Math.Min(scores.Count, ChessAiConfig.OPENING_MOVES_SELECT_COUNT);
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
                AiLog("MakeOnlyMoveAvailable");
                DispatchMove(0);
                return true;
            }

            return false;
        }

        private bool MakePanicMove()
        {
            // CPU does not panic
            if (aiMoveInputVO.aiMoveDelay == AiMoveDelay.CPU)
            {
                return false;
            }

            // Apply panic attack and leave early. This means that we also
            // ignore our silly move filters below and make a move regardless
            // since the player is 'paniced'.
            bool panic = false;
            double clockSeconds = aiMoveInputVO.opponentTimer.TotalSeconds;

            // It applies to the rest of the time controls only at 10 seconds due 
            // to the mindset when the clock is so low.
            if (clockSeconds < 10 )
            {
                panic = RollPercentageDice(ChessAiConfig.TEN_SECOND_PANIC_CHANCE);
            }

            if (panic)
            {
                AiLog("Rolled a panic move.");
                DispatchMove(scores.Count - 1, true);
                return true;
            }

            return false;
        }

        private bool MakeEmptyBoardMove()
        {
            // If we have an empty board, we can't expose the Ai. So just
            // make the best move.
            if (ReachedEndGame())
            {
                AiLog("End game piece count detected.");
                DispatchMove(0);
                return true;
            }

            return false;
        }

        private bool ReachedEndGame()
        {
            return (chessService.GetPieceCount(ChessColor.BLACK) <= ChessAiConfig.END_GAME_PIECE_COUNT ||
                chessService.GetPieceCount(ChessColor.WHITE) <= ChessAiConfig.END_GAME_PIECE_COUNT);
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

            return 0;
        }

        private int GetValueForPiece(string pieceName)
        {
            pieceName = pieceName.ToLower();

            if (pieceName == ChessPieceName.BLACK_KING)
            {
                return ChessAiConfig.KING_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_QUEEN)
            {
                return ChessAiConfig.QUEEN_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_ROOK)
            {
                return ChessAiConfig.ROOK_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_BISHOP)
            {
                return ChessAiConfig.BISHOP_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_KNIGHT)
            {
                return ChessAiConfig.KNIGHT_VALUE;
            }
            else if (pieceName == ChessPieceName.BLACK_PAWN)
            {
                return ChessAiConfig.PAWN_VALUE;
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
