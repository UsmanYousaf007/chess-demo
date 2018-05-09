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
/// 

using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

namespace TurboLabz.Chess
{
    public class ChessboardRefs : MonoBehaviour {
        // Skin Objects
        public GameObject chessContainer;
        public Transform chessboard;
        public SpriteRenderer background;
        public Transform[] chessboardSquares;
        public GameObject[] pieces;
        public GameObject[] possibleMoveIndicators;
        public GameObject kingCheckIndicator;
        public GameObject playerFromIndicator;
        public GameObject playerToIndicator;
        public GameObject opponentFromIndicator;
        public GameObject opponentToIndicator;
        public GameObject fileRankLabelsForward;
        public GameObject fileRankLabelsBackward;
        public GameObject chessboardBlocker;
        public GameObject dragIndicator;
        public GameObject dragValidIndicator;
        public GameObject dragInvalidIndicator;
        public GameObject hintFromIndicator;
        public GameObject hintToIndicator;
        public Image promoBg;
        public Image[] promoPieces;
        public Camera chessboardCamera;

        // Unique skinnable objects for each game mode UI
        public GameObject[] cpuCapturedPieces;
        public GameObject cpuDragGhost;

        // Skin support info
        public const string IMAGE_POSSIBLE_MOVE = "PossibleMove";
        public readonly Quaternion WHITE_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -270f));
        public readonly Vector3 WHITE_BOARD_POSITION =  new Vector3(420f, -420f, 10f);
        public readonly Quaternion BLACK_BOARD_ROTATION = Quaternion.Euler(new Vector3(0f, 0f, -90f));
        public readonly Vector3 BLACK_BOARD_POSITION = new Vector3(-420f, 420f, 10f);

        private string currentSkinId;

        public void ApplySkin(string skinId)
        {
            LogUtil.Log("Applying skin = " + skinId, "cyan");
            if (skinId == currentSkinId)
            {
                return;
            }
                
            currentSkinId = skinId;
            SkinContainer container = SkinContainer.LoadSkin(skinId);
            background.sprite = container.GetSprite(SkinContainer.SPRITE_BACKGROUND);
            promoBg.sprite = container.GetSprite(SkinContainer.PROMO_BG);

            foreach (GameObject piece in pieces)
            {
                piece.GetComponent<SpriteRenderer>().sprite = container.GetSprite(piece.name);
            }

            foreach (Image promoPiece in promoPieces)
            {
                promoPiece.sprite = container.GetSprite(promoPiece.name);
            }

            foreach (GameObject capturedPiece in cpuCapturedPieces)
            {
                capturedPiece.GetComponent<Image>().sprite = container.GetSprite(capturedPiece.name, true);
            }
        }
    }
}
