/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-08-01 14:21:03 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using UnityEngine;
using strange.extensions.signal.impl;
using UnityEngine.UI;
using TurboLabz.InstantGame;

namespace TurboLabz.Chess
{
    public class CDSModel
    {
        public const float DRAG_THRESHOLD = 10f;
        public const float PIECE_PICK_OFFSET = 125f;
        public const int PIECE_SORTING_ORDER = 1;

        public readonly Vector3 PIECE_ZOOM_SCALE = new Vector3(200f, 200f, 200f);
        public readonly Vector3 PIECE_DEFAULT_SCALE = new Vector3(100f, 100f, 100f);

        public CDS currentState;
        public CDS previousState;
        public Vector3 mouseDownPosition;
        public Vector3 mouseDragPosition;
        public Vector3 pieceOriginalPosition;
        public Vector3 mouseDragSquarePosition;
        public GameObject dragPiece;
        public Transform dragPieceTransform;
        public SpriteRenderer dragPieceSpriteRenderer;
        public FileRank mouseDownLocation;
        public FileRank pieceSelectionLocation;
        public FileRank mouseDragLocation;
        public List<ChessSquare> validSquares;
        public GameObject dragIndicator;
        public GameObject dragValidIndicator;
        public GameObject dragInvalidIndicator;
        public Signal<FileRank> squareClickedSignal;
        public Camera camera;
        public GameObject dragGhost;
        public Transform dragGhostTransform;
        public Image dragGhostImage;

        public CDSModel(GameObject dragIndicator,
                        GameObject dragValidIndicator,
                        GameObject dragInvalidIndicator,
                        GameObject dragGhost,
                        Signal<FileRank> squareClickedSignal,
                        Camera camera)
        {
            this.dragIndicator = dragIndicator;
            this.dragValidIndicator = dragValidIndicator;
            this.dragInvalidIndicator = dragInvalidIndicator;
            this.dragGhost = dragGhost;
            this.dragGhostTransform = dragGhost.transform;
            this.dragGhostImage = dragGhost.GetComponent<Image>();
            this.squareClickedSignal = squareClickedSignal;
            this.camera = camera;
            this.currentState = null;
            mouseDownPosition = Vector3.zero;
            mouseDragPosition = Vector3.zero;
            pieceOriginalPosition = Vector3.zero;
            mouseDragSquarePosition = Vector3.zero;
            dragPiece = null;
            dragPieceSpriteRenderer = null;
            mouseDownLocation.file = -1;
            mouseDownLocation.rank = -1;
            pieceSelectionLocation.file = -1;
            pieceSelectionLocation.rank = -1;
            mouseDragLocation.file = -1;
            mouseDragLocation.rank = -1;
            validSquares = null;
            dragIndicator.SetActive(false);
            dragValidIndicator.SetActive(false);
            dragInvalidIndicator.SetActive(false);
        }
    }
}
