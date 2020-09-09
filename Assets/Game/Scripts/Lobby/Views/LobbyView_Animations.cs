using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using TurboLabz.TLUtils;
using TMPro;
using DG.Tweening;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    public partial class LobbyView : View
    {
        [Header("Rating boost")]
        public RectTransform ratingBoost;
        public TextMeshProUGUI textRatingBoost;
        public Transform startPivot;
        public Transform endPivot;

        [Tooltip("Color to fade from")]
        [SerializeField]
        private Color StartColor = Color.white;

        public void RatingBoostAnimation(int ratingBoostVal)
        {
            textRatingBoost.text = "+" + ratingBoostVal;
            audioService.Play(audioService.sounds.SFX_REWARD_UNLOCKED);
            textRatingBoost.color = StartColor;
            textRatingBoost.gameObject.transform.position = startPivot.position;
            ratingBoost.gameObject.SetActive(true);
            StartCoroutine(RatingBoostCR());
        }

        IEnumerator RatingBoostCR()
        {
            yield return new WaitForSeconds(0.5f);

            textRatingBoost.DOFade(0f, 4.5f);
            textRatingBoost.transform.DOMoveY(endPivot.position.y, 4.5f);

            yield return new WaitForSeconds(6.2f);

            ratingBoost.gameObject.SetActive(false);
        }

    }
}