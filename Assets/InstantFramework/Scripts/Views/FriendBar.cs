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
        public Image avatarBG;
        public Image avatarIcon;
        public Text profileNameLabel;
        public Text eloScoreLabel;
        public GameObject timer;
        public Text timerLabel;
        public Text generalStatus;
        public Text yourMoveStatus;
        public Text matchStatus;
        public Button viewProfileButton;
        public Button stripButton;
        public Friend friendInfo;
        public DateTime lastActionTime;
        public GameObject thinking;
        public Image onlineStatus;
        public Sprite online;
        public Sprite offline;
        public Sprite activeStatus;
        public GameObject unreadChat;
        public bool isOnline;
        public bool isActive;
        public GameObject playArrow;
        public Text playButtonLabel;
        public Button notNowButton;
        public Text notNowButtonLabel;
        public Button acceptButton;
        public Text acceptButtonLabel;
        public Button cancelButton;
        public Text cancelButtonLabel;
        public Button okButton;
        public Text okButtonLabel;
        public Button viewButton;
        public Text viewButtonLabel;
        public Button removeCommunityFriendButton;
        public Text newMatchGreetingLabel;
        public GameObject newMatchGreeting;
        public GameObject rankedIcon;
        public GameObject friendlyIcon;

        bool stringsLoaded = false;
        static string strWaiting = "";
        static string strDeclined = "";
        static string strTheirMove = "";
        static string strYouWon = "";
        static string strYouLost = "";
        static string strDraw = "";
        static string strCanceled = "";
        static string strYourMove = "";
        static string strNewMatchGreeting = "";
        static string strDeclineApology = "";

        [HideInInspector] public LongPlayStatus longPlayStatus;
        [HideInInspector] public bool isCommunity;
        [HideInInspector] public bool isSearched;
        [HideInInspector] public bool isCommunityFriend;
        [HideInInspector] public bool isGameCanceled;
        [HideInInspector] public bool isPlayerTurn;
        [HideInInspector] public bool isRanked;

        [HideInInspector] public bool isFriendView;
        [HideInInspector] public long lastMatchTimeStamp;
        [Header("Friends Bar Optimization")]
        public GameObject bottomAlphaBg;
        public Mask maskObject;

        public void UpdateMasking(bool isLastCell, bool isLastSection)
        {
            bottomAlphaBg.SetActive(false);
            maskObject.enabled = false;
            if (!isLastSection && isLastCell)
            {
                bottomAlphaBg.SetActive(true);
            }
            if (isLastCell)
            {
                maskObject.enabled = true;
            }
        }

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
                    newMatchGreeting.gameObject.SetActive(true);
                    newMatchGreetingLabel.text = strNewMatchGreeting;
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    break;

                case LongPlayStatus.WAITING_FOR_ACCEPT:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strWaiting;
                    cancelButton.gameObject.SetActive(true);
                    cancelButton.interactable = true;
                    timerLabel.gameObject.SetActive(true);
                    stripButton.gameObject.SetActive(true);
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    if (isPlayerTurn)
                    {
                        generalStatus.text = strYourMove;
                    }
                    break;

                case LongPlayStatus.PLAYER_TURN:
                    yourMoveStatus.gameObject.SetActive(true);
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    break;

                case LongPlayStatus.OPPONENT_TURN:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strTheirMove;
                    stripButton.gameObject.SetActive(true);
                    playArrow.gameObject.SetActive(true);
                    timerLabel.gameObject.SetActive(true);
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    break;

                case LongPlayStatus.PLAYER_WON:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = isGameCanceled ? strCanceled: strYouWon;
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    if (isGameCanceled)
                    {
                        okButton.gameObject.SetActive(true);
                        okButton.interactable = true;
                    }
                    else
                    {
                        viewButton.gameObject.SetActive(true);
                        viewButton.interactable = true;
                    }

                    break;

                case LongPlayStatus.OPPONENT_WON:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = isGameCanceled ? strCanceled : strYouLost;
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    if (isGameCanceled)
                    {
                        okButton.gameObject.SetActive(true);
                        okButton.interactable = true;
                    }
                    else
                    {
                        viewButton.gameObject.SetActive(true);
                        viewButton.interactable = true;
                    }

                    break;

                case LongPlayStatus.DRAW:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDraw;
                    rankedIcon.SetActive(isRanked);
                    friendlyIcon.SetActive(!isRanked);

                    if (isGameCanceled)
                    {
                        okButton.gameObject.SetActive(true);
                        okButton.interactable = true;
                    }
                    else
                    {
                        viewButton.gameObject.SetActive(true);
                        viewButton.interactable = true;
                    }

                    break;

                case LongPlayStatus.DECLINED:
                    generalStatus.gameObject.SetActive(true);
                    generalStatus.text = strDeclined;
                    okButton.gameObject.SetActive(true);
                    okButton.interactable = true;
                    newMatchGreeting.gameObject.SetActive(true);
                    newMatchGreetingLabel.text = strDeclineApology;
                    break;
            }

            UpdateFriendViewStatus();
        }

        public void UpdateFriendViewStatus()
        {
            if (longPlayStatus != LongPlayStatus.DEFAULT && isFriendView)
            {
                timerLabel.gameObject.SetActive(false);
                newMatchGreeting.gameObject.SetActive(false);
                newMatchGreetingLabel.gameObject.SetActive(false);
                rankedIcon.SetActive(isRanked);
                friendlyIcon.SetActive(!isRanked);
                generalStatus.gameObject.SetActive(false);
                stripButton.gameObject.SetActive(false);
                yourMoveStatus.gameObject.SetActive(false);
                notNowButton.gameObject.SetActive(false);
                acceptButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                thinking.gameObject.SetActive(false);
                playArrow.gameObject.SetActive(false);
                okButton.gameObject.SetActive(false);
                removeCommunityFriendButton.gameObject.SetActive(false);
                viewButton.gameObject.SetActive(false);
                matchStatus.gameObject.SetActive(true);
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
            newMatchGreeting.gameObject.SetActive(false);
            rankedIcon.SetActive(false);
            friendlyIcon.SetActive(false);
            viewButton.gameObject.SetActive(false);
        }

        public void Init(ILocalizationService localizationService)
        {
            // Set localized text
            if (stringsLoaded)
                return;
                
            notNowButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_NOT_NOW);
            playButtonLabel.text = localizationService.Get(LocalizationKey.PLAY);
            acceptButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_ACCEPT);
            cancelButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_CANCEL);
            okButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_OK);
            yourMoveStatus.text = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            matchStatus.text = localizationService.Get(LocalizationKey.LONG_PLAY_MATCH_PROGRESS);
            strWaiting = localizationService.Get(LocalizationKey.LONG_PLAY_WAITING);
            strDeclined = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINED);
            strTheirMove = localizationService.Get(LocalizationKey.LONG_PLAY_THEIR_TURN);
            strYouWon = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_WON);
            strYouLost = localizationService.Get(LocalizationKey.LONG_PLAY_YOU_LOST);
            strDraw = localizationService.Get(LocalizationKey.LONG_PLAY_DRAW);
            strCanceled = localizationService.Get(LocalizationKey.LONG_PLAY_CANCELED);
            strYourMove = localizationService.Get(LocalizationKey.LONG_PLAY_YOUR_TURN);
            viewButtonLabel.text = localizationService.Get(LocalizationKey.LONG_PLAY_VIEW);
            strNewMatchGreeting = localizationService.Get(LocalizationKey.LONG_PLAY_NEW_MATCH_GREETING);
            strDeclineApology = localizationService.Get(LocalizationKey.LONG_PLAY_DECLINE_APOLOGY);

            stringsLoaded = true;
        }

        public int solution(int N)
        {
            if (N <= 0)
            {
                return 0;
            }
            string binary = Convert.ToString(N, 2);
            int maxCount = 0;
            int count = 0;
            int lastIndex = binary.LastIndexOf('1');
            int firstIndex = binary.IndexOf('1');
            for (int i = firstIndex; i < lastIndex; i++)
            {
                if (i == '0')
                {
                    count++;
                }
                else
                {
                    if (count>maxCount)
                    {
                        maxCount = count;
                    }
                    count = 0;
                }
            }
            return maxCount;
        }

        


    }


    
}
