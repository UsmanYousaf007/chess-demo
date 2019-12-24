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
        public Image addFriendBtnUnderline;
        public Image addFriendIcon;
        public Image plusIcon;
        public Button removeFriendBtn;
        public Text removeFriendBtnText;
        public Image removeFriendIcon;
        public Image minusIcon;
        public Image removeFriendBtnUnderline;
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
        bool inGame;
        
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
            alertDialogTitle.text = "Friends Limit Reached";
            alertDialogLabel.text = "Please remove unused friend strips to add new friends";

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
            playerAvatarBg.gameObject.SetActive(false);
            playerAvatarIcon.gameObject.SetActive(false);
            alertDialog.SetActive(false);

            inGame = vo.inGame;
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
            oppWinPercentage.text = "Wins " + percentage.ToString("0.") + "%";

            playerRankedWinsLabel.text = vo.playerWinsCount.ToString();
            opponentRankedWinsLabel.text = vo.opponentWinsCount.ToString();
            oppCountry.text = Flags.GetCountry(vo.oppCountryCode);

            EnableBlockButton(!vo.isCommunity);

            if (vo.friendType == GSBackendKeys.Friend.TYPE_COMMUNITY)
            {
                ShowFriended(false);
            }
            else
            {
                ShowFriended(!vo.isCommunity);
            }

            if (vo.longPlayStatus != LongPlayStatus.DEFAULT)
            {
                EnableRemoveButton(false);
                EnableBlockButton(false);
                inGame = true;
            }

            if (vo.friendType == GSBackendKeys.Friend.TYPE_SOCIAL)
            {
                EnableAddButton(false);
            }

            if (!vo.oppOnline && vo.oppActive)
            {
                onlineStatus.sprite = active;
            }
            else
            {
                onlineStatus.sprite = vo.oppOnline ? online : offline;
            }

            if (vo.inGame)
            {
                blockBtn.interactable = false;
                blockLabel.color = Colors.DISABLED_WHITE;
                blockUnderline.color = Colors.DISABLED_WHITE;

                if (vo.friendType == null)
                {
                    ShowFriended(false);
                }
                else
                {
                    EnableRemoveButton(false);
                }

                if (vo.isBot)
                {
                    EnableAddButton(false);
                    onlineStatus.sprite = online;
                }
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
            EnableRemoveButton(!inGame);
            friendedText.gameObject.SetActive(flag);
            removeFriendBtn.gameObject.SetActive(flag);
            addFriendBtn.gameObject.SetActive(!flag);
        }

        private void EnableRemoveButton(bool enableFlag)
        {
            removeFriendBtn.interactable = enableFlag;
            var colorToSet = enableFlag ? Colors.WHITE_150 : Colors.DISABLED_WHITE;
            removeFriendBtnText.color = colorToSet;
            removeFriendBtnUnderline.color = colorToSet;
            removeFriendIcon.color = colorToSet;
            minusIcon.color = colorToSet;
        }

        private void EnableAddButton(bool enableFlag)
        {
            addFriendBtn.interactable = enableFlag;
            var colorToSet = enableFlag ? Colors.WHITE_150 : Colors.DISABLED_WHITE;
            addFriendBtnText.color = colorToSet;
            addFriendBtnUnderline.color = colorToSet;
            addFriendIcon.color = colorToSet;
            plusIcon.color = colorToSet;
        }

        private void OnAlertDialogOkButtonClicked()
        {
            alertDialog.SetActive(false);
        }

        private void EnableBlockButton(bool enableFlag)
        {
            blockBtn.interactable = enableFlag;
            var colorToSet = enableFlag ? Colors.WHITE_150 : Colors.DISABLED_WHITE;
            blockLabel.color = colorToSet;
            blockUnderline.color = colorToSet;
        }

        public void SetProfilePic(Sprite sprite, string playerId = null)
        {
            if (playerId == null || playerId != this.playerId)
            {
                return;
            }

            if (sprite != null)
            {
                opponentAvatarIcon.gameObject.SetActive(false);
                opponentAvatarBG.gameObject.SetActive(false);
                oppProfilePic.sprite = sprite;
            }
        }
    }
}