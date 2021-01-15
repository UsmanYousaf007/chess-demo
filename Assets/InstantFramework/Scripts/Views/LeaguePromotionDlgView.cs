/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using System;
using DG.Tweening;
using TurboLabz.InstantGame;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class LeaguePromotionDlgView : View
    {
        [SerializeField] private Image playerTitleImg;
        [SerializeField] private RewardUIContainer[] _dailyRewardPerksContainers;
        [SerializeField] private RewardUIContainer[] _rewardContainers;
        [SerializeField] private ProfilePicView playerPic;
        [SerializeField] private TextMeshProUGUI rewardsSubHeadingText;
        [SerializeField] private Button collectBtn;
        [SerializeField] private ParticleSystem _leagueRingSlamEffect;
        [SerializeField] private ParticleSystem _leagueTitleSlamEffect;
        [SerializeField] private Animator _animator;

        public Signal CollectBtnClickedSignal = new Signal();

        public void Init()
        {
            collectBtn.onClick.AddListener(() =>
            {
                CollectBtnClickedSignal?.Dispatch();
            });
        }

        public void Show()
        {
            gameObject.SetActive(true);

            StartCoroutine(StartAnimationCoroutine());
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _animator.enabled = false;

            StopCoroutine(StartAnimationCoroutine());
        }

        public void UpdateView(RewardDlgVO vo, Dictionary<string, int> dailyReward, LeagueTierIconsContainer.LeagueAsset leagueAssets)
        {
            rewardsSubHeadingText.text = "Daily Reward Perks";
            playerPic.UpdateView(vo.playerProfile);

            playerPic.SetLeagueBorder(leagueAssets != null ? leagueAssets.ringSprite : null);
            playerPic.leagueBorder.gameObject.SetActive(false);

            //// Daily reward perks panel update
            for (int j = 0; j < _dailyRewardPerksContainers.Length; j++)
            {
                _dailyRewardPerksContainers[j].containerParent.SetActive(false);
            }

            foreach (var reward in dailyReward)
            {
                for (int j = 0; j < _dailyRewardPerksContainers.Length; j++)
                {
                    if (_dailyRewardPerksContainers[j].shortCode == reward.Key)
                    {
                        if (reward.Value > 1000)
                        {
                            int quantity = reward.Value / 1000;
                            _dailyRewardPerksContainers[j].quantityText.text = $"{quantity}k";
                        }
                        else
                        {
                            _dailyRewardPerksContainers[j].quantityText.text = $"x{reward.Value}";
                        }

                        // Setting scale zero here for scale in animation later
                        _dailyRewardPerksContainers[j].containerParent.transform.localScale = Vector3.zero;
                        _dailyRewardPerksContainers[j].containerParent.SetActive(true);
                    }
                }
            }

            //// Promotion rewards panel update
            for (int j = 0; j < _rewardContainers.Length; j++)
            {
                _rewardContainers[j].containerParent.SetActive(false);
            }

            for (int i = 0; i < vo.rewardShortCodes.Count; i++)
            {
                for (int j = 0; j < _rewardContainers.Length; j++)
                {
                    if (_rewardContainers[j].shortCode == vo.rewardShortCodes[i])
                    {
                        int qty = vo.GetRewardItemQty(i);
                        if (qty > 1000)
                        {
                            qty /= 1000;
                            _rewardContainers[j].quantityText.text = $"{qty}k";
                        }
                        else
                        {
                            _rewardContainers[j].quantityText.text = $"x{qty}";
                        }

                        // Setting scale zero here for scale in animation later
                        _rewardContainers[j].containerParent.transform.localScale = Vector3.zero;
                        _rewardContainers[j].containerParent.SetActive(true);
                    }
                }
            }
        }

        public void UpdateLeagueTitle(IPlayerModel playerModel, ITournamentsModel tournamentsModel, LeagueTierIconsContainer.LeagueAsset leagueAssets)
        {
            if (playerModel.league > 0)
            {
                playerTitleImg.sprite = leagueAssets.nameImg;
                //playerTitleImg.gameObject.SetActive(true);
            }
        }

        public void PlayLeagueRingSlamEffect()
        {
            _leagueRingSlamEffect.gameObject.SetActive(true);
            _leagueTitleSlamEffect.gameObject.SetActive(true);

            _leagueTitleSlamEffect.Stop();
            _leagueRingSlamEffect.Stop();

            _leagueTitleSlamEffect.Play();
            _leagueRingSlamEffect.Play();
        }

        public void PlayLeagueTitleSlamEffect()
        {
        }

        public void ScaleInDailyRewards(float time)
        {
            for (int j = 0; j < _dailyRewardPerksContainers.Length; j++)
            {
                if (_dailyRewardPerksContainers[j].containerParent.activeSelf)
                {
                    _dailyRewardPerksContainers[j].containerParent.transform.DOScale(1f, time);
                }
            }
        }

        public void ScaleInPromotionRewards(float time)
        {
            for (int j = 0; j < _rewardContainers.Length; j++)
            {
                if (_rewardContainers[j].containerParent.activeSelf)
                {
                    _rewardContainers[j].containerParent.transform.DOScale(1f, time);
                }
            }
        }

        public void PlayDailyRewardsEffects()
        {
            for (int j = 0; j < _dailyRewardPerksContainers.Length; j++)
            {
                if (_dailyRewardPerksContainers[j].containerParent.activeSelf)
                {
                    _dailyRewardPerksContainers[j].effect.gameObject.SetActive(true);
                    _dailyRewardPerksContainers[j].effect.Play();
                }
            }
        }

        public void PlayPromotionRewardsEffects()
        {
            for (int j = 0; j < _rewardContainers.Length; j++)
            {
                if (_rewardContainers[j].containerParent.activeSelf)
                {
                    _rewardContainers[j].effect.gameObject.SetActive(true);
                    _rewardContainers[j].effect.Play();
                }
            }
        }

        IEnumerator StartAnimationCoroutine()
        {
            yield return new WaitForSeconds(1f);
            //yield return new WaitForFixedUpdate();

            _animator.enabled = true;
        }
    }
}