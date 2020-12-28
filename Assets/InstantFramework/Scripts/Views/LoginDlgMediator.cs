using strange.extensions.mediation.impl;
using DG.Tweening;

namespace TurboLabz.InstantFramework
{
    public class LoginDlgMediator : Mediator
    {
        //View Injection
        [Inject] public LoginDlgView view { get; set; }

        //Dispatch Signals
        [Inject] public AuthFaceBookSignal authFaceBookSignal { get; set; }
        [Inject] public AuthSignInWithAppleSignal authSignInWithAppleSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

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
            SignInCompleted(vo.isSuccessful);
        }

        [ListensTo(typeof(AuthSignInWithAppleResultSignal))]
        public void OnAuthSignInWithAppleResult(AuthSignInWIthAppleResultVO vo)
        {
            SignInCompleted(vo.isSuccessful);
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
            view.view.DOFade(Settings.MIN_ALPHA, Settings.TWEEN_DURATION).OnComplete(() => {
                navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                promotionsService.LoadPromotion();
            });
        }

        private void SignInCompleted(bool isSucccess)
        {
            view.OnSignInCompleted();

            if (isSucccess)
            {
                CloseDlg();
            }
        }
    }
}
