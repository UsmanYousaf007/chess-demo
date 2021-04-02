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
        public RectTransform advantageFiller;
        public Image advantageFillerImage;
        public Text whiteAdvantage;
        public Text blackAdvantage;
    }

    public AnalysisMove normal;
    public AnalysisMove zoomed;
    public Sprite whiteAdvantageFilledSprite;
    public Sprite whiteAdvantagePartialSprite;
    public float whiteAdvantageOriginalWidth;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMove(string moveNumber, string move, Sprite moveQuality, Sprite piece, int whiteAdvantage, int blackAdvantage)
    {
        SetupMove(normal, moveNumber, move, moveQuality, piece, whiteAdvantage, blackAdvantage);
        SetupMove(zoomed, moveNumber, move, moveQuality, piece, whiteAdvantage, blackAdvantage);
    }

    private void SetupMove(AnalysisMove analysis, string moveNumber, string move, Sprite moveQuality, Sprite piece, int whiteAdvantage, int blackAdvantage)
    {
        analysis.moveNumber.text = moveNumber;
        analysis.move.text = move;
        analysis.piece.sprite = piece;
        analysis.moveQuality.enabled = moveQuality != null;
        analysis.moveQuality.sprite = moveQuality;
        analysis.whiteAdvantage.text = $"+{whiteAdvantage}";
        analysis.blackAdvantage.text = $"+{blackAdvantage}";
        analysis.whiteAdvantage.enabled = whiteAdvantage > blackAdvantage;
        analysis.blackAdvantage.enabled = whiteAdvantage < blackAdvantage;

        var fillAmount = whiteAdvantageOriginalWidth * ((float)(30 + whiteAdvantage) / 60);
        analysis.advantageFiller.sizeDelta = new Vector2(fillAmount, analysis.advantageFiller.sizeDelta.y);
        analysis.advantageFillerImage.enabled = fillAmount > 15;
        analysis.advantageFillerImage.sprite = fillAmount >= 215 ? whiteAdvantageFilledSprite : whiteAdvantagePartialSprite;
    }
}
