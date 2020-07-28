using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;
using strange.extensions.signal.impl;
using TurboLabz.InstantGame;
using TMPro;

namespace TurboLabz.InstantFramework
{

    public class OpponentStatusDialogueView : View
    {
        public Button closeBtn;
        public TextMeshProUGUI opponentStatus;
        public TextMeshProUGUI tryAgainText;

        [Header("Opponent Info")]
        public Image oppProfilePic;
        public Image opponentAvatarBG;
        public Image opponentAvatarIcon;
        public Text oppProfileName;
        public Image oppFlag;
        public Text oppCountry;
        public GameObject premiumBorder;

        [Header("Online Status")]
        public Image onlineStatus;
        public Sprite online;
        public Sprite offline;
        public Sprite active;

        public Sprite defaultAvatar;
        private SpritesContainer defaultAvatarContainer;

        [Inject] public ILocalizationService localizationService { get; set; }

        public void Init()
        {
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            tryAgainText.text = localizationService.Get(LocalizationKey.TRY_AGAIN_LATER_TEXT);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        void OnCancelClicked()
        {

        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateDialog(ProfileVO vo, string matchMode)
        {
            opponentAvatarBG.gameObject.SetActive(false);
            opponentAvatarIcon.gameObject.SetActive(false);
            premiumBorder.gameObject.SetActive(vo.isPremium);

            if (vo.playerPic != null)
            {
                oppProfilePic.sprite = vo.playerPic;
            }
            else
            {
                if (vo.avatarId != null)
                {
                    opponentAvatarBG.gameObject.SetActive(true);
                    opponentAvatarIcon.gameObject.SetActive(true);

                    opponentAvatarBG.color = Colors.Color(vo.avatarColorId);
                    opponentAvatarIcon.sprite = defaultAvatarContainer.GetSprite(vo.avatarId);
                }
                else
                {
                    oppProfilePic.sprite = defaultAvatar;
                }
            }

            oppProfileName.text = vo.playerName;
            oppFlag.sprite = Flags.GetFlag(vo.countryId);

            oppCountry.text = Flags.GetCountry(vo.countryId);

            if (!vo.isOnline && vo.isActive)
            {
                onlineStatus.sprite = active;
            }
            else
            {
                onlineStatus.sprite = vo.isOnline ? online : offline;
            }

            opponentStatus.text = "Already in a " + matchMode + " Game";

            /*if (vo.inGame)
            {
                if (vo.isBot)
                {
                    onlineStatus.sprite = online;
                }
            }*/
        }
    }
}
