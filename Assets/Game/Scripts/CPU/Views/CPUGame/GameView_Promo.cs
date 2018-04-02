/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:38:06 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.Chess;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public partial class GameView
    {
        public GameObject promoParent;
        public Button promoWhiteQueenButton;
        public Button promoBlackQueenButton;
        public Button promoWhiteRookButton;
        public Button promoBlackRookButton;
        public Button promoWhiteBishopButton;
        public Button promoBlackBishopButton;
        public Button promoWhiteKnightButton;
        public Button promoBlackKnightButton;

        public Signal<string> promoClickedSignal = new Signal<string>();

        public void InitPromotions()
        {
            promoWhiteQueenButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.WHITE_QUEEN); });
            promoWhiteRookButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.WHITE_ROOK); });
            promoWhiteBishopButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.WHITE_BISHOP); });
            promoWhiteKnightButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.WHITE_KNIGHT); });
            promoBlackQueenButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.BLACK_QUEEN); });
            promoBlackRookButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.BLACK_ROOK); });
            promoBlackBishopButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.BLACK_BISHOP); });
            promoBlackKnightButton.onClick.AddListener(() => { OnPromoClicked(ChessPieceName.BLACK_KNIGHT); });
        }

        public void CleanupPromotions()
        {
            promoWhiteQueenButton.onClick.RemoveAllListeners();
            promoWhiteRookButton.onClick.RemoveAllListeners();
            promoWhiteBishopButton.onClick.RemoveAllListeners();
            promoWhiteKnightButton.onClick.RemoveAllListeners();
            promoBlackQueenButton.onClick.RemoveAllListeners();
            promoBlackRookButton.onClick.RemoveAllListeners();
            promoBlackBishopButton.onClick.RemoveAllListeners();
            promoBlackKnightButton.onClick.RemoveAllListeners();
        }

        public void OnPromoClicked(string pieceName)
        {
            HidePromoDialog();
            promoClickedSignal.Dispatch(pieceName);
            audioSource.PlayOneShot(promoSoundFx, FX_VOLUME);
        }

        public void UpdatePromoDialog(ChessColor color)
        {
            bool blackActive = false;
            bool whiteActive = false;

            if (color == ChessColor.BLACK)
            {
                blackActive = true;
            }
            else
            {
                whiteActive = true;
            }

            promoWhiteQueenButton.gameObject.SetActive(whiteActive);
            promoWhiteRookButton.gameObject.SetActive(whiteActive);
            promoWhiteBishopButton.gameObject.SetActive(whiteActive);
            promoWhiteKnightButton.gameObject.SetActive(whiteActive);
            promoBlackQueenButton.gameObject.SetActive(blackActive);
            promoBlackRookButton.gameObject.SetActive(blackActive);
            promoBlackBishopButton.gameObject.SetActive(blackActive);
            promoBlackKnightButton.gameObject.SetActive(blackActive);

            DisableUndoButton();
        }

        public void ShowPromoDialog()
        {
            EnableModalBlocker();
            promoParent.SetActive(true);

            DisableHintButton();
            StashMenuButton();
            StashUndoButton();
        }

        public void HidePromoDialog()
        {
            DisableModalBlocker();
            promoParent.SetActive(false);

            PopMenuButton();
            PopUndoButton();
        }

        public bool IsPromoActive()
        {
            return promoParent.activeSelf;
        }

        public void UpdatePromoDialog(MoveVO moveVO)
        {
            // Find out the promoted piece name based on the moveFlag in the vo
            ChessMoveFlag moveFlag = moveVO.moveFlag;
            string promoPieceName = null;
            bool isBlackPiece = (moveVO.pieceColor == ChessColor.BLACK);

            if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_QUEEN)
            {   
                promoPieceName = isBlackPiece ? ChessPieceName.BLACK_QUEEN : ChessPieceName.WHITE_QUEEN;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_ROOK)
            {
                promoPieceName = isBlackPiece ? ChessPieceName.BLACK_ROOK : ChessPieceName.WHITE_ROOK;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_BISHOP)
            {
                promoPieceName = isBlackPiece ? ChessPieceName.BLACK_BISHOP : ChessPieceName.WHITE_BISHOP;
            }
            else if (moveFlag == ChessMoveFlag.PAWN_PROMOTION_KNIGHT)
            {
                promoPieceName = isBlackPiece ? ChessPieceName.BLACK_KNIGHT : ChessPieceName.WHITE_KNIGHT;
            }

            // Grab the new piece from the pool
            GameObject promoPieceImage = pool.GetObject(promoPieceName);

            // Grab the pawn piece image that is about to be destroyed
            FileRank toFileRank = moveVO.toSquare.fileRank;
            int toSquareIndex = RankFileMap.Map[toFileRank.rank, toFileRank.file];
            GameObject pawn = chessboardPieces[toSquareIndex];

            // It is possible for the pool to create a new parentless object
            // in the case of promotions. So we add it to the chessContainer here
            if (promoPieceImage.transform.parent == null)
            {
                promoPieceImage.transform.parent = chessContainer.transform;
            }

            // Place the new piece image in the appropriate location
            activatedPieceImages.Add(promoPieceImage);
            promoPieceImage.transform.position = pawn.transform.position;
            chessboardPieces[toSquareIndex] = promoPieceImage;

            // Destroy the pawn
            activatedPieceImages.Remove(pawn);
            pool.ReturnObject(pawn);
        }

        private void OnParentShowPromotions()
        {
            HidePromoDialog();
        }
    }
}
