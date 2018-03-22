/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 11:33:35 UTC+05:00
/// 
/// @description
/// [add_description_here]
/// 
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

using TurboLabz.Common;

namespace TurboLabz.Gamebet
{
    public partial class RoomsView : View
    {
        // TODO: Remove this injection, views cannot inject services or models
        [Inject] public ILocalizationService localizationService { get; set; }

        public float scaleMaxPct = 0f;
        public float scaleAmountPct = 0f;

        // Sprite cache is taken from the SpriteCache gameobject in the scene
        public SpriteCache spriteCache;
        public TextColorCache textColorCache;

        public RoomCard roomCardPrefab;

        public GameObject pagination;
        public ScrollDot scrollDotPrefab;
        public ScrollSpecialDot scrollSpecialDotPrefab;

        public Text currency1Label;
        public Button currency1BuyButton;

        public Text currency2Label;
        public Button currency2BuyButton;

        public Button freeCurrency1Button;
        public Text freeCurrency1ButtonLabel;

        public GameObject scrollContent;

        public Toggle gameDuration1Toggle;
        public Text gameDuration1ToggleLabel;
        public Toggle gameDuration2Toggle;
        public Text gameDuration2ToggleLabel;
        public Toggle gameDuration3Toggle;
        public Text gameDuration3ToggleLabel;

        public Button shopButton;
        public Button backButton;

        public HorizontalScrollSnap horizontalScrollSnap;

        RoomsVO roomsVo;

        // View signals
        public Signal currency1BuyButtonClickedSignal = new Signal();
        public Signal currency2BuyButtonClickedSignal = new Signal();
        public Signal freeCurrency1ButtonClickedSignal = new Signal();
        public Signal<string> roomButtonClickedSignal = new Signal<string>();
        public Signal shopButtonClickedSignal = new Signal();
        public Signal rotatingRoomTimeCompleteSignal = new Signal();
        public Signal backButtonClickedSignal = new Signal();

        // This ordered dictionary groups the rooms by their group IDs.
        //
        // key: room group id
        // value: dictionary
        //     key: game duration
        //     value: room info
        private IDictionary<string, RoomRecordVO> roomRecords;

        // Note that we could've sent game duration key info from the server
        // which could dictate what buttons to show rather than harcoding the
        // values here but we won't do that since this is a very Chess specific
        // feature i.e. to have game duration button selector. Other games
        // might not have that; we want to keep Gamebet agnostic to game
        // specific features.
        //
        // TODO: Moving forward we can possibly design an architecture which
        // lets add custom functionality to Gamebet per game in a very isolated
        // manner.

        private string cardIdWithHighestPossibleFee;

        // Game duration keys in milliseconds.
        private const long GAME_DURATION_1_KEY = 1 * 60 * 1000;
        private const long GAME_DURATION_2_KEY = 3 * 60 * 1000;
        private const long GAME_DURATION_3_KEY = 10 * 60 * 1000;

        private const long GAME_DURATION_5_KEY = 5 * 60 * 1000;

        private bool openFirstTime = true;

        private List<Transform> cardTransforms;
        private float centerOfScreen = Screen.width / 2f;

        private List<GameObject> cardTapable;
        private List<Toggle> scrollDots;
        private int currentCard;
        private int prevCard;

