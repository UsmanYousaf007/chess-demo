using UnityEngine;
using UnityEngine.UI;
using System;
using TurboLabz.InstantGame;

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

        static bool stringsLoaded = false;
        static string strWaiting = "";
        static string strDeclined = "";
        static string strTheirMove = "";
        static string strYouWon = "";
        static string strYouLost = "";
        static string strDraw = "";

        [HideInInspector] public LongPlayStatus longPlayStatus;
        [HideInInspector] public bool isCommunity;

        public void UpdateStatus()
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

            // Now enable required ones
            switch (longPlayStatus)
            {
                case LongPlayStatus.DEFAULT:
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.NEW_CHALLENGE:
                    notNowButton.gameObject.SetActive(true);
                    acceptButton.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.WAITING_FOR_ACCEPT:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strWaiting;
                    cancelButton.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.DECLINED:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDeclined;
                    okButton.gameObject.SetActive(true);
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
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.OPPONENT_WON:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strYouLost;
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;

                case LongPlayStatus.DRAW:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDraw;
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    break;



            }

            /*
            friendBar.statusLabel.gameObject.SetActive(true);
            friendBar.statusLabel.color = Colors.DULL_WHITE;

            // Update status
            if (friendBar.longPlayStatus == LongPlayStatus.DEFAULT)
            {
                friendBar.statusLabel.gameObject.SetActive(false);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.NEW_CHALLENGE)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_CHALLENGED_YOU);
                friendBar.statusLabel.color = Colors.YELLOW;
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_TURN)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
                friendBar.statusLabel.color = Colors.GREEN;
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.OPPONENT_TURN)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_THEIR_TURN);
                friendBar.statusLabel.color = Colors.WHITE;
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.PLAYER_WON)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_WON);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.OPPONENT_WON)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_LOST);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.DRAW)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DRAW);
            }
            else if (friendBar.longPlayStatus == LongPlayStatus.DECLINED)
            {
                friendBar.statusLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINED);
            }

            // Update timers
            if (friendBar.longPlayStatus != LongPlayStatus.NEW_CHALLENGE &&
                friendBar.longPlayStatus != LongPlayStatus.PLAYER_TURN &&
                friendBar.longPlayStatus != LongPlayStatus.OPPONENT_TURN)
            {
                friendBar.timer.gameObject.SetActive(false);
                friendBar.timerLabel.gameObject.SetActive(false);
                return;
            }

            friendBar.timer.gameObject.SetActive(true);
            friendBar.timerLabel.gameObject.SetActive(true);

            TimeSpan elapsedTime = DateTime.UtcNow.Subtract(friendBar.lastActionTime);

            if (elapsedTime.TotalHours < 1)
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_MINUTES,
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalMinutes)));
            }
            else if (elapsedTime.TotalDays < 1)
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_HOURS,
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalHours)));
            }
            else
            {
                friendBar.timerLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_DAYS,
                    Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalDays)));
            }
            */
        }

        public void Init(ILocalizationService localizationService)
        {
            // Set localized text
            if (stringsLoaded)
                return;
                
            notNowButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_NOT_NOW);
            acceptButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT);
            cancelButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_CANCEL);
            yourMoveStatus.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            strWaiting = localizationService.Get(LocalizationKey.LONG_PLAY_WAITING);
            strDeclined = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINED);
            strTheirMove = localizationService.Get(LocalizationKey.LONG_PLAY_THEIR_TURN);
            strYouWon = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            strYouLost = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_LOST);
            strDraw = localizationService.Get(LocalizationKey.LONG_PLAY_DRAW);

            stringsLoaded = true;
        }
    }
}