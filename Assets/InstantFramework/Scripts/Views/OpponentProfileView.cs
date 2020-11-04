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
    public class OpponentProfileView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Sprite defaultAvatar;
        public Sprite whiteAvatar;
        public Image profilePic;
        public Image avatarBg;
        public Image avatarIcon;
        public Text profileName;
        public GameObject noProfilePicBorder;
        public GameObject hasProfilePicBorder;
        //public GameObject premiumBorder;
        public Image leagueBorder;
        public Text eloScoreLabel;
        public Text eloScoreValue;
        public Image playerFlag;
        public Image onlineStatus;
        public Sprite online;
        public Sprite offline;
        public Sprite activeStatus;
        public Button viewProfileBtn;
        bool status; 

        private string opponentId;
        private SpritesContainer defaultAvatarContainer;

        public Signal viewProfileSignal = new Signal();

        public void Init()
        {
            eloScoreLabel.text = localizationService.Get(LocalizationKey.ELO_SCORE);
            defaultAvatarContainer = SpritesContainer.Load(GSBackendKeys.DEFAULT_AVATAR_ALTAS_NAME);
            viewProfileBtn.onClick.AddListener(ViewProfileClicked);
        }

        public void CleanUp()
        {
        }

        public void UpdateView(ProfileVO vo)
        {
            profileName.text = vo.playerName;
            eloScoreValue.text = vo.eloScore.ToString();
            playerFlag.sprite = Flags.GetFlag(vo.countryId);
            opponentId = vo.playerId;

            if (!vo.isOnline && vo.isActive)
            {
                onlineStatus.sprite = activeStatus;
            }
            else
            {
                onlineStatus.sprite = vo.isOnline ? online : offline; 
            }

            SetProfilePic(vo);
        }

        public void UpdateEloScores(EloVO vo)
        {
            if (vo.opponentId == opponentId)
            {
                eloScoreValue.text = vo.opponentEloScore.ToString();
            }
        }

        public void UpdateFriendOnlineStatusSignal(string friendId, bool isOnline, bool isActive)
        {
            if (friendId == opponentId)
            {
                if (!isOnline && isActive)
                {
                    onlineStatus.sprite = activeStatus;
                }
                else
                {
                    onlineStatus.sprite = isOnline ? online : offline;
                    status = isOnline;
                }
            }
        }

        public void ForceUpdateFriendOnlineStatusSignal(string friendId, bool isOnline, bool isActive)
        {
            if (friendId == opponentId)
            {
                if(status == false)
                {
                    if (!isOnline && isActive)
                    {
                        onlineStatus.sprite = activeStatus;
                    }
                    else
                    {
                        onlineStatus.sprite = isOnline ? online : offline;
                        status = isOnline;
                    }
                }
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void SetProfilePic(ProfileVO vo)
        {
            noProfilePicBorder.SetActive(false);
            hasProfilePicBorder.SetActive(false);
            avatarBg.gameObject.SetActive(false);
            avatarIcon.gameObject.SetActive(false);
            //premiumBorder.gameObject.SetActive(vo.isPremium);
            leagueBorder.gameObject.SetActive(vo.leagueBorder != null);
            leagueBorder.sprite = vo.leagueBorder;
            leagueBorder.SetNativeSize();

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
                    if (newSprite != null)
                    {
                        avatarIcon.gameObject.SetActive(true);
                        avatarBg.gameObject.SetActive(true);
                        avatarIcon.sprite = newSprite;
                        avatarBg.sprite = whiteAvatar;
                        avatarBg.color = Colors.Color(vo.avatarColorId);
                    }
                }

                noProfilePicBorder.SetActive(true);
            }
        }

        // TODO: remove code duplication
        public void UpdateProfilePic(string playerId, Sprite sprite)
        {
            if (playerId == opponentId)
            {
                noProfilePicBorder.SetActive(false);
                hasProfilePicBorder.SetActive(false);
                avatarBg.gameObject.SetActive(false);
                avatarIcon.gameObject.SetActive(false);
                if (sprite != null)
                {
                    profilePic.sprite = sprite;
                    hasProfilePicBorder.SetActive(true);
                }
            }
        }

        private void ViewProfileClicked()
        {
            viewProfileSignal.Dispatch();
        }
    }
}