        public void Init()
        {
            currency1BuyButton.onClick.AddListener(OnCurrency1BuyButtonClicked);
            currency2BuyButton.onClick.AddListener(OnCurrency2BuyButtonClicked);
            freeCurrency1Button.onClick.AddListener(OnFreeCurrency1ButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
        }

        void OnDisable()
        {
            StopRoomCountdownClockCR(roomsVo);
            StopNewRoomCountdownClockCR(roomsVo);
        }

        void Update()
        {
            if (scrollContent.transform.childCount > 0)
            {
                foreach (Transform roomCard in cardTransforms)
                {
                    float distanceFromCenter = Mathf.Abs(roomCard.position.x - centerOfScreen);
                    float progressToCenter = Mathf.Abs(Mathf.Min(centerOfScreen, distanceFromCenter) / centerOfScreen);
                    float scalePct = scaleMaxPct - (scaleAmountPct * progressToCenter);
                    roomCard.transform.localScale = RoomsViewConstants.maxCardScale * scalePct;
                }
            }
        }

        public void UpdateView(RoomsVO vo)
        {
            roomsVo = vo;
            currency1Label.text = vo.currency1.ToString("N0");
            currency2Label.text = vo.currency2.ToString("N0");
            cardIdWithHighestPossibleFee = vo.cardIdOfHighestAffordableRoom;

            freeCurrency1ButtonLabel.text = localizationService.Get(LocalizationKey.ROOMS_FREE_CURRENCY_1_BUTTON_LABEL);

            roomRecords = vo.roomRecords;

            UpdateCards(vo);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetTapBlockerCard(int cardIndex)
        {
            prevCard = currentCard;
            currentCard = cardIndex;

            cardTapable[prevCard].SetActive(true);
            cardTapable[currentCard].SetActive(false);
        }

        private void UpdateCards(RoomsVO vo)
        {
            UnityUtil.DestroyChildren(scrollContent.transform);
            UnityUtil.DestroyChildren(pagination.transform);

            bool roomsFound = false;
            int scrollStartingScreen = 0;

            cardTransforms = new List<Transform>();
            cardTapable = new List<GameObject>();
            scrollDots = new List<Toggle>();

            foreach (KeyValuePair<string, RoomInfo> roomInfo in vo.rooms)
            {
                roomsFound = true;

                RoomSetting roomSetting = roomInfo.Value.setting;
                string roomId = roomSetting.id;
                int gameDurationMinutes = (int)(roomSetting.gameDuration / 60000);

                if (roomSetting.groupId == RoomsConstants.ROTATING_ROOMS_GROUP_ID)
                {
                    ScrollSpecialDot scrollSpecialDot = Instantiate<ScrollSpecialDot>(scrollSpecialDotPrefab);
                    scrollSpecialDot.transform.SetParent(pagination.transform, false);

                    scrollDots.Add(scrollSpecialDot.GetComponent<Toggle>());
                }
                else
                {
                    ScrollDot scrollDot = Instantiate<ScrollDot>(scrollDotPrefab);
                    scrollDot.transform.SetParent(pagination.transform, false);

                    scrollDots.Add(scrollDot.GetComponent<Toggle>());
                }

                RoomCard card = Instantiate<RoomCard>(roomCardPrefab);

                card.transform.GetChild(0).localScale = RoomsViewConstants.minCardScale;
                cardTransforms.Add(card.transform.GetChild(0).transform);

                card.TapBlocker.SetActive(true);
                cardTapable.Add(card.TapBlocker);

                if (roomSetting.id == cardIdWithHighestPossibleFee && openFirstTime)
                {
                    openFirstTime = false;

                    horizontalScrollSnap.StartingScreen = scrollStartingScreen;
                    currentCard = scrollStartingScreen;
                }

                ++scrollStartingScreen;

                if (roomSetting.groupId == RoomsConstants.STANDARD_ROOMS_GROUP_ID)
                {
                    UpdateStandardCard(card, roomSetting, roomInfo, roomId, gameDurationMinutes);
                }
                else if (roomSetting.groupId == RoomsConstants.ALL_IN_ONE_ROOMS_GROUP_ID)
                {
                    UpdateAllInOneCard(card, roomSetting, roomInfo, roomId, gameDurationMinutes);
                }
                else if (roomSetting.groupId == RoomsConstants.ROTATING_ROOMS_GROUP_ID)
                {
                    UpdateRotatingCard(card, roomSetting, roomInfo, roomId, gameDurationMinutes);
                }
                // It is very important that worldPositionStays is set to false
                // otherwise the card will have incorrect size on screen
                // sizes/resolutions other than the one it is originally
                // designed for.
                card.transform.SetParent(scrollContent.transform, false);

                // TODO: Add "XP Gain" to room card.
                // TODO: Add "Games Won/Lost/Drawn" to room card.
            }

            cardTransforms[currentCard].localScale = RoomsViewConstants.maxCardScale;
            scrollDots[currentCard].isOn = true;
            SetTapBlockerCard(currentCard);

            Assertions.Assert(roomsFound, "No rooms exist ");
        }


        #region StandardRoomCard
        private void UpdateStandardCard(RoomCard card, RoomSetting roomSetting, KeyValuePair<string, RoomInfo> roomInfo, string roomId, int gameDurationMinutes)
        {
            if (roomInfo.Value.isMystery)
            {
                card.mysteryCard.SetActive(true);
            }
            else
            {
                card.standardRoomCard.roomCard.SetActive(true);
            }

            if (roomInfo.Value.isLocked)
            {
                card.lockedCard.SetActive(true);
            }
            else
            {
                card.standardRoomCard.lockedItems.SetActive(true);
            }

            card.gameObject.name = roomCardPrefab.name + char.ToUpper(roomId[0]) + roomId.Substring(1);

            // TODO: Check if these listeners need to be removed.
            card.roomButton.onClick.AddListener(() => { OnRoomButtonClicked(roomId); });

            card.standardRoomCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);
            card.standardRoomCard.roomBanner.sprite = spriteCache.GetRoomCardBanner(roomId);

            card.standardRoomCard.flag.sprite = spriteCache.GetRoomFlagMajor(roomId);
            card.standardRoomCard.roomNameLabel.text = localizationService.Get(roomId); // TODO(mubeeniqbal): This should be all uppercase. Figure out how to make it uppercase while using localization.
            card.standardRoomCard.roomDurationLabel.text = localizationService.Get(LocalizationKey.RC_ROOM_DURATION_LABEL, gameDurationMinutes);

            card.standardRoomCard.prizeTitleLabel.text = localizationService.Get(LocalizationKey.RC_PRIZE_TITLE_LABEL);
            card.standardRoomCard.prizeAmountLabel.text = roomSetting.prize.ToString("N0");

            card.standardRoomCard.entryFeeTitleLabel.text = localizationService.Get(LocalizationKey.RC_ENTRY_FEE_TITLE_LABEL);
            card.standardRoomCard.entryFeeLabel.text = roomSetting.wager.ToString("N0");

            card.standardRoomCard.unlockLevelText.text = localizationService.Get(LocalizationKey.RC_UNLOCK_LEVEL_LABLE, roomSetting.unlockAtLevel);

            Assertions.Assert(roomRecords.ContainsKey(roomId),
                "The room record for room ID " + roomId + " does not exist!");

            RoomRecordVO standardRecordVO = roomRecords[roomId];

            card.standardRoomCard.roomTitleLabel.text = localizationService.GetRoomTitle(standardRecordVO.roomTitleId);

            int standardTrophiesWon = standardRecordVO.trophiesWon;
            string standardZeroTrophyCountLabelText = localizationService.Get(LocalizationKey.RC_ZERO_TROPHY_COUNT_LABEL);
            string standardTrophyCountLabelText = localizationService.Get(LocalizationKey.RC_TROPHY_COUNT_LABEL, standardTrophiesWon);

            card.standardRoomCard.trophyCountLabel.text = (standardTrophiesWon == 0) ? standardZeroTrophyCountLabelText : standardTrophyCountLabelText;

            int standardWinsForTrophy = roomSetting.winsForTrophy;
            int standardGamesWon = standardRecordVO.gamesWon;
            int standardCurrentWinsForNextTrophy = standardGamesWon % standardWinsForTrophy;

            card.standardRoomCard.winsNeededTitleLabel.text = localizationService.Get(LocalizationKey.RC_WINS_NEEDED_TITLE_LABEL);
            card.standardRoomCard.trophyProgressFill.fillAmount = (float)standardCurrentWinsForNextTrophy / standardWinsForTrophy;
            card.standardRoomCard.trophyWinsNeededLabel.text = localizationService.Get(LocalizationKey.RC_TROPHY_WINS_NEEDED_LABEL, standardCurrentWinsForNextTrophy, standardWinsForTrophy);

            card.standardRoomCard.infoBg.sprite = spriteCache.GetRoomCardInfoBg(roomId);

            card.standardRoomCard.mysterCardText.text = localizationService.Get(LocalizationKey.RC_MYSTERY_CARD_LABLE);

            RoomCard._RoomInfo infoCard = card.infoCard;

            // TODO: Check if these listeners need to be removed.
            card.standardRoomCard.infoButton.onClick.AddListener(() => { OnInfoButtonClicked(card.roomCard, infoCard.card); });

            infoCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);

            infoCard.rulesLabel.text = localizationService.Get(LocalizationKey.RIC_RULES_LABEL, roomSetting.winsForTrophy);

            infoCard.roomTitle1.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_1, roomSetting.trophiesForRoomTitle1);
            infoCard.roomTitle1.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_1);

