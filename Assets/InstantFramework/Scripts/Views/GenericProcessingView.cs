using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class GenericProcessingView : View
    {
        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}
