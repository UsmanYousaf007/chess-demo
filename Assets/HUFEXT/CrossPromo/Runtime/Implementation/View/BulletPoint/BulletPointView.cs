using HUF.Utils.Configs.API;
using UnityEngine;
using UnityEngine.UI;

namespace HUFEXT.CrossPromo.Implementation.View.BulletPoint
{
    public class BulletPointView : MonoBehaviour
    {
        [SerializeField] Image outerBulletImage = default;
        [SerializeField] Image filledBulletImage = default;
        
        float fadeDuration;

        void OnEnable()
        {
            var config = HConfigs.GetConfig<CrossPromoRemoteConfig>();
            fadeDuration = config.BulletPointFadeTime;
        }

        public void SetActive(bool active, bool immediate)
        {
            filledBulletImage.CrossFadeAlpha(active ? 1.0f : 0.0f, immediate ? 0.0f : fadeDuration, false);
        }

        public void SetColor(Color color)
        {
            outerBulletImage.color = color;
            filledBulletImage.color = color;
        }
    }
}