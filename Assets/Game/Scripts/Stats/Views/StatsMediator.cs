/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections;

namespace TurboLabz.InstantGame
{
    public class StatsMediator : Mediator
    {
        // Dispatch signals
        [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public ShowShareScreenDialogSignal shareScreenDialogSignal { get; set; }
        [Inject] public UploadFileSignal uploadFileSignal { get; set; }
        // View injection
        [Inject] public StatsView view { get; set; }
        //Model injection
        [Inject] public IPlayerModel playerModel { get; set; }
        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IScreenCaptureService screenCaptureService { get; set; }
        [Inject] public IPhotoService photoPickerService { get; set; }
        [Inject] public IPicsModel picsModel { get; set; }

        

        public override void OnRegister()
        {
            view.Init();
            view.restorePurchasesSignal.AddListener(OnRestorePurchases);
            view.backButton.onClick.AddListener(OnBackButtonClicked);
            view.shareBtn.onClick.AddListener(OnShareScreenClicked);

            view.choosePhotoBtn.onClick.AddListener(OnChoosePhotoBtnClicked);
            view.takePhotoBtn.onClick.AddListener(OnTakePhotoBtnClicked);

            view.openPhotoSettingsBtn.onClick.AddListener(OnOpenSettingsBtnClicked);
            view.profilePicBtn.onClick.AddListener(OnUploadProfilPicBtnClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS) 
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.profile);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateStatsSignal))]
        public void OnUpdateStats(StatsVO vo)
        {
            view.UpdateView(vo);
        }

        void OnRestorePurchases()
        {
            restorePurchasesSignal.Dispatch();
        }

        void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }

        public void OnShareScreenClicked()
        {
            screenCaptureService.CaptureScreenShot(view.logo);
            OpenShareDialog();
        }

        public void OnShowShareDialogSignal()
        {
            shareScreenDialogSignal.Dispatch();
        }

        public void OpenShareDialog()
        {
            StartCoroutine(DispatchShareSignal());
        }

        public IEnumerator DispatchShareSignal()
        {
            yield return new WaitForSeconds(.25f);
            OnShowShareDialogSignal();
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessingUI(bool show, bool showProcessingUi)
        {
            view.ShowProcessing(show, showProcessingUi);
        }

        void OnTakePhotoBtnClicked()
        {
            analyticsService.Event(AnalyticsEventId.upload_picture, AnalyticsContext.take_new);
            if (photoPickerService.HasCameraPermission())
            {
                photoPickerService.TakePhoto(512, 512);
            }
            else
            {
                view.CloseProfilePicDialog();
                view.OpenSettingsDialog();
            }

        }

        void OnChoosePhotoBtnClicked()
        {
            analyticsService.Event(AnalyticsEventId.upload_picture, AnalyticsContext.choose_existing);
            if (photoPickerService.HasGalleryPermission())
            {
                photoPickerService.PickPhoto(512, 512);
            }
            else
            {
                view.CloseProfilePicDialog();
                view.OpenSettingsDialog();
            }
        }

        void OnUploadProfilPicBtnClicked()
        {
            analyticsService.Event(AnalyticsEventId.upload_picture, AnalyticsContext.dlg_shown);
            view.OpenProfilePicDialog();
        }

        void OnOpenSettingsBtnClicked()
        {
            view.CloseSettingsDialog();
            photoPickerService.OpenCameraSettings();
        }

        [ListensTo(typeof(PhotoPickerCompleteSignal))]
        public void OnPhotoPickerComplete (Photo photo)
        {
            view.CloseProfilePicDialog();
            view.ShowProcessing(true, true);

            if (photo != null)
            {
                picsModel.SetPlayerPic(playerModel.id, photo.sprite);
                var uploadFileVO = new UploadFileVO
                {
                    fileName = "profilePic",
                    stream = photo.stream,
                    mimeType = "image/png"
                };
                uploadFileSignal.Dispatch(uploadFileVO);
            }
        }
    }
}
