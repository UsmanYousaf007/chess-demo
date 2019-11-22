using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework 
{
    public class ProfileDialogView : View
    {
        [Header("Player Info")]
        public Image playerProfilePic;
        public Image playerAvatarBg;
        public Image playerAvatarIcon;
        public Text playerProfileName;
        public Text playerEloLabel;
        public Image playerFlag;

        [Header("Opponent Info")]
        public Image oppProfilePic;
        public Image opponentAvatarBG;
        public Image opponentAvatarIcon;
        public Text oppProfileName;
        public Text oppEloLabel;
        public Image oppFlag;
        public Text oppPlayingSinceDate;
        public Text oppLastSeen;
        public Text oppCountry;
        public Text oppWinPercentage;

        [Header("Confirm Dialog")]
        public GameObject confirmDialog;
        public Text confirmLabel;
        public Text yesLabel;
        public Text noLabel;
        public Button yesBtn;
        public Button noBtn;

        [Header("Stats")]
        public Text rankedTitle;
        public Text playerRankedWinsLabel;
        public Text opponentRankedWinsLabel;

        [Header("Others")]
        public Text vsLabel;
        public Sprite defaultAvatar;
        public Text blockLabel;
        public Button blockBtn;
        public Image blockUnderline;
        public Button closeBtn;
        public Button addFriendBtn;
        public Text addFriendBtnText;
        public Button removeFriendBtn;
        public Text removeFriendBtnText;
        public Text friendedText;
        public GameObject thinking;
        public Text chatLabel;
        public Button chatBtn;

        [Header("Alert Dialog")]
        public GameObject alertDialog;
        public Text alertDialogTitle;
        public Text alertDialogLabel;
        public Text okButtonText;
        public Button okBtn;

        [Header("Online Status")]
        public Image onlineStatus;
        public Sprite online;
        public Sprite offline;
        public Sprite active;

        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ISettingsModel settingsModel { get; set; }



        public Signal<string> blockUserSignal = new Signal<string>();
        public Signal<string> addFriendSignal = new Signal<string>();
        public Signal<string> friendRemoveSignal = new Signal<string>();
        public Signal<string> chatSignal = new Signal<string>();

        string eloPrefix = null;
        string totalGamesPrefix = null;
        string playerId = null;

        private SpritesContainer defaultAvatarContainer;

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            eloPrefix = localizationService.Get(LocalizationKey.ELO_SCORE);
            totalGamesPrefix = localizationService.Get(LocalizationKey.FRIENDS_TOTAL_GAMES_LABEL);
            confirmLabel.text = localizationService.Get(LocalizationKey.FRIENDS_CONFIRM_LABEL);
            yesLabel.text = localizationService.Get(LocalizationKey.FRIENDS_YES_LABEL);
            noLabel.text = localizationService.Get(LocalizationKey.FRIENDS_NO_LABEL);
            rankedTitle.text = localizationService.Get(LocalizationKey.FRIENDS_WINS_LABEL);
            vsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_VS_LABEL);
            blockLabel.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_LABEL);
            chatLabel.text = localizationService.Get(LocalizationKey.FRIENDS_CHAT_LABEL);
            addFriendBtnText.text = localizationService.Get(LocalizationKey.FRIENDS_ADD_TO_FRIENDS);
            removeFriendBtnText.text = localizationService.Get(LocalizationKey.FRIENDS_REMOVE_FROM_FRIENDS);
            friendedText.text = localizationService.Get(LocalizationKey.FRIENDS_TEXT_FRIENDED);
            playerProfilePic.sprite = defaultAvatar;
            oppProfilePic.sprite = defaultAvatar;
            alertDialogTitle.text = "Limit Reached";

            blockBtn.onClick.AddListener(OnBlockConfirm);
            chatBtn.onClick.AddListener(OnChat);
            noBtn.onClick.AddListener(OnConfirmNo);
            yesBtn.onClick.AddListener(() => OnBlock(playerId));
            addFriendBtn.onClick.AddListener(OnAddFriendClicked);
            removeFriendBtn.onClick.AddListener(OnRemoveFriendClicked);

            okBtn.onClick.AddListener(OnAlertDialogOkButtonClicked);
            alertDialog.SetActive(false);

        }

        public void UpdateProfileDialog(ProfileDialogVO vo)
        {
            alertDialogLabel.text = "Max add friend limit reached = " + settingsModel.maxFriendsCount;
            playerAvatarBg.gameObject.SetActive(false);
            playerAvatarIcon.gameObject.SetActive(false);
            alertDialog.SetActive(false);

            playerId = vo.playerId;
            if (vo.playerProfilePic!= null)
            {
                playerProfilePic.sprite = vo.playerProfilePic;
            }
            else
            {
                if (vo.playerAvatarId != null)
                {
                    playerAvatarBg.gameObject.SetActive(true);
                    playerAvatarIcon.gameObject.SetActive(true);

                    playerAvatarBg.color = Colors.Color(vo.playerAvatarColor);
                    playerAvatarIcon.sprite = defaultAvatarContainer.GetSprite(vo.playerAvatarId);
                }
                else
                {
                    playerProfilePic.sprite = defaultAvatar;
                }
            }
            
            playerProfileName.text = vo.playerProfileName;
            playerEloLabel.text = eloPrefix + " " + vo.playerElo;
            playerFlag.sprite = Flags.GetFlag(vo.playerCountryCode);


            opponentAvatarBG.gameObject.SetActive(false);
            opponentAvatarIcon.gameObject.SetActive(false);
            if (vo.oppProfilePic != null)
            {
                oppProfilePic.sprite = vo.oppProfilePic;
            }
            else
            {
                if (vo.oppAvatarId != null)
                {
                    opponentAvatarBG.gameObject.SetActive(true);
                    opponentAvatarIcon.gameObject.SetActive(true);

                    opponentAvatarBG.color = Colors.Color(vo.oppAvatarColor);
                    opponentAvatarIcon.sprite = defaultAvatarContainer.GetSprite(vo.oppAvatarId);
                }
                else
                {
                    oppProfilePic.sprite = defaultAvatar;
                }
            }

            
            oppProfileName.text = vo.oppProfileName;
            oppEloLabel.text = eloPrefix + " " + vo.oppElo;
            oppFlag.sprite = Flags.GetFlag(vo.oppCountryCode);
            oppPlayingSinceDate.text = "Playing since, " + vo.oppPlayingSinceDate;
            oppLastSeen.text = "Last played, " + vo.oppLastSeen;

            float total = vo.oppTotalGamesWon + vo.oppTotalGamesLost;
            float percentage = total > 0.0f ? (vo.oppTotalGamesWon / total) * 100.0f : 0.0f;
            oppWinPercentage.text = "Win Percentage: " + percentage.ToString("0.") + "%";

            playerRankedWinsLabel.text = vo.playerWinsCount.ToString();
            opponentRankedWinsLabel.text = vo.opponentWinsCount.ToString();
            oppCountry.text = Flags.GetCountry(vo.oppCountryCode);

            blockBtn.interactable = !vo.isCommunity;
            blockLabel.color = vo.isCommunity ? Colors.DISABLED_WHITE : Colors.WHITE;
            blockUnderline.color = vo.isCommunity ? Colors.DISABLED_WHITE : Colors.WHITE;

            if (vo.friendType == GSBackendKeys.Friend.TYPE_COMMUNITY)
            {
                ShowFriended(false);
            }
            else
            {
                ShowFriended(!vo.isCommunity);
            }

            if (vo.longPlayStatus != LongPlayStatus.DEFAULT || vo.friendType == GSBackendKeys.Friend.TYPE_SOCIAL)
            {
                EnableAddButton(false);
                EnableRemoveButton(false);
            }

            if (!vo.oppOnline && vo.oppActive)
            {
                onlineStatus.sprite = active;
            }
            else
            {
                onlineStatus.sprite = vo.oppOnline ? online : offline;
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            confirmDialog.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnConfirmNo()
        {
            confirmDialog.SetActive(false);
        }

        private void OnBlockConfirm()
        {
            confirmDialog.SetActive(true);
        }

        private void OnChat()
        {
            chatSignal.Dispatch(playerId);
        }

        private void OnBlock(string blockPlayerId)
        {
            blockUserSignal.Dispatch(blockPlayerId);
        }

        private void TrigerAddFriendSignal()
        {
            addFriendSignal.Dispatch(playerId);
            ShowFriended(true);
        }

        private void TrigerRemoveFriendSignal()
        {
            friendRemoveSignal.Dispatch(playerId);
            ShowFriended(false);
        }

        private void OnAddFriendClicked()
        {
            
            if(playerModel.playerFriendsCount >= settingsModel.maxFriendsCount)
            {
                alertDialog.SetActive(true);
            }
            else
            {
                thinking.gameObject.SetActive(true);
                Invoke("TrigerAddFriendSignal", 2.0f);

                EnableAddButton(false);

            }
        }

        private void OnRemoveFriendClicked()
        {
            thinking.gameObject.SetActive(true);
            friendedText.gameObject.SetActive(false);
            Invoke("TrigerRemoveFriendSignal", 2.0f);

            EnableRemoveButton(false);
        }

        public void ShowFriended(bool flag)
        {
            thinking.gameObject.SetActive(false);
            EnableAddButton(true);
            EnableRemoveButton(true);
            friendedText.gameObject.SetActive(flag);
            removeFriendBtn.gameObject.SetActive(flag);
            addFriendBtn.gameObject.SetActive(!flag);
        }

        private void EnableRemoveButton(bool enableFlag)
        {
            if (enableFlag)
            {
                removeFriendBtn.interactable = true;
                removeFriendBtnText.color = Colors.WHITE;
            }
            else
            {
                removeFriendBtn.interactable = false;
                removeFriendBtnText.color = Colors.DISABLED_WHITE;

            }
        }

        private void EnableAddButton(bool enableFlag)
        {
            if(enableFlag)
            {
                addFriendBtn.interactable = true;
                addFriendBtnText.color = Colors.ColorAlpha(addFriendBtnText.color, 0.9f);
            }
            else
            {
                addFriendBtn.interactable = false;
                addFriendBtnText.color = Colors.ColorAlpha(addFriendBtnText.color, 0.5f);

            }
        }

        private void OnAlertDialogOkButtonClicked()
        {
            alertDialog.SetActive(false);
        }

    }
}