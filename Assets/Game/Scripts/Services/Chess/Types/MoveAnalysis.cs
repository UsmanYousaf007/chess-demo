namespace TurboLabz.Chess
{
    public class MoveAnalysis 
    {
        public ChessMove playerMove;
        public ChessMove bestMove;
        public MoveQuality moveQuality;
        public float strength;
        public bool isPlayerMove;
        public float playerAdvantage;
        public int playerScore;
        public int playerScoreDebug;
        public int bestScore;

        public static MoveQuality MoveQualityToEnum(string quality)
        {
            var rv = MoveQuality.NORMAL;

            switch (quality)
            {
                case "BLUNDER":
                    rv = MoveQuality.BLUNDER;
                    break;

                case "MISTAKE":
                    rv = MoveQuality.MISTAKE;
                    break;

                case "PERFECT":
                    rv = MoveQuality.PERFECT;
                    break;
            }

            return rv;
        }
    }

    public enum MoveQuality
    {
        BLUNDER,
        MISTAKE,
        PERFECT,
        NORMAL
    }

    public class MatchAnalysis
    {
        public int blunders;
        public int perfectMoves;
        public int mistakes;
    }
}
