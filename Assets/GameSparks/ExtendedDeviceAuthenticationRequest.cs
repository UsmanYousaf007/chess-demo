using System.Collections.Generic;
using GameSparks.Api.Requests;

public class ExtendedDeviceAuthenticationRequest : DeviceAuthenticationRequest
{
    public ExtendedDeviceAuthenticationRequest SetDeviceId(string id)
    {
        Dictionary<string, object> req = (Dictionary<string, object>)JSONData;
        req.Remove("deviceId");
        req.Add("deviceId", id);
        return this;
    }
}
