using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TurboLabz.Chess;
using TurboLabz.InstantGame;

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
    public int MoveNumber => moveNumber;

    private bool isLocked;
    private int moveNumber;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupMove(int moveNumberInt, string moveNumber, string move, Sprite moveQuality, Sprite piece, float playerAdvantage, ChessColor playerColor, bool isLocked)
    {
        this.isLocked = isLocked;
        this.moveNumber = moveNumberInt;
        SetupMove(normal, moveNumber, move, moveQuality, piece, playerAdvantage, playerColor);
        SetupMove(zoomed, moveNumber, move, moveQuality, piece, playerAdvantage, playerColor);
    }

    private void SetupMove(AnalysisMove analysis, string moveNumber, string move, Sprite moveQuality, Sprite piece, float playerAdvantage, ChessColor playerColor)
    {
        analysis.moveNumber.text = moveNumber;
        analysis.move.text = move;
        analysis.piece.sprite = piece;
        analysis.moveQuality.enabled = moveQuality != null || isLocked;
        analysis.moveQuality.sprite = isLocked ? moveQualityInactive : moveQuality;

        analysis.whiteAdvantage.text = analysis.blackAdvantage.text = string.Format("{0:0.0}", playerAdvantage);
        analysis.whiteAdvantage.enabled = playerColor == ChessColor.WHITE && !isLocked;
        analysis.blackAdvantage.enabled = playerColor == ChessColor.BLACK && !isLocked;

        playerAdvantage = playerColor == ChessColor.BLACK ? playerAdvantage * -1 : playerAdvantage;
        var fillAmount = 230 * ((10 + playerAdvantage) / 20);
        analysis.advantageFiller.sizeDelta = new Vector2(fillAmount, analysis.advantageFiller.sizeDelta.y);
        analysis.advantageFillerImage.enabled = fillAmount > 15 && !isLocked;
        analysis.advantageFillerImage.sprite = fillAmount >= 215 ? whiteAdvantageFilledSprite : whiteAdvantagePartialSprite;

        analysis.blackAdvantage.color = playerColor == ChessColor.BLACK && analysis.advantageFillerImage.sprite == whiteAdvantageFilledSprite ? Colors.BLACK : Colors.WHITE;
        analysis.whiteAdvantage.color = playerColor == ChessColor.WHITE && analysis.advantageFillerImage.enabled ? Colors.BLACK : Colors.WHITE;
    }
}
