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
        public Text winsTitle;
        public Text drawsTitle;
        public Text playerWinsLabel;
        public Text playerDrawsLabel;
        public Text opponentWinsLabel;
        public Text opponentDrawsLabel;
        public Text totalGamesLabel;

        [Header("Others")]
        public Text vsLabel;
        public Sprite defaultAvatar;
        public Text blockLabel;
        public Button blockBtn;
        public Button closeBtn;
        public Button addFriendBtn;
        public Button removeFriendBtn;
        public Text friendedText;

        [Inject] public ILocalizationService localizationService { get; set; }

        public Signal<string> blockUserSignal = new Signal<string>();
        public Signal<string> addFriendSignal = new Signal<string>();
        public Signal<string> friendRemoveSignal = new Signal<string>();

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
            winsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_WINS_LABEL);
            drawsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_DRAWS_LABEL);
            vsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_VS_LABEL);
            blockLabel.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_LABEL);
            friendedText.text = "Friended";
            playerProfilePic.sprite = defaultAvatar;
            oppProfilePic.sprite = defaultAvatar;
           

            blockBtn.onClick.AddListener(OnBlockConfirm);
            noBtn.onClick.AddListener(OnConfirmNo);
            yesBtn.onClick.AddListener(() => OnBlock(playerId));
            addFriendBtn.onClick.AddListener(OnAddFriendClicked);
            removeFriendBtn.onClick.AddListener(OnRemoveFriendClicked);
            
        }

        public void UpdateProfileDialog(ProfileDialogVO vo)
        {
            playerAvatarBg.gameObject.SetActive(false);
            playerAvatarIcon.gameObject.SetActive(false);

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
            oppPlayingSinceDate.text = "Playing since " + vo.oppPlayingSinceDate;
            oppLastSeen.text = "Last played " + vo.oppLastSeen;

            float total = vo.oppTotalGamesWon + vo.oppTotalGamesLost;
            float percentage = total > 0.0f ? (vo.oppTotalGamesWon / total) * 100.0f : 0.0f;
            oppWinPercentage.text = "Win Percentage: " + percentage.ToString("0.") + "%";

            playerWinsLabel.text = vo.playerWinsCount.ToString();
            playerDrawsLabel.text = vo.playerDrawsCount.ToString();
            opponentWinsLabel.text = vo.opponentWinsCount.ToString();
            opponentDrawsLabel.text = vo.opponentDrawsCount.ToString();
            totalGamesLabel.text = totalGamesPrefix + vo.totalGamesCount;
            oppCountry.text = Flags.GetCountry(vo.oppCountryCode);

            blockBtn.gameObject.SetActive(!vo.isCommunity);
            ShowFriended(!vo.isCommunity);
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

        private void OnBlock(string blockPlayerId)
        {
            blockUserSignal.Dispatch(blockPlayerId);
        }

        private void OnAddFriendClicked()
        {
            addFriendSignal.Dispatch(playerId);
            ShowFriended(true);
        }

        private void OnRemoveFriendClicked()
        {
            friendRemoveSignal.Dispatch(playerId);
            ShowFriended(false);
        }

        public void ShowFriended(bool flag)
        {
            if (flag)
            {
                friendedText.gameObject.SetActive(true);
                removeFriendBtn.gameObject.SetActive(true);
                addFriendBtn.gameObject.SetActive(false);
            }
            else
            {
                friendedText.gameObject.SetActive(false);
                removeFriendBtn.gameObject.SetActive(false);
                addFriendBtn.gameObject.SetActive(true);
            }

        }

    }
}