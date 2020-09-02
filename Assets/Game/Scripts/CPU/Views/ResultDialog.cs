using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ResultDialog : MonoBehaviour
{
    public Image resultsGameImage;
    public Sprite winSprite;
    public Sprite defeatSprite;
    public Sprite drawSprite;
    public Text resultsGameResultLabel;
    public Text resultsGameResultReasonLabel;
    public Text resultsFriendlyLabel;

    public Text resultsRatingValueLabel;
    public Text resultsRatingChangeLabel;

    public Button resultsBoostRatingButton;
    public Text resultsBoostRatingButtonLabel;
    public Text resultsRatingBoostedLabel;
    public Image resultsBoostRatingAdTVImage;
    public Text resultsBoostRatingAddedCount;
    public GameObject resultsBoostRatingToolTip;
    public Text resultsBoostRatingToolTipText;
    public Text resultsBoostRatingGemsCost;
    public Image resultsBoostRatingGemsBg;
    public Sprite enoughGemsSprite;
    public Sprite notEnoughGemsSprite;
    public string resultsBoostRatingShortCode;

    public Button resultsCollectRewardButton;
    public Text resultsCollectRewardButtonLabel;
    public Image resultsAdTVImage;

    public Button resultsViewBoardButton;
    public Text resultsViewBoardButtonLabel;

    public Button resultsSkipRewardButton;
    public Text resultsSkipRewardButtonLabel;

    public Button showCrossPromoButton;

    public RectTransform rewardBar;
    public Text earnRewardsText;
    public GameObject earnRewardsSection;
    public Image dailogueBg;

    public ViewBoardResults viewBoardResultPanel;

    // Tournament Match fields
    public Image tournamentTypeImage;
    public Text roundScoreHeading;
    public Text roundScoreText;
    public Text checkMateBonusText;
    public Button playMatchButton;
    public Text youHaveTicketsText;
    public Button backToArenaButton;
    public Text tournamentMatchPlayGemsCost;
    public Image tournamentMatchPlayGemsBg;
    public string ticketsShortCode;

}