            infoCard.roomTitle2.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_2, roomSetting.trophiesForRoomTitle2);
            infoCard.roomTitle2.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_2);

            infoCard.roomTitle3.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_3, roomSetting.trophiesForRoomTitle3);
            infoCard.roomTitle3.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_3);

            infoCard.backButton.onClick.AddListener(() => { OnInfoCardBackButtonClicked(card.roomCard, infoCard.card); });
        }
        #endregion

        #region AllInOneRoomCard
        private void UpdateAllInOneCard(RoomCard card, RoomSetting roomSetting, KeyValuePair<string, RoomInfo> roomInfo, string roomId, int gameDurationMinutes)
        {
            card.allInOneCard.roomCard.SetActive(true);

            if (roomInfo.Value.isLocked)
            {
                card.lockedCard.SetActive(true);
            }
            else
            {
                card.allInOneCard.lockedItems.SetActive(true);
            }

            card.gameObject.name = roomCardPrefab.name + char.ToUpper(roomId[0]) + roomId.Substring(1);

            // TODO: Check if these listeners need to be removed.
            card.roomButton.onClick.AddListener(() => { OnRoomButtonClicked(roomId); });

            card.allInOneCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);

            card.allInOneCard.roomBanner.sprite = spriteCache.GetRoomCardBanner(roomId);
            card.allInOneCard.roomDurationLabel.text = localizationService.Get(LocalizationKey.RC_ROOM_DURATION_LABEL, gameDurationMinutes);

            card.allInOneCard.prizeTitleLabel.text = localizationService.Get(LocalizationKey.RC_PRIZE_TITLE_LABEL);

            card.allInOneCard.prizeDoubleLabel.text = localizationService.Get(LocalizationKey.RC_PRIZE_DESCRIPTION_LABEL);
            card.allInOneCard.prizeMultiplierLabel.text = localizationService.Get(LocalizationKey.RC_2X);

            card.allInOneCard.entryFeeTitleLabel.text = localizationService.Get(LocalizationKey.RC_MINIMUM_ENTRY_FEE_TITLE_LABEL);
            card.allInOneCard.entryFeeLabel.text = roomSetting.wager.ToString("N0");

            card.allInOneCard.unlockLevelText.text = localizationService.Get(LocalizationKey.RC_UNLOCK_LEVEL_LABLE, roomSetting.unlockAtLevel);

            Assertions.Assert(roomRecords.ContainsKey(roomId),
                "The room record for room ID " + roomId + " does not exist!");

            RoomRecordVO AllInOneRecordVO = roomRecords[roomId];

            card.allInOneCard.roomTitleLabel.text = localizationService.GetRoomTitle(AllInOneRecordVO.roomTitleId);

            int AllInOneTrophiesWon = AllInOneRecordVO.trophiesWon;
            string AllInOneZeroTrophyCountLabelText = localizationService.Get(LocalizationKey.RC_ZERO_TROPHY_COUNT_LABEL);
            string AllInOneTrophyCountLabelText = localizationService.Get(LocalizationKey.RC_TROPHY_COUNT_LABEL, AllInOneTrophiesWon);

            card.allInOneCard.trophyCountLabel.text = (AllInOneTrophiesWon == 0) ? AllInOneZeroTrophyCountLabelText : AllInOneTrophyCountLabelText;

            int AllInOneWinsForTrophy = roomSetting.winsForTrophy;
            int AllInOneGamesWon = AllInOneRecordVO.gamesWon;
            int AllInOneCurrentWinsForNextTrophy = AllInOneGamesWon % AllInOneWinsForTrophy;

            card.allInOneCard.winsNeededTitleLabel.text = localizationService.Get(LocalizationKey.RC_WINS_NEEDED_TITLE_LABEL);
            card.allInOneCard.trophyProgressFill.fillAmount = (float)AllInOneCurrentWinsForNextTrophy / AllInOneWinsForTrophy;
            card.allInOneCard.trophyWinsNeededLabel.text = localizationService.Get(LocalizationKey.RC_TROPHY_WINS_NEEDED_LABEL, AllInOneCurrentWinsForNextTrophy, AllInOneWinsForTrophy);

            card.allInOneCard.infoBg.sprite = spriteCache.GetRoomCardInfoBg(roomId);

            RoomCard._RoomInfo infoCard = card.AllInOneInfoCard;

            // TODO: Check if these listeners need to be removed.
            card.allInOneCard.infoButton.onClick.AddListener(() => { OnInfoButtonClicked(card.roomCard, infoCard.card); });

            infoCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);

            infoCard.rulesLabel.text = localizationService.Get(LocalizationKey.RIC_RULES_LABEL, roomSetting.winsForTrophy);

            infoCard.roomTitle1.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_1, roomSetting.trophiesForRoomTitle1);
            infoCard.roomTitle1.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_1);

            infoCard.roomTitle2.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_2, roomSetting.trophiesForRoomTitle2);
            infoCard.roomTitle2.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_2);

            infoCard.roomTitle3.trophiesNeededLabel.text = localizationService.Get(LocalizationKey.RIC_TROPHIES_NEEDED_FOR_ROOM_TITLE_3, roomSetting.trophiesForRoomTitle3);
            infoCard.roomTitle3.roomTitleLabel.text = localizationService.Get(LocalizationKey.ROOM_TITLE_3);

            infoCard.backButton.onClick.AddListener(() => { OnInfoCardBackButtonClicked(card.roomCard, infoCard.card); });
        }
        #endregion

        #region RotatingRoomCard
        private void UpdateRotatingCard(RoomCard card, RoomSetting roomSetting, KeyValuePair<string, RoomInfo> roomInfo, string roomId, int gameDurationMinutes)
        {
            card.rotatingRoomCard.roomCard.SetActive(true);

            if (roomInfo.Value.isLocked)
            {
                card.lockedCard.SetActive(true);
            }
            else
            { 
                card.rotatingRoomCard.lockedItems.SetActive(true);
            }

            card.gameObject.name = roomCardPrefab.name + char.ToUpper(roomId[0]) + roomId.Substring(1);

            // TODO: Check if these listeners need to be removed.
            card.roomButton.onClick.AddListener(() => { OnRoomButtonClicked(roomId); });

            card.rotatingRoomCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);
            card.rotatingRoomCard.roomBanner.sprite = spriteCache.GetRoomCardBanner(roomId);

            card.rotatingRoomCard.roomName.sprite = spriteCache.GetRotatingRoomName(roomId);

            card.standardRoomCard.roomDurationLabel.text = localizationService.Get(LocalizationKey.RC_ROOM_DURATION_LABEL, gameDurationMinutes);

            card.rotatingRoomCard.prizeTitleLabel.text = localizationService.Get(LocalizationKey.RC_PRIZE_TITLE_LABEL);
            card.rotatingRoomCard.prizeAmountLabel.text = roomSetting.prize.ToString("N0");

            card.rotatingRoomCard.entryFeeTitleLabel.text = localizationService.Get(LocalizationKey.RC_ENTRY_FEE_TITLE_LABEL);
            card.rotatingRoomCard.entryFeeLabel.text = roomSetting.wager.ToString("N0");

            card.rotatingRoomCard.unlockLevelText.text = localizationService.Get(LocalizationKey.RC_UNLOCK_LEVEL_LABLE, roomSetting.unlockAtLevel);

            card.rotatingRoomCard.rewardedGoalsLabel.text = localizationService.Get(LocalizationKey.RC_REWARDED_GOALS_LABEL);

            // To do <irtaza> The progress value below are hard coded. These values will come from server.
            card.rotatingRoomCard.progressLabel.text = localizationService.Get(LocalizationKey.RC_PROGRESS_LABEL,1,3);
            card.rotatingRoomCard.progressLabel.color = textColorCache.GetProgressFieldColor(roomId);
            card.rotatingRoomCard.nextRoomText.text = localizationService.Get(LocalizationKey.RC_NEXT_ROOM_LABEL);

            if (!roomInfo.Value.isLocked)
            {
                roomInfo.Value.roomCountdownCR = StartCoroutine(RoomCountdownCR(roomInfo.Value.timeRemaining, card.rotatingRoomCard.timeRemainingLabel, roomInfo.Value, card));
            }

            card.rotatingRoomCard.infoBg.sprite = spriteCache.GetRoomCardInfoBg(roomId);
            card.rotatingRoomCard.goalsButtonBg.sprite = spriteCache.GetRotatingRoomGoalsButtonBg(roomId);

            RoomCard._RotatingRoomInfo infoCard = card.rotatingRoomInfoCard;

            // TODO: Check if these listeners need to be removed.
            card.rotatingRoomCard.infoButton.onClick.AddListener(() => { OnInfoButtonClicked(card.roomCard, infoCard.card); });

            infoCard.background.sprite = spriteCache.GetRoomCardBackground(roomId);
            infoCard.description.text = roomSetting.roomDescription;

            infoCard.backButton.onClick.AddListener(() => { OnInfoCardBackButtonClicked(card.roomCard, infoCard.card); });
        }
        #endregion

        private void OnCurrency1BuyButtonClicked()
        {
            currency1BuyButtonClickedSignal.Dispatch();
        }

        private void OnCurrency2BuyButtonClicked()
        {
            currency2BuyButtonClickedSignal.Dispatch();
        }

        private void OnFreeCurrency1ButtonClicked()
        {
            freeCurrency1ButtonClickedSignal.Dispatch();
        }

        private void OnRoomButtonClicked(string roomId)
        {
            roomButtonClickedSignal.Dispatch(roomId);
        }

        private void OnInfoButtonClicked(GameObject roomCard, GameObject infoCard)
        {
            roomCard.SetActive(false);
            infoCard.SetActive(true);
        }

        private void OnInfoCardBackButtonClicked(GameObject roomCard, GameObject infoCard)
        {
            roomCard.SetActive(true);
            infoCard.SetActive(false);
        }

        private void OnShopButtonClicked()
        {
            shopButtonClickedSignal.Dispatch();
        }

        private void OnBackButtonClicked()
        {
            backButtonClickedSignal.Dispatch();
        }
    }
}
