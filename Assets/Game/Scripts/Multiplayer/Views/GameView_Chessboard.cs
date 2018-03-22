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

namespace TurboLabz.MPChess
{
    public partial class GameView
    {
        // TODO: TEMP AUDIO CODE
        // An audio component is added to the GameView object. Remove the 
        // component when removing this temp code.
        private AudioSource audioSource;
        public AudioClip placePieceSoundFx;
        public AudioClip promoSoundFx;
        public AudioClip victoryFx;
        public AudioClip defeatFx;
        public AudioClip captureFx;
        public AudioClip checkFx;
        private const float FX_VOLUME = 1f;

        private const string IMAGE_POSSIBLE_MOVE = "PossibleMove";
        private const string IMAGE_ATTACK_INDICATOR = "Attack";
        private const string IMAGE_FROM = "From";
        private const string IMAGE_TO = "To";
        private const string IMAGE_SQUARE_WHITE = "SquareWhite";
        private const string IMAGE_SQUARE_BLACK = "SquareBlack";
        private const float PIECE_ANIMATION_TIME = 0.15f;
        private readonly Quaternion WHITE_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -270f));
        private readonly Vector3 WHITE_BOARD_POSITION =  new Vector3(420f, -420f, 10f);
        private readonly Quaternion BLACK_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        private readonly Vector3 BLACK_BOARD_POSITION = new Vector3(-420f, 420f, 10f);

        public Signal<FileRank> squareClickedSignal = new Signal<FileRank>();
        public Signal opponentMoveRenderComplete = new Signal();
        public GameObject chessContainer;
        public Transform chessboard;
        public Transform[] chessboardSquares;
        public GameObject[] pieces;
        public GameObject[] possibleMoveIndicators;
        public GameObject[] attackIndicators;
        public GameObject whiteKingCheckIndicator;
        public GameObject blackKingCheckIndicator;
        public GameObject playerFromWhiteIndicator;
        public GameObject playerFromBlackIndicator;
        public GameObject playerToIndicator;
        public GameObject opponentFromIndicator;
        public GameObject opponentToIndicator;
        public GameObject fileRankLabelsForward;
        public GameObject fileRankLabelsBackward;

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

        public void ApplyChessboardSkin()
        {
            SkinContainer container = SkinContainer.container;
            if (container == null)
            {
                return;
            }

            Color tempColor = Color.white;
            tempColor.a = 0f;
            foreach (Transform squareT in chessboardSquares)
            {
                squareT.GetComponent<SpriteRenderer>().color = tempColor;
            }

            // Apply skin to the background (includes chess board)
            Sprite bgSprite = container.GetSprite(SkinSpriteKey.BACKGROUND);
            // TODO: replace with reference
            GameObject bgObj = GameObject.Find("Background");

            if (bgObj != null)
            {
                bgObj.GetComponent<SpriteRenderer>().sprite = bgSprite;
            }

            // Apply skin to the chess pieces
            foreach(GameObject piece in pieces)
            {
                Sprite pieceSprite = container.GetSprite(piece.name);
                if (pieceSprite != null)
                {
                    piece.GetComponent<SpriteRenderer>().sprite = pieceSprite;
                }
            }

            // Apply skin to the captured chess pieces
            foreach (GameObject capturePiece in capturedPieces)
            {
                Sprite pieceSprite = container.GetSprite("c" + capturePiece.name);
                if (pieceSprite != null)
                {
                    capturePiece.GetComponent<UnityEngine.UI.Image>().sprite = pieceSprite;
                }
            }
        }

        public void InitChessboard()
        {
            // TODO: TEMP AUDIO CODE
            audioSource = GetComponent<AudioSource>();
           

            // Add poolable images to our object pool
            foreach(GameObject piece in pieces)
            {
                pool.AddObject(piece);
            }

            foreach(GameObject possibleMoveIndicator in possibleMoveIndicators)
            {
                pool.AddObject(possibleMoveIndicator);
            }

            foreach(GameObject attackIndicator in attackIndicators)
            {
                pool.AddObject(attackIndicator);
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
                pieceImage.transform.position = chessboardSquares[squareIndex].position;
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
            pieceTransform.position = chessboardSquares[toSquareIndex].position;

            // Hide the check indicators
            blackKingCheckIndicator.SetActive(false);
            whiteKingCheckIndicator.SetActive(false);

            // Handle remaining visual updates
            UpdatePiecesPostMove(moveVO, true);

            HandleCastling(moveVO);

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
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
                audioSource.PlayOneShot(captureFx, FX_VOLUME);
            }
            else
            {
                // TODO: TEMP AUDIO CODE
                audioSource.PlayOneShot(placePieceSoundFx, FX_VOLUME);
            }

            // Update the from piece position to the target position
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            pieceTransform.position = chessboardSquares[toSquareIndex].position;

            // Update chessboard piece image array
            chessboardPieces[toSquareIndex] = chessboardPieces[fromSquareIndex];
            chessboardPieces[fromSquareIndex] = null;

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
        }

        public void UpdateOpponentMove(MoveVO moveVO)
        {
            FileRank fromFileRank = moveVO.fromSquare.fileRank;
            FileRank toFileRank = moveVO.toSquare.fileRank;

            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];
            ShowOpponentFromIndicator(moveVO.fromSquare);

            // Hide the check indicators
            blackKingCheckIndicator.SetActive(false);
            whiteKingCheckIndicator.SetActive(false);

            // Animate the piece movement and update the piece image array upon completion
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            opponentAnimationInProgress = true;
            pieceTransform.DOMove(chessboardSquares[toSquareIndex].position, PIECE_ANIMATION_TIME)
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
                rookTransform.DOMove(chessboardSquares[rookToSquareIndex].position, PIECE_ANIMATION_TIME).SetEase(Ease.Linear);

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
                audioSource.PlayOneShot(captureFx, FX_VOLUME);
            }
            else
            {
                // TODO: TEMP AUDIO CODE
                audioSource.PlayOneShot(placePieceSoundFx, FX_VOLUME);
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
                UpdatePromo(vo);
            }

            // Update additional info like score, notation etc
            UpdateInfoPostMove(vo, isPlayerTurn);
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
            UpdatePromo(vo);

            // Update additional info like score, notation etc
            UpdateInfoPostMove(vo, true);
        }

        public void UpdateMoveForResume(MoveVO vo, bool isPlayerTurn)
        {
            ChessSquare capturedSquare = vo.capturedSquare;

            if (capturedSquare != null)
            {
                HandleCapturePieceGraphic(capturedSquare.piece.name, isPlayerTurn);
            }

            UpdateInfoPostMove(vo, isPlayerTurn);

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
            HidePossibleMoves();

            foreach (ChessSquare square in possibleMoves)
            {   
                int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];

                GameObject possibleMoveIndicator = pool.GetObject(IMAGE_POSSIBLE_MOVE);
                possibleMoveIndicator.transform.position = chessboardSquares[squareIndex].position;
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

            OnPieceDeselected();
        }

        public void ShowPlayerFromIndicator(ChessSquare square)
        {
            HidePlayerFromIndicator();

            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];

            Transform targetSquare = chessboardSquares[squareIndex];
            string squareSpriteName = targetSquare.GetComponent<SpriteRenderer>().sprite.name;
            GameObject indicator = (squareSpriteName == IMAGE_SQUARE_WHITE) ? playerFromWhiteIndicator : playerFromBlackIndicator;
            indicator.transform.position = targetSquare.position;
            indicator.SetActive(true);
        }

        public void ShowPlayerToIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            playerToIndicator.transform.position = chessboardSquares[squareIndex].position;
            playerToIndicator.SetActive(true);
        }

        public void HidePlayerFromIndicator()
        {
            playerFromWhiteIndicator.SetActive(false);
            playerFromBlackIndicator.SetActive(false);
        }

        public void HidePlayerToIndicator()
        {
            playerToIndicator.SetActive(false);
        }

        public void ShowOpponentFromIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            opponentFromIndicator.transform.position = chessboardSquares[squareIndex].position;
            opponentFromIndicator.SetActive(true);
        }

        public void ShowOpponentToIndicator(ChessSquare square)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            opponentToIndicator.transform.position = chessboardSquares[squareIndex].position;
            opponentToIndicator.SetActive(true);
        }

        public void HideOpponentFromIndicator()
        {
            opponentFromIndicator.SetActive(false);
        }

        public void HideOpponentToIndicator()
        {
            opponentToIndicator.SetActive(false);
        }

        public void SetupChessboard(bool isPlayerWhite)
        {
            if (isPlayerWhite)
            {
                chessboard.rotation = WHITE_BOARD_ROTATION;
                chessboard.position = WHITE_BOARD_POSITION;
                playerColor = ChessColor.WHITE;
                opponentColor = ChessColor.BLACK;
            }
            else
            { 
                chessboard.rotation = BLACK_BOARD_ROTATION;
                chessboard.position = BLACK_BOARD_POSITION;
                playerColor = ChessColor.BLACK;
                opponentColor = ChessColor.WHITE;
            }

            fileRankLabelsForward.SetActive(isPlayerWhite);
            fileRankLabelsBackward.SetActive(!isPlayerWhite);
        }

        private void UpdateInfoPostMove(MoveVO vo, bool isPlayerTurn)
        {
            // Highlight the player's king if its gone into check
            UpdateKingCheckIndicator(vo);

            // Update the centipawn scores
            UpdateScores(vo);

            // Update the notation
            UpdateNotation(vo);
        }

        private void UpdateKingCheckIndicator(MoveVO vo)
        {
            blackKingCheckIndicator.SetActive(false);
            whiteKingCheckIndicator.SetActive(false);

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
                checkIndicator = blackKingCheckIndicator;
            }
            else
            {
                kingSquareIndex = Array.IndexOf(chessboardPieces, whiteKing);
                checkIndicator = whiteKingCheckIndicator;
            }

            checkIndicator.SetActive(true);
            checkIndicator.transform.position = chessboardSquares[kingSquareIndex].position;

            if (vo.isPlayerInCheck)
            {
                audioSource.PlayOneShot(checkFx, FX_VOLUME);
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

        private void OnShowChessboard()
        {
            chessContainer.SetActive(true);
            playerFromWhiteIndicator.SetActive(false);
            playerFromBlackIndicator.SetActive(false);
            playerToIndicator.SetActive(false);
            opponentFromIndicator.SetActive(false);
            opponentToIndicator.SetActive(false);
            whiteKingCheckIndicator.SetActive(false);
            blackKingCheckIndicator.SetActive(false);

            InitClickAndDrag();
            HidePossibleMoves();
        }

        private void OnHideChessboard()
        {
            chessContainer.SetActive(false);
        }
    }
}
