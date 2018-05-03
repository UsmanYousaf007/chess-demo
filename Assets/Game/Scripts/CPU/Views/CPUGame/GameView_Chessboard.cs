/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:36:57 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using DG.Tweening;
using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using UnityEngine.UI;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    public partial class GameView
    {
        public ChessboardRefs refs;
        public Signal<FileRank> squareClickedSignal = new Signal<FileRank>();
        public Signal opponentMoveRenderComplete = new Signal();

        private const float PIECE_ANIMATION_TIME = 0.15f;

        private ObjectPool pool = new ObjectPool();
        private List<GameObject> activatedPossibleMoveIndicators = new List<GameObject>();
        private List<GameObject> activatedAttackIndicators = new List<GameObject>();
        private List<GameObject> activatedPieceImages = new List<GameObject>();
        private GameObject[] chessboardPieces = new GameObject[64];
        private GameObject blackKing;
        private GameObject whiteKing;
        private ChessColor playerColor;
        private ChessColor opponentColor;
        private bool opponentAnimationInProgress = false;
        private IEnumerator showPossibleMovesCR = null;

        public void InitChessboard()
        {
            // Add poolable images to our object pool
            foreach(GameObject piece in refs.pieces)
            {
                pool.AddObject(piece);
            }

            foreach(GameObject possibleMoveIndicator in refs.possibleMoveIndicators)
            {
                pool.AddObject(possibleMoveIndicator);
            }

            // Add listeners to our squares
            AddSquareListeners();
        }

        public void UpdateChessboard(ChessSquare[,] chessSquares)
        {
            // Reset the piece image pool
            foreach (GameObject obj in activatedPieceImages)
            {
                pool.ReturnObject(obj);
            }

            activatedPieceImages.Clear();

            // Clear the piece layout array (chessboardPieces by position)
            for (int i = 0; i < 64; i++)
            {
                chessboardPieces[i] = null;
            }

            // Now add the new piece positions to the image pool
            // as well as the piece layout array (chessboardPieces)
            foreach(ChessSquare square in chessSquares)
            {
                if (square.piece == null)
                {
                    continue;
                }

                GameObject pieceImage = pool.GetObject(square.piece.name);
                activatedPieceImages.Add(pieceImage);

                int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
                pieceImage.transform.position = refs.chessboardSquares[squareIndex].position;
                chessboardPieces[squareIndex] = pieceImage;

                if (pieceImage.name == "k")
                {
                    blackKing = pieceImage;
                }
                else if (pieceImage.name == "K")
                {
                    whiteKing = pieceImage;
                }
            }
        }

        public void UpdatePlayerMove(MoveVO moveVO)
        {
            // Read the file+rank for the move from the vo object
            FileRank fromFileRank = moveVO.fromSquare.fileRank;
            FileRank toFileRank = moveVO.toSquare.fileRank;

            // Get the chessboard piece index from the file+rank
            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];

            // Update the from piece position to the target position
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            pieceTransform.position = refs.chessboardSquares[toSquareIndex].position;

            // Hide the check indicators
            refs.blackKingCheckIndicator.SetActive(false);
            refs.whiteKingCheckIndicator.SetActive(false);

            // Handle remaining visual updates
            UpdatePiecesPostMove(moveVO, true);

            HandleCastling(moveVO);

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
            HideHint();
        }

        public void UpdatePlayerPrePromoMove(MoveVO moveVO)
        {
            // Read the file+rank for the move from the vo object
            FileRank fromFileRank = moveVO.fromSquare.fileRank;
            FileRank toFileRank = moveVO.toSquare.fileRank;

            // Get the chessboard piece index from the file+rank
            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];

            // If there is a piece in the target location, destroy it
            GameObject capturedPieceImage = chessboardPieces[toSquareIndex];

            if (capturedPieceImage != null)
            {
                // Destroy the captured piece image
                activatedPieceImages.Remove(capturedPieceImage);
                pool.ReturnObject(capturedPieceImage);
                chessboardPieces[toSquareIndex] = null;
                audioService.Play(audioService.sounds.SFX_CAPTURE);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_PLACE_PIECE);
            }

            // Update the from piece position to the target position
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            pieceTransform.position = refs.chessboardSquares[toSquareIndex].position;

            // Update chessboard piece image array
            chessboardPieces[toSquareIndex] = chessboardPieces[fromSquareIndex];
            chessboardPieces[fromSquareIndex] = null;

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
            HideHint();
        }

        public void UpdateOpponentMove(MoveVO moveVO)
        {
            FileRank fromFileRank = moveVO.fromSquare.fileRank;
            FileRank toFileRank = moveVO.toSquare.fileRank;

            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];
            ShowOpponentFromIndicator(moveVO.fromSquare);

            // Hide the check indicators
            refs.blackKingCheckIndicator.SetActive(false);
            refs.whiteKingCheckIndicator.SetActive(false);

            // Animate the piece movement and update the piece image array upon completion
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            opponentAnimationInProgress = true;
            pieceTransform.DOMove(refs.chessboardSquares[toSquareIndex].position, PIECE_ANIMATION_TIME)
                .SetEase(Ease.Linear)
                .OnComplete(()=>OnOpponentMoveCompleted(moveVO));

            HandleCastling(moveVO);
        }

        private void OnOpponentMoveCompleted(MoveVO moveVO)
        {
            UpdatePiecesPostMove(moveVO, false);
            ShowOpponentToIndicator(moveVO.toSquare);
            opponentAnimationInProgress = false;
            opponentMoveRenderComplete.Dispatch();
        }

        private void HandleCastling(MoveVO moveVO)
        {
            if (moveVO.moveFlag == ChessMoveFlag.CASTLE_KING_SIDE || moveVO.moveFlag == ChessMoveFlag.CASTLE_QUEEN_SIDE)
            {
                FileRank rookFromFileRankVO;
                rookFromFileRankVO.file = (moveVO.moveFlag == ChessMoveFlag.CASTLE_KING_SIDE) ? 7 : 0;
                rookFromFileRankVO.rank = moveVO.fromSquare.fileRank.rank;

                FileRank rookToFileRankVO;
                rookToFileRankVO.file = (moveVO.moveFlag == ChessMoveFlag.CASTLE_KING_SIDE) ? 5 : 3;
                rookToFileRankVO.rank = moveVO.toSquare.fileRank.rank;

                int rookFromSquareIndex = RankFileMap.Map[rookFromFileRankVO.rank, rookFromFileRankVO.file];
                int rookToSquareIndex = RankFileMap.Map[rookToFileRankVO.rank, rookToFileRankVO.file];

                Transform rookTransform = chessboardPieces[rookFromSquareIndex].transform;
                rookTransform.DOMove(refs.chessboardSquares[rookToSquareIndex].position, PIECE_ANIMATION_TIME).SetEase(Ease.Linear);

                // Update chessboard piece image array
                chessboardPieces[rookToSquareIndex] = chessboardPieces[rookFromSquareIndex];
                chessboardPieces[rookFromSquareIndex] = null;
            }
        }

        private void UpdatePiecesPostMove(MoveVO vo, bool isPlayerTurn)
        {
            // Get the file+rank from the move vo
            FileRank fromFileRank = vo.fromSquare.fileRank;
            FileRank toFileRank = vo.toSquare.fileRank;

            // Get the chessboard piece indexes from the file+rank
            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];

            // Handle captured pieces
            ChessSquare capturedSquare = vo.capturedSquare;

            if (capturedSquare != null)
            {
                // Get the chessboard piece index for the captured square
                int capturedSquareIndex = RankFileMap.Map[capturedSquare.fileRank.rank, capturedSquare.fileRank.file];

                // Destroy the captured piece image
                GameObject capturedPieceImage = chessboardPieces[capturedSquareIndex];
                activatedPieceImages.Remove(capturedPieceImage);
                pool.ReturnObject(capturedPieceImage);
                chessboardPieces[capturedSquareIndex] = null;

                // Show the captured piece in the captured pieces slots
                HandleCapturePieceGraphic(capturedPieceImage.name, isPlayerTurn);
                audioService.Play(audioService.sounds.SFX_CAPTURE);
            }
            else
            {
                audioService.Play(audioService.sounds.SFX_PLACE_PIECE);
            }

            // Update chessboard piece image array
            chessboardPieces[toSquareIndex] = chessboardPieces[fromSquareIndex];
            chessboardPieces[fromSquareIndex] = null;

            // Handle opponent promotions if required
            if (vo.moveFlag == ChessMoveFlag.PAWN_PROMOTION_QUEEN ||
                vo.moveFlag == ChessMoveFlag.PAWN_PROMOTION_ROOK ||
                vo.moveFlag == ChessMoveFlag.PAWN_PROMOTION_BISHOP ||
                vo.moveFlag == ChessMoveFlag.PAWN_PROMOTION_KNIGHT)
            {
                UpdatePromoDialog(vo);
            }

            // Update additional info like score, notation etc
            UpdateInfoPostMove(vo);
        }

        public void UpdatePiecesPostPromo(MoveVO vo)
        {
            // Handle captured pieces
            ChessSquare capturedSquare = vo.capturedSquare;

            if (capturedSquare != null)
            {
                // Show the captured piece in the captured pieces slots
                HandleCapturePieceGraphic(capturedSquare.piece.name, true);
            }
                
            // Destroy the pawn and put the new image in place
            UpdatePromoDialog(vo);

            // Update additional info like score, notation etc
            UpdateInfoPostMove(vo);
        }

        public void UpdateMoveForResume(MoveVO vo, bool isPlayerTurn)
        {
            ChessSquare capturedSquare = vo.capturedSquare;

            if (capturedSquare != null)
            {
                HandleCapturePieceGraphic(capturedSquare.piece.name, isPlayerTurn);
            }

            UpdateInfoPostMove(vo, true);

            if (isPlayerTurn)
            {
                HideOpponentFromIndicator();
                HideOpponentToIndicator();
            }
            else
            {
                ShowOpponentFromIndicator(vo.fromSquare);
                ShowOpponentToIndicator(vo.toSquare);
            }
        }

        public void ShowPossibleMoves(FileRank pieceLocation, List<ChessSquare> possibleMoves)
        {
            if (showPossibleMovesCR != null)
            {
                StopCoroutine(showPossibleMovesCR);
                showPossibleMovesCR = null;
            }

            showPossibleMovesCR = ShowPossibleMovesCR(pieceLocation, possibleMoves);
            StartCoroutine(showPossibleMovesCR);
        }

        private IEnumerator ShowPossibleMovesCR(FileRank pieceLocation, List<ChessSquare> possibleMoves)
        {
            while(opponentAnimationInProgress)
            {
                yield return null;
            }

            // Hide the old ones before showing new ones
            HidePossibleMovesExec();

            foreach (ChessSquare square in possibleMoves)
            {   
                int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];

                GameObject possibleMoveIndicator = pool.GetObject(ChessboardRefs.IMAGE_POSSIBLE_MOVE);
                possibleMoveIndicator.transform.position = refs.chessboardSquares[squareIndex].position;
                activatedPossibleMoveIndicators.Add(possibleMoveIndicator);

                // Attack indicators are distracting, switching them off.
                /*
                GameObject piece = chessboardPieces[squareIndex];

                if (piece != null)
                {
                    GameObject attackIndicator = pool.GetObject(IMAGE_ATTACK_INDICATOR);
                    attackIndicator.transform.position = chessboardSquares[squareIndex].position;
                    activatedAttackIndicators.Add(attackIndicator);
                }
                */
            }

            OnPieceSelected(pieceLocation, possibleMoves);
        }

        public void HidePossibleMoves()
        {
            HidePossibleMovesExec();
            HandleEvent(CDSEvent.PLAYER_TURN_PIECE_DESELECTED);
        }

        private void HidePossibleMovesExec()
        {
            foreach (GameObject obj in activatedPossibleMoveIndicators)
            {
                pool.ReturnObject(obj);
            }

            activatedPossibleMoveIndicators.Clear();

            foreach (GameObject obj in activatedAttackIndicators)
            {
                pool.ReturnObject(obj);
            }

            activatedAttackIndicators.Clear();

            // Also cancel any "pending" show possible move calls that
            // have been coroutined due to opponent animation
            if (showPossibleMovesCR != null)
            {
                StopCoroutine(showPossibleMovesCR);
                showPossibleMovesCR = null;
            }
        }

        public void ShowPlayerFromIndicator(ChessSquare square)
        {
            refs.playerFromIndicator.SetActive(true);
        }

        public void ShowPlayerToIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            refs.playerToIndicator.transform.position = refs.chessboardSquares[squareIndex].position;
            refs.playerToIndicator.SetActive(true);
        }

        public void HidePlayerFromIndicator()
        {
            refs.playerFromIndicator.SetActive(false);
        }

        public void HidePlayerToIndicator()
        {
            refs.playerToIndicator.SetActive(false);
        }

        public void ShowOpponentFromIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            refs.opponentFromIndicator.transform.position = refs.chessboardSquares[squareIndex].position;
            refs.opponentFromIndicator.SetActive(true);
        }

        public void ShowOpponentToIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            refs.opponentToIndicator.transform.position = refs.chessboardSquares[squareIndex].position;
            refs.opponentToIndicator.SetActive(true);
        }

        public void HideOpponentFromIndicator()
        {
            refs.opponentFromIndicator.SetActive(false);
        }

        public void HideOpponentToIndicator()
        {
            refs.opponentToIndicator.SetActive(false);
        }

        public void SetupChessboard(bool isPlayerWhite)
        {
            if (isPlayerWhite)
            {
                refs.chessboard.rotation = refs.WHITE_BOARD_ROTATION;
                refs.chessboard.position = refs.WHITE_BOARD_POSITION;
                playerColor = ChessColor.WHITE;
                opponentColor = ChessColor.BLACK;

            }
            else
            { 
                refs.chessboard.rotation = refs.BLACK_BOARD_ROTATION;
                refs.chessboard.position = refs.BLACK_BOARD_POSITION;
                playerColor = ChessColor.BLACK;
                opponentColor = ChessColor.WHITE;
            }

            refs.fileRankLabelsForward.SetActive(isPlayerWhite);
            refs.fileRankLabelsBackward.SetActive(!isPlayerWhite);

            refs.chessContainer.SetActive(true);
            refs.playerFromIndicator.SetActive(false);
            refs.playerToIndicator.SetActive(false);
            refs.opponentFromIndicator.SetActive(false);
            refs.opponentToIndicator.SetActive(false);
            refs.whiteKingCheckIndicator.SetActive(false);
            refs.blackKingCheckIndicator.SetActive(false);

            InitClickAndDrag();
            HidePossibleMoves();
        }

        private void UpdateInfoPostMove(MoveVO vo, bool isResume = false)
        {
            // Highlight the player's king if its gone into check
            UpdateKingCheckIndicator(vo, isResume);

            // Update the centipawn scores
            UpdateScores(vo);

            // Update the notation
            UpdateNotation(vo);
        }

        private void UpdateKingCheckIndicator(MoveVO vo, bool isResume)
        {
            refs.blackKingCheckIndicator.SetActive(false);
            refs.whiteKingCheckIndicator.SetActive(false);

            ChessColor checkedColor;

            if (vo.isPlayerInCheck)
            {
                checkedColor = playerColor;
            }
            else if (vo.isOpponentInCheck)
            {
                checkedColor = opponentColor;
            }
            else
            {
                return;
            }

            int kingSquareIndex;
            GameObject checkIndicator;

            if (checkedColor == ChessColor.BLACK)
            {
                kingSquareIndex = Array.IndexOf(chessboardPieces, blackKing);
                checkIndicator = refs.blackKingCheckIndicator;
            }
            else
            {
                kingSquareIndex = Array.IndexOf(chessboardPieces, whiteKing);
                checkIndicator = refs.whiteKingCheckIndicator;
            }

            checkIndicator.SetActive(true);
            checkIndicator.transform.position = refs.chessboardSquares[kingSquareIndex].position;

            if (vo.isPlayerInCheck && !isResume)
            {
                audioService.Play(audioService.sounds.SFX_CHECK);
            }
        }

        private void TintSpriteRed(GameObject sprite)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.red;
        }

        private void TintSpriteGray(GameObject sprite)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.gray;
        }

        private void OnParentHideChessboard()
        {
            refs.chessContainer.SetActive(false);
        }
    }
}
