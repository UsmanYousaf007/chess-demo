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

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Board Scaling")]
        public Transform boardContent;
        public GameObject bottomBarContent;

        [Header("Chessboard")]
        public GameObject[] pieces;
        public GameObject[] possibleMoveIndicators;
        public Transform[] chessboardSquares;
        public TapOff tapOff;
        public GameObject chessContainer;
        public Transform chessboard;
        public Transform playerProfileUiAnchor;
        public Transform opponentProfileUiAnchor;
        public Transform analysisPanelEndPivot;

        public GameObject playerFromIndicator;
        public GameObject playerToIndicator;
        public GameObject opponentFromIndicator;
        public GameObject opponentToIndicator;
        public GameObject fileRankLabelsForward;
        public GameObject fileRankLabelsBackward;
        public GameObject kingCheckIndicator;
        public Transform coachUIAnchorPoint;

        public Signal<FileRank> squareClickedSignal = new Signal<FileRank>();
        public Signal opponentMoveRenderComplete = new Signal();

        private const string IMAGE_POSSIBLE_MOVE = "PossibleMove";
        private readonly Quaternion WHITE_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -270f));
        private readonly Vector3 WHITE_BOARD_POSITION =  new Vector3(420f, -420f, 10f);
        private readonly Quaternion BLACK_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        private readonly Vector3 BLACK_BOARD_POSITION = new Vector3(-420f, 420f, 10f);
        private const float PIECE_ANIMATION_TIME = 0.2f;

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
        private float scaleUniform;

        public void InitChessboard()
        {
            // Add poolable images to our object pool
            foreach(GameObject piece in pieces)
            {
                pool.AddObject(piece);
            }

            foreach(GameObject possibleMoveIndicator in possibleMoveIndicators)
            {
                pool.AddObject(possibleMoveIndicator);
            }

            // Add listeners to our squares
            AddSquareListeners();

            // Add the tap off listener
            tapOff.tapOffSignal.AddListener(TapOff);

            StretchBoard();
        }

        private void TapOff()
        {
            FileRank offBoard;
            offBoard.file = -1;
            offBoard.rank = -1;
            squareClickedSignal.Dispatch(offBoard);
        }

        private void StretchBoard()
        {
            const int BOARD_WIDTH = 120 * 8;        // Square width x 8 squares
            const float BOARD_STRETCH_CAP = 1.3f;   // Maximum stretch scale 

            // Stretch board according to screen width
            scaleUniform = Screen.width / (BOARD_WIDTH * canvas.transform.localScale.x);
            float scaleUniformOriginal = scaleUniform;
            scaleUniform = (scaleUniform > BOARD_STRETCH_CAP) ? BOARD_STRETCH_CAP : scaleUniform; // Apply cap
            boardContent.localScale = Vector3.Scale(new Vector3(scaleUniform, scaleUniform, scaleUniform), boardContent.localScale);
            float scaleWidth = 1.0f - (scaleUniformOriginal - scaleUniform) / 1.5f;

            // Getting reference to the main camera
            Camera mainCamera = Camera.main;

            // Adjust screen content according to board stretch
            Rect strechMax = ((RectTransform)gameObject.transform).rect;
            float h = ((RectTransform)playerInfoPanel.transform).sizeDelta.y;
            float offsetY = playerInfoPanel.transform.position.y * (scaleUniform - 1.0f);
            ((RectTransform)playerInfoPanel.transform).sizeDelta = new Vector2(strechMax.width * scaleWidth, h);
            var playerProfileScreenPoint = mainCamera.WorldToScreenPoint(playerProfileUiAnchor.position);
            playerInfoPanel.transform.position = playerProfileScreenPoint;
            //playerInfoPanel.transform.position = new Vector3(playerInfoPanel.transform.position.x, (playerInfoPanel.transform.position.y - offsetY) - scaleWidth, playerInfoPanel.transform.position.z);

            h = ((RectTransform)opponentInfoPanel.transform).sizeDelta.y;
            ((RectTransform)opponentInfoPanel.transform).sizeDelta = new Vector2(strechMax.width * scaleWidth, h);
            //opponentInfoPanel.transform.position = new Vector3(opponentInfoPanel.transform.position.x, (opponentInfoPanel.transform.position.y + offsetY) + scaleWidth, opponentInfoPanel.transform.position.z);
            var opponentProfileScreenPoint = mainCamera.WorldToScreenPoint(opponentProfileUiAnchor.position);
            opponentInfoPanel.transform.position = opponentProfileScreenPoint;

            //((RectTransform)coachView.bg.transform).sizeDelta = new Vector2((strechMax.width * scaleWidth) + (20 * scaleWidth), ((RectTransform)coachView.bg.transform).sizeDelta.y);
            //var viewportPoint = mainCamera.WorldToScreenPoint(coachUIAnchorPoint.position);
            //coachView.bg.transform.position = viewportPoint;

            float bottomBarH = ((RectTransform)bottomBarContent.transform).sizeDelta.y;
            ((RectTransform)bottomBarContent.transform).sizeDelta = new Vector2(strechMax.width * scaleWidth, bottomBarH);

            if (!NotchHandler.HasNotch())
            {
                h = Mathf.Abs(playerInfoPanel.transform.localPosition.y - analysisPanelEndPivot.localPosition.y);
                ((RectTransform)analysisPanel.transform).sizeDelta = new Vector2(strechMax.width * scaleWidth, h);
                analysisPanel.transform.position = playerProfileScreenPoint;
            }
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
            kingCheckIndicator.SetActive(false);

            // Handle remaining visual updates
            UpdatePiecesPostMove(moveVO, true);

            HandleCastling(moveVO);

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
            HideHint();
            HideHindsight();

            showAdOnBack = true;
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
            pieceTransform.position = chessboardSquares[toSquareIndex].position;

            // Update chessboard piece image array
            chessboardPieces[toSquareIndex] = chessboardPieces[fromSquareIndex];
            chessboardPieces[fromSquareIndex] = null;

            // Update indicators
            ShowPlayerFromIndicator(moveVO.fromSquare);
            ShowPlayerToIndicator(moveVO.toSquare);
            HideOpponentFromIndicator();
            HideOpponentToIndicator();
            HideHint();
            HideHindsight();

            showAdOnBack = true;
        }

        public void UpdateOpponentMove(MoveVO moveVO)
        {
            FileRank fromFileRank = moveVO.fromSquare.fileRank;
            FileRank toFileRank = moveVO.toSquare.fileRank;

            int fromSquareIndex = RankFileMap.Map[fromFileRank.rank, fromFileRank.file];
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];
            ShowOpponentFromIndicator(moveVO.fromSquare);

            // Hide the check indicators
            kingCheckIndicator.SetActive(false);

            // Animate the piece movement and update the piece image array upon completion
            Transform pieceTransform = chessboardPieces[fromSquareIndex].transform;
            opponentAnimationInProgress = true;
            pieceTransform.DOMove(chessboardSquares[toSquareIndex].position, PIECE_ANIMATION_TIME)
                .SetEase(Ease.InSine)
                .OnComplete(()=>OnOpponentMoveCompleted(moveVO));

            HandleCastling(moveVO);
        }

        private void OnOpponentMoveCompleted(MoveVO moveVO)
        {
            UpdatePiecesPostMove(moveVO, false);
            ShowOpponentToIndicator(moveVO.toSquare);
            opponentAnimationInProgress = false;
            opponentMoveRenderComplete.Dispatch();
            ShowSpecialHintBubble(moveVO.opponentScore);
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
                rookTransform.DOMove(chessboardSquares[rookToSquareIndex].position, PIECE_ANIMATION_TIME).SetEase(Ease.InSine);

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
                ResetCapturedIndicators(isPlayerTurn);
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
            else
            {
                ResetCapturedIndicators(true);
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
            else
            {
                ResetCapturedIndicators(isPlayerTurn);
            }

            UpdateInfoPostMove(vo, true);

            if (isPlayerTurn)
            {
                HideOpponentFromIndicator();
                HideOpponentToIndicator();
                ShowPlayerFromIndicator(vo.fromSquare);
                ShowPlayerToIndicator(vo.toSquare);
            }
            else
            {
                HidePlayerFromIndicator();
                HidePlayerToIndicator();
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
            if (this.gameObject.activeInHierarchy)
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

        public void ShowIndicator(ChessSquare square, GameObject indicator)
        {
            int squareIndex = RankFileMap.Map[square.fileRank.rank, square.fileRank.file];
            indicator.transform.position = chessboardSquares[squareIndex].position;
            indicator.SetActive(true);
        }

        public void ShowPlayerFromIndicator(ChessSquare square)
        {
            ShowIndicator(square, playerFromIndicator);
        }

        public void ShowPlayerToIndicator(ChessSquare square)
        {
            ShowIndicator(square, playerToIndicator);
        }

        public void HidePlayerFromIndicator()
        {
            playerFromIndicator.SetActive(false);
        }

        public void HidePlayerToIndicator()
        {
            playerToIndicator.SetActive(false);
        }

        public bool ArePlayerMoveIndicatorsVisible()
        {
            return playerToIndicator.activeSelf && playerFromIndicator.activeSelf;
        }

        public void ShowOpponentFromIndicator(ChessSquare square)
        {
            ShowIndicator(square, opponentFromIndicator);
        }

        public void ShowOpponentToIndicator(ChessSquare square)
        {
            ShowIndicator(square, opponentToIndicator);
        }

        public void HideOpponentFromIndicator()
        {
            opponentFromIndicator.SetActive(false);
        }

        public void HideOpponentToIndicator()
        {
            opponentToIndicator.SetActive(false);
        }

        public void SetupChessboard(SetupChessboardVO vo)
        {
            isLongPlay = vo.isLongPlay;
            isRankedGame = vo.isRanked;
            gameTimeMode = vo.gameTimeMode;
            challengeId = vo.challengeId;
            //isTenMinGame = vo.isTenMinGame;
            //isOneMinGame = vo.isOneMinGame;
            //isThirtyMinGame = vo.isThirtyMinGame;

            if (vo.isPlayerWhite)
            {
                chessboard.localRotation = WHITE_BOARD_ROTATION;
                chessboard.localPosition = WHITE_BOARD_POSITION;
                playerColor = ChessColor.WHITE;
                opponentColor = ChessColor.BLACK;

            }
            else
            { 
                chessboard.localRotation = BLACK_BOARD_ROTATION;
                chessboard.localPosition = BLACK_BOARD_POSITION;
                playerColor = ChessColor.BLACK;
                opponentColor = ChessColor.WHITE;
            }

            fileRankLabelsForward.SetActive(vo.isPlayerWhite);
            fileRankLabelsBackward.SetActive(!vo.isPlayerWhite);

            chessContainer.SetActive(true);
            playerFromIndicator.SetActive(false);
            playerToIndicator.SetActive(false);
            opponentFromIndicator.SetActive(false);
            opponentToIndicator.SetActive(false);
            kingCheckIndicator.SetActive(false);

            InitClickAndDrag();
            HidePossibleMoves();
            DisableModalBlocker();
            EnableMenuButton();
            playerInfoPanel.SetActive(true);
            opponentInfoPanel.SetActive(true);
            UpdateBotBar();
            ResetCapturedPieces();

            ToggleTopPanel(true);
            SetMatchType();

            powerModeImage.gameObject.SetActive(vo.powerMode);
            powerModeImage.sprite = vo.powerMode ? powerPlayOnSprite : powerPlayOffSprite;
        }

        private void UpdateInfoPostMove(MoveVO vo, bool isResume = false)
        {
            // Highlight the player's king if its gone into check
            UpdateKingCheckIndicator(vo, isResume);

            // Update the centipawn scores
            UpdateScores(vo, isResume);

            // Update the notation
            //UpdateNotation(vo);
        }

        public void UpdateKingCheckIndicator(MoveVO vo, bool isResume)
        {
            kingCheckIndicator.SetActive(false);

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

            int kingSquareIndex = (checkedColor == ChessColor.BLACK) ? 
                Array.IndexOf(chessboardPieces, blackKing) : 
                Array.IndexOf(chessboardPieces, whiteKing);

            kingCheckIndicator.SetActive(true);
            kingCheckIndicator.transform.position = chessboardSquares[kingSquareIndex].position;

            if (!isResume)
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
            chessContainer.SetActive(false);
        }
    }
}
