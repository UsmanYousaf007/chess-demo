/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System;
using System.Collections;
using DG.Tweening;

namespace TurboLabz.InstantGame
{
    [CLSCompliant(false)]
    public class ProfileView : View
    {
        [Header("Profile Section")]
        public Sprite defaultAvatar;
        public Sprite whiteAvatar;
        public Image profilePic;
        public Image avatarBG;
        public Image avatarIcon;
        public Text profileName;
        public GameObject noProfilePicBorder;
        public GameObject hasProfilePicBorder;
        public GameObject premiumBorder;
        public Image leagueBorder;
        public Text eloScoreLabel;
        public Text eloScoreValue;
        public Image playerFlag;
        private string playerId;
        private SpritesContainer defaultAvatarContainer;
        public RectTransform avatarContainer;
        Transform avatarContainerStartRef;
        public RectTransform avatarContainerRef;
        public Button profilePicButton;

        [Header("Tournaments Section")]
        public Button playTournamentButton;
        public Text playTournamentButtonLabel;
        public Text tournamentLiveLabel;
        public Image liveTournamentIcon;
        public GameObject liveTournamentGO;

        [Header("Social Connection Section")]
        public Button socialConnectionButton;
        public Text socialConnectionButtonLabel;
        public Image facebookIcon;
        public Image appleIcon;

        public RectTransform fbBtnPos;
        public Button facebookButton;
        public Text facebookButtonLabel;
        public GameObject facebookConnectAnim;
        public Button signInWithAppleButton;
        public Text signInWithAppleLabel;

        [Header("Themes Section")]
        public Button changeThemesButton;
        public Image themesIcon;

        [Header("Inbox Section")]
        public Button inboxButton;
        public Text messagesCount;
        public Image inboxNotification;

        //Signals
        public Signal inboxButtonClickedSignal = new Signal();
        public Signal changeThemesButtonClickedSignal = new Signal();
        public Signal socialConnectionButtonClickedSignal = new Signal();
        public Signal facebookButtonClickedSignal = new Signal();
        public Signal profilePicButtonClickedSignal = new Signal();
        public Signal signInWithAppleClicked = new Signal();
        public Signal<JoinedTournamentData> joinedTournamentButtonClickedSignal = new Signal<JoinedTournamentData>();
        public Signal<LiveTournamentData> openTournamentButtonClickedSignal = new Signal<LiveTournamentData>();

        //Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        // Services
        [Inject] public ILocalizationService localizationService { get; set; }
        [Inject] public IAudioService audioService { get; set; }

        private static StoreIconsContainer iconsContainer;

        public void Init()
        {
            if(avatarContainer != null)
            {
                avatarContainerStartRef = avatarContainer;
            }

            if (iconsContainer == null)
            {
                iconsContainer = StoreIconsContainer.Load();
            }

            if (facebookButton != null)
            {
                facebookButton.onClick.AddListener(OnFacebookButtonClicked);
                facebookConnectAnim.SetActive(false);
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.onClick.AddListener(OnSignInWithAppleClicked);
            }

            profilePicButton.onClick.AddListener(OnProfilePicButtonClicked);
            eloScoreLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);

            if (playTournamentButtonLabel != null)
            {
                playTournamentButtonLabel.text = localizationService.Get(LocalizationKey.PLAY_TOURNAMENT);
            }

            if (tournamentLiveLabel != null)
            {
                tournamentLiveLabel.text = localizationService.Get(LocalizationKey.LIVE_TEXT);
            }

            if (playTournamentButton != null)
            {
                playTournamentButton.onClick.AddListener(OnPlayTournamentButtonClicked);
            }

            if (inboxButton != null)
            {
                inboxButton.onClick.AddListener(OnInboxButtonClicked);
            }

            if(changeThemesButton != null)
            {
                changeThemesButton.onClick.AddListener(OnClickedChangeThemesButton);
            }

            if(socialConnectionButton != null)
            {
                socialConnectionButton.onClick.AddListener(OnClickedSocialConnectionButton);
            }
        }

