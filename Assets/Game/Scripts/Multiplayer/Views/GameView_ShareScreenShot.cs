using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using DG.Tweening;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Share Screen Shot")]
        public Text shareTitleText;
        public Image shareImage;
        public Text shareButtonText;
        public Button shareCloseButton;
        public Button shareButton;
        public GameObject shareConfirmDlg;

        public void InitShareScreen()
        {
            shareTitleText.text = localizationService.Get(LocalizationKey.SHARE_GAME_SCREENSHOT);
            shareButtonText.text = localizationService.Get(LocalizationKey.SHARE);
            shareButton.onClick.AddListener(ShareCloseButtonClicked);
            shareButton.onClick.AddListener(ShareButtonClicked);
        }

        public void ShowShareDialog()
        {
            shareConfirmDlg.SetActive(true);
        }

        public void ShareCloseButtonClicked()
        {
            shareConfirmDlg.SetActive(false);
        }

        public void ShareButtonClicked()
        {
            shareConfirmDlg.SetActive(false);
        }

        public void SetShareScreenSprite(Sprite sprite)
        {
            shareImage.sprite = sprite;
        }
    }

}
