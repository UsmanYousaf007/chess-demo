using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class GenericProcessingMediator : Mediator
    {
        //View Injection
        [Inject] public GenericProcessingView view { get; set; }

        [ListensTo(typeof(ShowGenericProcessingSignal))]
        public void OnShow(bool show)
        {
            view.Show(show);
        }
    }
}
