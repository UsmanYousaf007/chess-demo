/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 17:37:45 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public GameObject findDlg;
        public Text searchingLabel;

        public void InitFind()
        {
            searchingLabel.text = localizationService.Get(LocalizationKey.MULTIPLAYER_SEARCHING);
        }

        public void ShowFind()
        {
            EnableModalBlocker();
            findDlg.SetActive(true);

            DisableMenuButton();
        }

        public void HideFind()
        {
            DisableModalBlocker();
            findDlg.SetActive(false);

            EnableMenuButton();
        }

        public void SetupFindMode()
        {
            chessContainer.SetActive(true);
            InitClickAndDrag();

            fileRankLabelsForward.SetActive(false);
            fileRankLabelsBackward.SetActive(false);
            playerFromIndicator.SetActive(false);
            playerToIndicator.SetActive(false);
            opponentFromIndicator.SetActive(false);
            opponentToIndicator.SetActive(false);
            kingCheckIndicator.SetActive(false);
            HidePossibleMoves();
            DisableMenuButton();
            playerInfoPanel.SetActive(false);
            opponentInfoPanel.SetActive(false);

            // Reset the piece image pool
            foreach (GameObject obj in activatedPieceImages)
            {
                pool.ReturnObject(obj);
            }

            activatedPieceImages.Clear();

            EnableModalBlocker();
        }
    }
}
