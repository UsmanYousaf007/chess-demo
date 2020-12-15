using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class LoginDlgMediator : Mediator
    {
        //View Injection
        [Inject] public LoginDlgView view { get; set; }

        //Dispatch Signals
        [Inject] public AuthFaceBookSignal authFaceBookSignal { get; set; }
        [Inject] public AuthSignInWithAppleSignal authSignInWithAppleSignal { get; set; }

        //Services
        [Inject] public IPromotionsService promotionsService { get; set; }
        [Inject] public ISignInWithAppleService signInWithAppleService { get; set; }

        public override void OnRegister()
        {
            view.Init(signInWithAppleService.IsSupported());
            view.facebookLoginSignal.AddListener(OnFacebookLoginSignal);
            view.siwaSignal.AddListener(OnSiwaSignal);
            view.guestSignal.AddListener(OnGuestSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOGIN_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LOGIN_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(AuthFacebookResultSignal))]
        public void OnAuthFacebookResult(AuthFacebookResultVO vo)
        {
            view.OnSignInCompleted();

            if (vo.isSuccessful)
            {
                CloseDlg();
            }
        }

        [ListensTo(typeof(AuthSignInWithAppleResultSignal))]
        public void OnAuthSignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            view.OnSignInCompleted();

            if (vo.isSuccessful)
            {
                CloseDlg();
            }
        }

        [ListensTo(typeof(LoginAsGuestSignal))]
        public void OnLoginAsGuest()
        {
            OnGuestSignal();
        }

        private void OnFacebookLoginSignal()
        {
            authFaceBookSignal.Dispatch();
        }

        private void OnSiwaSignal()
        {
            authSignInWithAppleSignal.Dispatch();
        }

        private void OnGuestSignal()
        {
            CloseDlg();
        }

        private void CloseDlg()
        {
            promotionsService.LoadPromotion();
        }
    }
}
