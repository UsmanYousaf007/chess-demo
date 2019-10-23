using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public partial class LobbyView : View
    {
        [Header("Lobby Promotions")]
        public Transform loadPromotionAt;
        public Transform moveScrollViewTo;
        public Transform promotionContainer;
        public GameObject coachTrainingDailogue;
        public GameObject strengthTrainingDailogue;

        private GameObject spawnedBanner;
        private Vector3 scrollViewOrignalPosition;

        public void ShowPromotion(string key)
        {
            if (spawnedBanner != null)
            {
                Destroy(spawnedBanner);
            }

            if (LobbyPromotionKeys.Contains(key))
            {
                var prefabToInstantiate = Resources.Load(key);

                if (prefabToInstantiate != null)
                {
                    spawnedBanner = Instantiate(prefabToInstantiate, loadPromotionAt.position, Quaternion.identity, promotionContainer) as GameObject;
                    scrollRect.transform.localPosition = moveScrollViewTo.localPosition;
                    spawnedBanner.GetComponent<Button>().onClick.AddListener(() => OnClickPromotion(key));
                }
                else
                {
                    LogUtil.Log(string.Format("Banner promotion resource againt key '{0}' not found", key), "red");
                }
            }
            else
            {
                scrollRect.transform.localPosition = scrollViewOrignalPosition;
            }
        }

        private void OnClickPromotion(string key)
        {
            audioService.PlayStandardClick();

            switch (key)
            {
                case LobbyPromotionKeys.COACH_BANNER:
                    coachTrainingDailogue.SetActive(true);
                    break;

                case LobbyPromotionKeys.STRENGTH_BANNER:
                    strengthTrainingDailogue.SetActive(true);
                    break;
            }
        }
    }
}