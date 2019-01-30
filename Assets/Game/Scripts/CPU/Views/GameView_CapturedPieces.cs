/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-16 17:11:51 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

namespace TurboLabz.CPU
{
    public partial class GameView
    {
        public GameObject[] capturedPieces; // Holds references to all piece sprites
        public Transform[] capturedSlotsOpponent; // Holds references to positional empty game objects
        public Transform[] capturedSlotsPlayer; // Holds references to positional empty game objects
        public Image[] capturedIndicatorsOpponent; // Holds references to captured piece indicators
        public Image[] capturedIndicatorsPlayer; // Holds references to captured piece indicators
        public Text[] capturedCountersOpponent; // Holds references to the counter text on each position
        public Text[] capturedCountersPlayer; // Holds references to the counter text on each position

        private int[] capturedCountOpponent = new int[5]; // Remembers the value for each counter text
        private int[] capturedCountPlayer = new int[5]; // Remembers the value for each counter text
        private string[] slotTrackerOpponent = new string[5]; // Remembers which piece is allocated to which slot
        private string[] slotTrackerPlayer = new string[5]; // Remembers which piece is allocated to which slot

        private void OnParentShowCapturedPieces()
        {
            ResetCapturedPieces();
        }

        private void ResetCapturedPieces()
        {
            for (int i = 0; i < 5; ++i)
            {
                capturedCountersOpponent[i].text = "";
                capturedCountersPlayer[i].text = "";
                capturedCountOpponent[i] = 0;
                capturedCountPlayer[i] = 0;
                slotTrackerOpponent[i] = null;
                slotTrackerPlayer[i] = null;
            }

            foreach (GameObject obj in capturedPieces)
            {
                obj.SetActive(false);
            }

            ResetCapturedIndicators(true);
            ResetCapturedIndicators(false);
        }

        private void ResetCapturedIndicators(bool isPlayerTurn)
        {
            Image[]  captureIndicators = isPlayerTurn ? capturedIndicatorsPlayer : capturedIndicatorsOpponent;
            for (int i = 0; i < 5; ++i)
            {
                captureIndicators[i].gameObject.SetActive(false);
            }
        }

        private void HandleCapturePieceGraphic(string pieceName, bool isPlayerTurn)
        {
            Transform[] targetSlots;
            Text[] targetCounters;
            int[] targetCount;
            string[] targetSlotTracker;
            Image[] targetCaptureIndicators;

            if (isPlayerTurn)
            {
                targetSlots = capturedSlotsPlayer;
                targetCounters = capturedCountersPlayer;
                targetCount = capturedCountPlayer;
                targetSlotTracker = slotTrackerPlayer;
                targetCaptureIndicators = capturedIndicatorsPlayer;
            }
            else
            {
                targetSlots = capturedSlotsOpponent;
                targetCounters = capturedCountersOpponent;
                targetCount = capturedCountOpponent;
                targetSlotTracker = slotTrackerOpponent;
                targetCaptureIndicators = capturedIndicatorsOpponent;
            }

            // Find the slot for this piece
            int slotIndex = 0;

            for (int i = 0; i < 5; ++i)
            {
                slotIndex = i;

                if (targetSlotTracker[i] == null)
                {
                    targetSlotTracker[i] = pieceName;
                    break;
                }
                else if (targetSlotTracker[i] == pieceName)
                {
                    break;
                }
            }

            // Activate the piece graphic and set its position
            foreach (GameObject piece in capturedPieces)
            {
                if (piece.name == pieceName)
                {
                    piece.SetActive(true);
                    piece.transform.position = targetSlots[slotIndex].position;

                    ResetCapturedIndicators(isPlayerTurn);
                    targetCaptureIndicators[slotIndex].gameObject.SetActive(true);
                    break;
                }
            }

            // Finally update the counter
            ++targetCount[slotIndex];

            if (targetCount[slotIndex] > 1)
            {
                targetCounters[slotIndex].text = "x" + targetCount[slotIndex];
            }
        }
    }
}
