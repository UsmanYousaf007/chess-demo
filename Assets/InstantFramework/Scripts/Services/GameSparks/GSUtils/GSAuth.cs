using System.Collections.Generic;
using TurboLabz.InstantFramework;
using GameSparks.Core;

// External auth types
public static class GSAuth
{
    private const string FACEBOOK = "FB";
    private const string GOOGLE_PLAY = "GP";

    private static readonly IDictionary<string, ExternalAuthType> externalIdMap =
        new Dictionary<string, ExternalAuthType>() {
        { FACEBOOK, ExternalAuthType.FACEBOOK },
        { GOOGLE_PLAY, ExternalAuthType.GOOGLE_PLAY }
    };

    public static IDictionary<ExternalAuthType, ExternalAuthData> GetExternalAuthentications(GSData externalIds)
    {
        IDictionary<ExternalAuthType, ExternalAuthData> externalAuthentications = new Dictionary<ExternalAuthType, ExternalAuthData>();
        IDictionary<string, object> externalIdsBaseData = externalIds.BaseData;

        foreach (KeyValuePair<string, object> e in externalIdsBaseData)
        {
            ExternalAuthType type = externalIdMap[e.Key];
            ExternalAuthData data;
            data.id = (string)e.Value;

            externalAuthentications.Add(type, data);
        }

        return externalAuthentications;
    }
}