        public void Show()
        {
            UpdateThemeIcon();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void CleanUp()
        {
            if (facebookButton != null)
            {
                facebookButton.onClick.RemoveAllListeners();
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.onClick.RemoveAllListeners();
            }
        }

        public void UpdateTournamentView()
        {
            if (liveTournamentIcon != null)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                LiveTournamentData openTournament = tournamentsModel.GetOpenTournament();
                if (joinedTournament != null)
                {
                    liveTournamentIcon.sprite = tournamentsModel.GetStickerSprite(joinedTournament.type);
                    liveTournamentIcon.SetNativeSize();
                    playTournamentButton.interactable = true;
                    liveTournamentGO.SetActive(true);
                }
                else if (openTournament != null)
                {
                    liveTournamentIcon.sprite = tournamentsModel.GetStickerSprite(openTournament.type);
                    liveTournamentIcon.SetNativeSize();
                    playTournamentButton.interactable = true;
                    liveTournamentGO.SetActive(true);
                }
                else {
                    liveTournamentIcon.sprite = null;
                    playTournamentButton.interactable = false;
                    liveTournamentGO.SetActive(false);
                }
            }
        }

        public void UpdateView(ProfileVO vo)
        {
            /*if (facebookButtonLabel != null)
            {
                facebookButtonLabel.text = localizationService.Get(LocalizationKey.FACEBOOK_LOGIN);
            }

            if (signInWithAppleLabel != null)
            {
                signInWithAppleLabel.text = localizationService.Get(LocalizationKey.SIGN_IN);
            }*/

            profileName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);
            playerId = vo.playerId;
            SetProfilePic(vo);

            var showLoginButton = !(vo.isFacebookLoggedIn || vo.isAppleSignedIn);
            if (facebookButton != null)
            {
                //facebookButton.gameObject.SetActive(showLoginButton);
                facebookConnectAnim.SetActive(false);
            }

