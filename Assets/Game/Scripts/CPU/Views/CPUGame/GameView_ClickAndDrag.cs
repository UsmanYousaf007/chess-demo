/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-06-13 11:37:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using UnityEngine;
using TurboLabz.Chess;
using TurboLabz.Common;

namespace TurboLabz.CPUChess
{
    public partial class GameView
    {
        public GameObject dragIndicator;
        public GameObject dragValidIndicator;
        public GameObject dragInvalidIndicator;
        public GameObject dragGhost;

        private CDSModel model;

        public void InitClickAndDrag()
        {
            model = new CDSModel(dragIndicator,
                dragValidIndicator,
                dragInvalidIndicator,
                dragGhost,
                squareClickedSignal,
                chessboardCamera);
            
            foreach(GameObject piece in chessboardPieces)
            {
                if (piece != null)
                {
                    piece.transform.localScale = model.PIECE_DEFAULT_SCALE;
                }
            }
        }

        public void EnablePlayerTurnInteraction()
        {
            HandleEvent(CDSEvent.PLAYER_TURN);
        }

        public void EnableOpponentTurnInteraction()
        {
            HandleEvent(CDSEvent.OPPONENT_TURN);
        }

        public void DisableInteraction()
        {
            HandleEvent(CDSEvent.MOUSE_DRAG_RESET);
        }

        public void OnParentHideClickAndDrag()
        {
            HandleEvent(CDSEvent.MOUSE_DRAG_RESET);
        }

        public void OnApplicationFocusClickAndDrag(bool focus)
        {
            if (!focus)
            {
                HandleEvent(CDSEvent.MOUSE_DRAG_RESET);    
            }
        }

        public void RemoveChessboardListeners()
        {
            for (int squareIndex = 0; squareIndex < 64; ++squareIndex)
            {
                Transform squareTransform = chessboardSquares[squareIndex];

                if (squareTransform != null)
                {
                    ChessboardSquare square = squareTransform.GetComponent<ChessboardSquare>();
                    square.mouseDownSignal.RemoveListener(OnMouseDown);
                    square.mouseDragSignal.RemoveListener(OnMouseDrag);
                    square.mouseEnterSignal.RemoveListener(OnMouseEnter);
                    square.mouseUpSignal.RemoveListener(OnMouseUp);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////
        // Click and Drag Events

        private void HandleEvent(CDSEvent evt)
        {
            //LogUtil.Log("CDSEvent: " + evt);

            if (model.currentState == null)
            {
                model.currentState = new CDSDefault();
            }

            CDS currentState = model.currentState;
            CDS newState = model.currentState.HandleEvent(evt, model);

            if (newState != null)
            {
                model.previousState = currentState;
                model.currentState = newState;
                newState.RenderDisplayOnEnter(model);

                //LogUtil.Log(evt + ": " + newState.GetType().Name, "white");
            }
        }

        private void OnMouseDown(FileRank fileRank, Vector3 mousePosition)
        {
            model.mouseDownPosition = chessboardCamera.ScreenToWorldPoint(mousePosition);
            model.mouseDownLocation = fileRank;
            HandleEvent(CDSEvent.MOUSE_DOWN);
        }

        private void OnMouseDrag(Vector3 mousePosition)
        {
            model.mouseDragPosition = chessboardCamera.ScreenToWorldPoint(mousePosition);

            if (Vector3.Distance(model.mouseDownPosition, model.mouseDragPosition) > CDSModel.DRAG_THRESHOLD)
            {
                HandleEvent(CDSEvent.MOUSE_DRAG);
            }
        }

        private void OnMouseUp()
        {
            HandleEvent(CDSEvent.MOUSE_UP);
        }

        private void OnMouseEnter(FileRank location, Vector3 position)
        {
            model.mouseDragLocation = location;
            int squareIndex = RankFileMap.Map[location.rank, location.file];
            model.mouseDragSquarePosition = chessboardSquares[squareIndex].position;
        }

        private void OnPieceSelected(FileRank location, List<ChessSquare> possibleMoves)
        {
            model.validSquares = possibleMoves;
            int pieceIndex = RankFileMap.Map[location.rank, location.file];
            model.dragPiece = chessboardPieces[pieceIndex];
            model.dragPieceTransform = model.dragPiece.transform;
            model.dragPieceSpriteRenderer = model.dragPiece.GetComponent<SpriteRenderer>();
            model.pieceOriginalPosition = model.dragPieceTransform.position;
            model.pieceSelectionLocation = location;

            HandleEvent(CDSEvent.PLAYER_TURN_PIECE_SELECTED);
        }

        private void OnPieceDeselected()
        {
            HandleEvent(CDSEvent.PLAYER_TURN_PIECE_DESELECTED);
        }

        ////////////////////////////////////////////////////////////////////////
        // Initialization - Once per app instance

        private void AddSquareListeners()
        {
            int squareIndex = 0;

            for (int rank = 0; rank < 8; ++rank)
            {
                for (int file = 0; file < 8; ++file)
                {
                    FileRank fileRank;
                    fileRank.file = file;
                    fileRank.rank = rank;

                    ChessboardSquare square = chessboardSquares[squareIndex].GetComponent<ChessboardSquare>();
                    square.fileRank = fileRank;
                    square.mouseDownSignal.AddListener(OnMouseDown);
                    square.mouseDragSignal.AddListener(OnMouseDrag);
                    square.mouseEnterSignal.AddListener(OnMouseEnter);
                    square.mouseUpSignal.AddListener(OnMouseUp);

                    ++squareIndex;
                }
            }
        }
    }
}
