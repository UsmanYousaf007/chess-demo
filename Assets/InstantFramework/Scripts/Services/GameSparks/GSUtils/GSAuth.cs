using System.Collections.Generic;
using TurboLabz.InstantFramework;
using GameSparks.Core;

// External auth types
public static class GSAuth
{
    private const string FACEBOOK = "FB";
    private const string GOOGLE_PLAY = "GP";
    private const string APPLE = "AP";

    private static readonly IDictionary<string, ExternalAuthType> externalIdMap =
        new Dictionary<string, ExternalAuthType>() {
        { FACEBOOK, ExternalAuthType.FACEBOOK },
        { GOOGLE_PLAY, ExternalAuthType.GOOGLE_PLAY },
        { APPLE, ExternalAuthType.APPLE}
    };

    public static IDictionary<ExternalAuthType, ExternalAuth> GetExternalAuthentications(GSData externalIds)
    {
        IDictionary<ExternalAuthType, ExternalAuth> externalAuthentications = new Dictionary<ExternalAuthType, ExternalAuth>();
        IDictionary<string, object> externalIdsBaseData = externalIds.BaseData;

        foreach (KeyValuePair<string, object> e in externalIdsBaseData)
        {
            ExternalAuthType type = externalIdMap[e.Key];
            ExternalAuth data;
            data.id = (string)e.Value;

            externalAuthentications.Add(type, data);
        }

        return externalAuthentications;
    }
}