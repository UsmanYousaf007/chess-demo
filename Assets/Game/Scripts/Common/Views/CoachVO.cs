using UnityEngine;
using TurboLabz.InstantFramework;

public struct CoachVO
{
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public string moveFrom;
    public string moveTo;
    public string activeSkinId;
    public string pieceName;
    public bool isBestMove;
    public IAudioService audioService;
    public IAnalyticsService analyticsService;
    public AnalyticsContext analyticsContext;
    public IDownloadablesModel downloadablesModel;
}