            ChangeSocialAccountButtonsState(showLoginButton, vo.isAppleSignInSupported && showLoginButton);
        }

        public void FacebookAuthResult(AuthFacebookResultVO vo)
        {
            if (vo.isSuccessful)
            {
                if (vo.pic != null)
                {
                    SetProfilePic(vo.pic);
                }
                profileName.text = vo.name;
                eloScoreValue.text = vo.rating.ToString();
                ChangeSocialAccountButtonsState(false, false);
            }

            if (facebookConnectAnim != null)
            {
                facebookConnectAnim.SetActive(false);
            }
        }

        public void SignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            if (vo.isSuccessful)
            {
                profileName.text = vo.name;
                ChangeSocialAccountButtonsState(false, false);
            }

            if (facebookConnectAnim != null)
            {
                facebookConnectAnim.SetActive(false);
            }
        }

        public void SignOutSocialAccount()
        {
            ChangeSocialAccountButtonsState(true, true);
        }

        void ChangeSocialAccountButtonsState(bool showFBLoginButton, bool showSignInWithAppleButton)
        {
            if (facebookIcon != null)
            {
                //facebookButton.gameObject.SetActive(showFBLoginButton);
                facebookIcon.enabled = showFBLoginButton;
            }

            if (appleIcon != null)
            {
                //signInWithAppleButton.gameObject.SetActive(showSignInWithAppleButton);
                appleIcon.enabled = showSignInWithAppleButton;
            }

            /*if (avatarContainer && avatarContainerRef)
            {
                if (!showFBLoginButton && !showSignInWithAppleButton)
                {
                    avatarContainer.localScale = avatarContainerRef.localScale;
                    avatarContainer.localPosition = avatarContainerRef.localPosition;
                }
                else
                {
                    avatarContainer.localScale = new Vector3(1, 1, 1);
                    avatarContainer.localPosition = avatarContainerStartRef.localPosition;
                }
            }*/

            /*if (!showSignInWithAppleButton && fbBtnPos != null)
            {
                facebookButton.transform.localPosition = fbBtnPos.localPosition;
            }*/

            if (socialConnectionButton != null)
            {
                if (!showSignInWithAppleButton)
                {
                    facebookIcon.transform.localPosition = fbBtnPos.localPosition;
                    socialConnectionButton.onClick.AddListener(OnFacebookButtonClicked);
                }
                else
                {
                    socialConnectionButton.onClick.AddListener(OnClickedSocialConnectionButton);
                }
            }
        }

        public void UpdateEloScores(EloVO vo)
        {
            eloScoreValue.text = vo.playerEloScore.ToString();
        }

        public void ToggleFacebookButton(bool toggle)
        {
            /*if (facebookButton != null)
            {
                facebookButton.interactable = toggle;
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.interactable = toggle;
            }*/

            if (socialConnectionButton != null)
            {
                socialConnectionButton.interactable = toggle;
            }
        }

        public void UpdateProfilePic(Photo vo)
        {
            if (vo.sprite == null)
                Debug.Log("UpdateProfilePic sprite is null");

            if (vo.sprite != null)
            {
                SetProfilePic(vo.sprite);
            }
        }

        private void SetProfilePic(Sprite sprite)
        {
            Debug.Log("OnProfilePicUpdate SetProfilePic");
            noProfilePicBorder.SetActive(false);
            hasProfilePicBorder.SetActive(false);
            avatarIcon.gameObject.SetActive(false);
            avatarBG.gameObject.SetActive(false);
            profilePic.color = Color.white;

            if (sprite == null)
            {
                profilePic.sprite = defaultAvatar;
                noProfilePicBorder.SetActive(true);
            }
            else
            {
                profilePic.sprite = sprite;
                hasProfilePicBorder.SetActive(true);
            }
        }

        private void SetProfilePic(ProfileVO vo)
        {
            noProfilePicBorder.SetActive(false);
            hasProfilePicBorder.SetActive(false);
            avatarBG.gameObject.SetActive(false);
            avatarIcon.gameObject.SetActive(false);
            ShowPremiumBorder(false);
            SetLeagueBorder(vo.leagueBorder);

            if (vo.playerPic != null)
            {
                profilePic.sprite = vo.playerPic;
                hasProfilePicBorder.SetActive(true);
            }
            else 
            {
                profilePic.sprite = defaultAvatar;

                if (vo.avatarId != null)
                {
                    Sprite newSprite = defaultAvatarContainer.GetSprite(vo.avatarId);
                    if(newSprite != null)
                    {
                        avatarIcon.gameObject.SetActive(true);
                        avatarBG.gameObject.SetActive(true);
                        avatarIcon.sprite = newSprite;
                        avatarBG.sprite = whiteAvatar;
                        avatarBG.color = Colors.Color(vo.avatarColorId);
                    }
                }
                 
                noProfilePicBorder.SetActive(true);
            }

        }

        private void OnFacebookButtonClicked()
        {
            facebookButtonClickedSignal.Dispatch();
            facebookConnectAnim.SetActive(true);
        }

        private void OnSignInWithAppleClicked()
        {
            signInWithAppleClicked.Dispatch();
            facebookConnectAnim.SetActive(true);
        }

        private void OnProfilePicButtonClicked()
        {
            profilePicButtonClickedSignal.Dispatch();
        }

        public void ShowPremiumBorder(bool show)
        {
            premiumBorder.SetActive(show);
        }

        private void OnPlayTournamentButtonClicked()
        {
            JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
            LiveTournamentData openTournament = tournamentsModel.GetOpenTournament();

            if (joinedTournament != null)
            {
                joinedTournamentButtonClickedSignal.Dispatch(joinedTournament);
            }
            else if (openTournament != null)
            {
                openTournamentButtonClickedSignal.Dispatch(openTournament);
            }

            analyticsService.Event(AnalyticsEventId.tournament_promo);
        }

        public void SetLeagueBorder(Sprite border)
        {
            leagueBorder.gameObject.SetActive(border != null);
            leagueBorder.sprite = border;
        }

        private void OnInboxButtonClicked()
        {
            audioService.PlayStandardClick();
            inboxButtonClickedSignal.Dispatch();
        }

        public void UpdateMessagesCount(long messages)
        {
            if (messagesCount != null)
            {
                if (messages > 0)
                {
                    messagesCount.text = messages.ToString();
                    messagesCount.enabled = true;
                    inboxNotification.enabled = true;
                }
                else
                {
                    inboxNotification.enabled = false;
                    messagesCount.enabled = false;
                }
            }
        }

        public void UpdateThemeIcon()
        {
            if (themesIcon != null)
            {
                themesIcon.sprite = iconsContainer.GetSprite(playerModel.activeSkinId);
            }
        }

        private void OnClickedChangeThemesButton()
        {
            audioService.PlayStandardClick();
            changeThemesButtonClickedSignal.Dispatch();
        }

        private void OnClickedSocialConnectionButton()
        {
            audioService.PlayStandardClick();
            socialConnectionButtonClickedSignal.Dispatch();
        }
    }
}