namespace TurboLabz.InstantFramework
{
    public class ConfirmDlgVO
    {
        public string title;
        public string desc;
        public string yesButtonText;
        public delegate void OnClick();
        public OnClick onClickYesButton;
    }
}
