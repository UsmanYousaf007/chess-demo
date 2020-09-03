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
    public class ProfileView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Button facebookButton;
        public Text facebookButtonLabel;
        public GameObject facebookConnectAnim;
        public Button signInWithAppleButton;
        public Text signInWithAppleLabel;

        public Button playTournamentButton;
        public Text playTournamentButtonLabel;
        public Text tournamentLiveLabel;
        public Image liveTournamentIcon;

        public Sprite defaultAvatar;
        public Sprite whiteAvatar;
        public Image profilePic;
        public Image avatarBG;
        public Image avatarIcon;
        public Text profileName;
        public GameObject noProfilePicBorder;
        public GameObject hasProfilePicBorder;
        public GameObject premiumBorder;
        public Text eloScoreLabel;
        public Text eloScoreValue;
        public Image playerFlag;
        private string playerId;
        private SpritesContainer defaultAvatarContainer;

        public RectTransform fbBtnPos;

        public RectTransform avatarContainer;
        Transform avatarContainerStartRef;
        public RectTransform avatarContainerRef;

        public Button profilePicButton;
        
        public Signal facebookButtonClickedSignal = new Signal();
        public Signal profilePicButtonClickedSignal = new Signal();
        public Signal signInWithAppleClicked = new Signal();
        public Signal<JoinedTournamentData> joinedTournamentButtonClickedSignal = new Signal<JoinedTournamentData>();
        public Signal<LiveTournamentData> openTournamentButtonClickedSignal = new Signal<LiveTournamentData>();

        //Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public void Init()
        {
            if(avatarContainer != null)
            {
                avatarContainerStartRef = avatarContainer;
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
                    playTournamentButton.interactable = true;
                }
                else if (openTournament != null)
                {
                    liveTournamentIcon.sprite = tournamentsModel.GetStickerSprite(openTournament.type);
                    playTournamentButton.interactable = true;
                }
                else {
                    liveTournamentIcon.sprite = null;
                    playTournamentButton.interactable = false;
                }
            }
        }

        public void UpdateView(ProfileVO vo)
        {
            if (facebookButtonLabel != null)
            {
                facebookButtonLabel.text = localizationService.Get(LocalizationKey.FACEBOOK_LOGIN);
            }

            if (signInWithAppleLabel != null)
            {
                signInWithAppleLabel.text = localizationService.Get(LocalizationKey.SIGN_IN);
            }

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

            /*if (signInWithAppleButton != null)
            {
                signInWithAppleButton.gameObject.SetActive(vo.isAppleSignInSupported && showLoginButton);
                if(!signInWithAppleButton.IsActive() && fbBtnPos != null)
                {
                    facebookButton.transform.localPosition = fbBtnPos.localPosition;
                }
            }*/
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

                /*if (facebookButton != null)
                {
                    facebookButton.gameObject.SetActive(false);
                }

                if (signInWithAppleButton != null)
                {
                    signInWithAppleButton.gameObject.SetActive(false);
                }*/

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

                /*if (facebookButton != null)
                {
                    facebookButton.gameObject.SetActive(false);
                }

                if (signInWithAppleButton != null)
                {
                    signInWithAppleButton.gameObject.SetActive(false);
                }*/

                ChangeSocialAccountButtonsState(false, false);
            }

            if (facebookConnectAnim != null)
            {
                facebookConnectAnim.SetActive(false);
            }
        }

        public void SignOutSocialAccount()
        {
            /*if (facebookButton != null)
            {
                facebookButton.gameObject.SetActive(true);
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.gameObject.SetActive(true);
            }*/

            ChangeSocialAccountButtonsState(true, true);
        }

        void ChangeSocialAccountButtonsState(bool showFBLoginButton, bool showSignInWithAppleButton)
        {
            if (facebookButton != null)
            {
                facebookButton.gameObject.SetActive(showFBLoginButton);
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.gameObject.SetActive(showSignInWithAppleButton);
            }

            if (avatarContainer && avatarContainerRef)
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
            }

            if (!showSignInWithAppleButton && fbBtnPos != null)
            {
                facebookButton.transform.localPosition = fbBtnPos.localPosition;
            }
        }

        public void UpdateEloScores(EloVO vo)
        {
            eloScoreValue.text = vo.playerEloScore.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ToggleFacebookButton(bool toggle)
        {
            if (facebookButton != null)
            {
                facebookButton.interactable = toggle;
            }

            if (signInWithAppleButton != null)
            {
                signInWithAppleButton.interactable = toggle;
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
            ShowPremiumBorder(vo.isPremium);

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
    }
}