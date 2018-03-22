/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:54:49 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

namespace TurboLabz.Chess
{
    public class CDSPlayerTurnPieceDragging : CDS
    {
        // Render dragging
        public override void RenderDisplayOnEnter(CDSModel model)
        {
            model.dragPiece.SetActive(false);
            model.dragGhost.SetActive(true);

            model.mouseDragPosition.z = model.pieceOriginalPosition.z;
            model.mouseDragPosition.y += CDSModel.PIECE_PICK_OFFSET;
            model.dragPieceSpriteRenderer.sortingOrder = CDSModel.PIECE_SORTING_ORDER;
            model.dragPieceTransform.position = model.mouseDragPosition;

            // Now our ghost represents the scaled piece
            //model.dragPieceTransform.localScale = model.PIECE_ZOOM_SCALE;

            model.dragIndicator.transform.position = model.mouseDragSquarePosition;
            model.dragIndicator.SetActive(true);

            model.dragValidIndicator.SetActive(false);
            model.dragInvalidIndicator.SetActive(false);

            if (model.mouseDragLocation != model.mouseDownLocation)
            {
                if (IsValidMoveLocation(model.mouseDragLocation, model))
                {
                    model.dragValidIndicator.SetActive(true);
                    model.dragValidIndicator.transform.position = model.mouseDragSquarePosition;
                }
                else
                {
                    model.dragInvalidIndicator.SetActive(true);
                    model.dragInvalidIndicator.transform.position = model.mouseDragSquarePosition;
                }
            }

            UpdateGhost(model);
        }

        public override CDS HandleEvent(CDSEvent evt, CDSModel model)
        {
            if (evt == CDSEvent.MOUSE_DRAG)
            {
                return this;
            }
            else if (evt == CDSEvent.MOUSE_UP)
            {
               CancelRenderDragging(model);

               if (IsValidMoveLocation(model.mouseDragLocation, model))
               {
                    return new CDSPlayerTurnMoved();
               }
               else
               {
                    model.dragPieceTransform.position = model.pieceOriginalPosition;
                    return new CDSPlayerTurnPieceSelected();
                }
            }
            else if (evt == CDSEvent.MOUSE_DRAG_RESET)
            {
                CancelRenderDragging(model);
                model.dragPieceTransform.position = model.pieceOriginalPosition;
                return new CDSPlayerTurnPieceSelected();
            }

            return null;
        }

        private void UpdateGhost(CDSModel model)
        {
            // Match sprite
            model.dragGhostImage.sprite = model.dragPieceSpriteRenderer.sprite;

            // Match position
            Vector3 screenPosition = model.camera.WorldToScreenPoint(model.dragPieceTransform.position);
            model.dragGhostTransform.position = screenPosition;
        }

        private void CancelRenderDragging(CDSModel model)
        {
            if (model.dragPiece != null)
            {
                model.dragPieceTransform.localScale = new Vector3(100f, 100f, 100f);
                model.dragPieceSpriteRenderer.sortingOrder = 0;
                model.dragPiece.SetActive(true);
            }

            model.dragIndicator.SetActive(false);
            model.dragValidIndicator.SetActive(false);
            model.dragInvalidIndicator.SetActive(false);

            model.dragGhost.SetActive(false);

        }
    }
}
