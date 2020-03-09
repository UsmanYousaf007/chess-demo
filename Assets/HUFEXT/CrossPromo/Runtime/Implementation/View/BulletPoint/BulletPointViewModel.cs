using HUF.Utils.Configs.API;
using UnityEngine;

namespace HUFEXT.CrossPromo.Implementation.View.BulletPoint
{
    public class BulletPointViewModel
    {
        readonly BulletPointView view;
        
        public BulletPointViewModel(RectTransform parent)
        {
            var config = HConfigs.GetConfig<CrossPromoLocalConfig>();
            view = Object.Instantiate(config.BulletPointView, parent);
        }

        public void Activate(bool immediate = false)
        {
            view.SetActive(true, immediate);
        }

        public void Deactivate(bool immediate = false)
        {
            view.SetActive(false, immediate);
        }

        public void DestroyView()
        {
            Object.Destroy(view.gameObject);
        }

        public void SetActive(bool isActive)
        {
            view.gameObject.SetActive(isActive);
        }
        
        public void SetColor(Color color)
        {
            view.SetColor(color);
        }
    }
}