using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework 
{
    public class ProfileDialogView : View
    {
        [Header("Player Info")]
        public Image playerProfilePic;
        public Text playerProfileName;
        public Text playerEloLabel;
        public Image playerFlag;

        [Header("Opponent Info")]
        public Image oppProfilePic;
        public Text oppProfileName;
        public Text oppEloLabel;
        public Image oppFlag;

        [Header("Confirm Dialog")]
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

        [Inject] public ILocalizationService localizationService { get; set; }

        string eloPrefix = null;
        string totalGamesPrefix = null;

        public void Init()
        {
            eloPrefix = localizationService.Get(LocalizationKey.ELO_SCORE);
            totalGamesPrefix = localizationService.Get(LocalizationKey.FRIENDS_TOTAL_GAMES_LABEL);
            confirmLabel.text = localizationService.Get(LocalizationKey.FRIENDS_CONFIRM_LABEL);
            yesLabel.text = localizationService.Get(LocalizationKey.FRIENDS_YES_LABEL);
            noLabel.text = localizationService.Get(LocalizationKey.FRIENDS_NO_LABEL);
            winsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_WINS_LABEL);
            drawsTitle.text = localizationService.Get(LocalizationKey.FRIENDS_DRAWS_LABEL);
            vsLabel.text = localizationService.Get(LocalizationKey.FRIENDS_VS_LABEL);
            blockLabel.text = localizationService.Get(LocalizationKey.FRIENDS_BLOCK_LABEL);
            playerProfilePic.sprite = defaultAvatar;
            oppProfilePic.sprite = defaultAvatar;
        }

        public void UpdateProfileDialog(ProfileDialogVO vo)
        {
            playerProfilePic.sprite = vo.playerProfilePic;
            playerProfileName.text = vo.playerProfileName;
            playerEloLabel.text = eloPrefix + " " + vo.playerElo;
            playerFlag.sprite = Flags.GetFlag(vo.playerCountryCode);

            oppProfilePic.sprite = vo.oppProfilePic;
            oppProfileName.text = vo.oppProfileName;
            oppEloLabel.text = eloPrefix + " " + vo.oppElo;
            oppFlag.sprite = Flags.GetFlag(vo.oppCountryCode);

            playerWinsLabel.text = vo.playerWinsCount.ToString();
            playerDrawsLabel.text = vo.playerDrawsCount.ToString();
            opponentWinsLabel.text = vo.opponentWinsCount.ToString();
            opponentDrawsLabel.text = vo.opponentDrawsCount.ToString();
            totalGamesLabel.text = totalGamesPrefix + vo.totalGamesCount;

            blockBtn.gameObject.SetActive(!vo.isCommunity);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}