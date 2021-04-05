using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnalysisMoveView : MonoBehaviour
{
    [Serializable]
    public class AnalysisMove
    {
        public Text moveNumber;
        public Text move;
        public Image piece;
        public Image moveQuality;
        public Image advantageBg;
        public RectTransform advantageFiller;
        public Image advantageFillerImage;
        public Text whiteAdvantage;
        public Text blackAdvantage;
    }

    public AnalysisMove normal;
    public AnalysisMove zoomed;
    public Sprite whiteAdvantageFilledSprite;
    public Sprite whiteAdvantagePartialSprite;
    public Sprite moveQualityInactive;

    public bool IsLocked => isLocked;

    private bool isLocked;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMove(string moveNumber, string move, Sprite moveQuality, Sprite piece, int whiteAdvantage, int blackAdvantage, bool isLocked)
    {
        this.isLocked = isLocked;
        SetupMove(normal, moveNumber, move, moveQuality, piece, whiteAdvantage, blackAdvantage);
        SetupMove(zoomed, moveNumber, move, moveQuality, piece, whiteAdvantage, blackAdvantage);
    }

    private void SetupMove(AnalysisMove analysis, string moveNumber, string move, Sprite moveQuality, Sprite piece, int whiteAdvantage, int blackAdvantage)
    {
        analysis.moveNumber.text = moveNumber;
        analysis.move.text = move;
        analysis.piece.sprite = piece;
        analysis.moveQuality.enabled = moveQuality != null || isLocked;
        analysis.moveQuality.sprite = isLocked ? moveQualityInactive : moveQuality;

        var showAdvantage = whiteAdvantage != 0 || isLocked;
        ShowAdvantage(analysis, showAdvantage);

        if (showAdvantage)
        {
            analysis.whiteAdvantage.text = $"+{whiteAdvantage}";
            analysis.blackAdvantage.text = $"+{blackAdvantage}";
            analysis.whiteAdvantage.enabled = whiteAdvantage > blackAdvantage && !isLocked;
            analysis.blackAdvantage.enabled = whiteAdvantage < blackAdvantage && !isLocked;

            var fillAmount = 230 * ((float)(30 + whiteAdvantage) / 60);
            analysis.advantageFiller.sizeDelta = new Vector2(fillAmount, analysis.advantageFiller.sizeDelta.y);
            analysis.advantageFillerImage.enabled = fillAmount > 15 && !isLocked;
            analysis.advantageFillerImage.sprite = fillAmount >= 215 ? whiteAdvantageFilledSprite : whiteAdvantagePartialSprite;
        }
    }

    private void ShowAdvantage(AnalysisMove analysis, bool show)
    {
        analysis.advantageBg.enabled =
            analysis.advantageFillerImage.enabled =
            analysis.blackAdvantage.enabled =
            analysis.whiteAdvantage.enabled = show;
    }
}
