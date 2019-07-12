using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;
using DG.Tweening;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ShareDialogView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

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

        public void UpdateShareDialog(Sprite sprite)
        {
            if (sprite != null)
            {
                shareImage.sprite = sprite;
            }
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
