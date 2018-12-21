using UnityEngine;
using UnityEngine.UI;
using System;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class FriendBar : MonoBehaviour
    {
        public Image avatarImage;
        public Text profileNameLabel;
        public Text eloScoreLabel;
        public GameObject timer;
        public Text timerLabel;
        public Text generalStatus;
        public Text yourMoveStatus;
        public Button viewProfileButton;
        public Button stripButton;
        public Friend friendInfo;
        public DateTime lastActionTime;
        public GameObject thinking;
        public Image onlineStatus;
        public Sprite online;
        public Sprite offline;
        public GameObject unreadChat;
        public bool isOnline;
        public GameObject playArrow;
        public Button notNowButton;
        public Text notNowButtonLabel;
        public Button acceptButton;
        public Text acceptButtonLabel;
        public Button cancelButton;
        public Text cancelButtonLabel;
        public Button okButton;
        public Text okButtonLabel;
        public Button removeCommunityFriendButton;

        bool stringsLoaded = false;
        static string strWaiting = "";
        static string strDeclined = "";
        static string strTheirMove = "";
        static string strYouWon = "";
        static string strYouLost = "";
        static string strDraw = "";

        [HideInInspector] public LongPlayStatus longPlayStatus;
        [HideInInspector] public bool isCommunity;
        [HideInInspector] public bool isCommunityFriend;

        public void UpdateStatus()
        {
            DisableOptionalElements();

            // Now enable required ones
            switch (longPlayStatus)
            {
                case LongPlayStatus.DEFAULT:
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);

                    if (isCommunityFriend)
                    {
                        removeCommunityFriendButton.gameObject.SetActive(true);
                    }
                    break;

                case LongPlayStatus.NEW_CHALLENGE:
                    notNowButton.gameObject.SetActive(true);
                    notNowButton.interactable = true;
                    acceptButton.gameObject.SetActive(true);
                    acceptButton.interactable = true;
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.WAITING_FOR_ACCEPT:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strWaiting;
                    cancelButton.gameObject.SetActive(true);
                    cancelButton.interactable = true;
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.PLAYER_TURN:
                    yourMoveStatus.gameObject.SetActive(true);
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.OPPONENT_TURN:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strTheirMove;
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.PLAYER_WON:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strYouWon;
                    okButton.gameObject.SetActive(true);
                    okButton.interactable = true;
                    break;

                case LongPlayStatus.OPPONENT_WON:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strYouLost;
                    okButton.gameObject.SetActive(true);
                    okButton.interactable = true;
                    break;

                case LongPlayStatus.DRAW:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDraw;
                    okButton.gameObject.SetActive(true);
                    okButton.interactable = true;
                    break;

                case LongPlayStatus.DECLINED:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDeclined;
                    okButton.gameObject.SetActive(true);
                    okButton.interactable = true;
                    break;
            }
        }

        public void UpdateCommmunityStrip()
        {
            DisableOptionalElements();
            stripButton.gameObject.SetActive(true);
            playArrow.gameObject.SetActive(true);
        }

        private void DisableOptionalElements()
        {
            // Hide all optional elements (except unread indicator, which is controlled 
            // by another pipeline)
            stripButton.gameObject.SetActive(false);
            generalStatus.gameObject.SetActive(false);
            yourMoveStatus.gameObject.SetActive(false);
            notNowButton.gameObject.SetActive(false);
            acceptButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            thinking.gameObject.SetActive(false);
            playArrow.gameObject.SetActive(false);
            timerLabel.gameObject.SetActive(false);
            okButton.gameObject.SetActive(false);
            removeCommunityFriendButton.gameObject.SetActive(false);
        }

        public void Init(ILocalizationService localizationService)
        {
            // Set localized text
            if (stringsLoaded)
                return;
                
            notNowButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_NOT_NOW);
            acceptButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT);
            cancelButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_CANCEL);
            okButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_OK);
            yourMoveStatus.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            strWaiting = localizationService.Get(LocalizationKey.LONG_PLAY_WAITING);
            strDeclined = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINED);
            strTheirMove = localizationService.Get(LocalizationKey.LONG_PLAY_THEIR_TURN);
            strYouWon = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_WON);
            strYouLost = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_LOST);
            strDraw = localizationService.Get(LocalizationKey.LONG_PLAY_DRAW);

            stringsLoaded = true;
        }
    }
}