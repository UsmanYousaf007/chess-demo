using TurboLabz.InstantFramework;
using UnityEngine;

public struct StrengthVO
{
    public float strength;
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public GameObject fromIndicator;
    public GameObject toIndicator;
    public IAudioService audioService;
    public IAnalyticsService analyticsService;
    public AnalyticsContext analyticsContext;
    public IDownloadablesModel downloadablesModel;
    public string pieceName;
    public string activeSkinId;
}