using TurboLabz.InstantFramework;
using UnityEngine;

public struct StrengthVO
{
    public float strength;
    public Vector3 fromPosition;
    public Vector3 toPosition;
    public GameObject fromIndicator;
    public GameObject toIndicator;
    public IAnalyticsService analyticsService;
    public AnalyticsContext analyticsContext;
    public string pieceName;
    public string activeSkinId;
}