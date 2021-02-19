using System;
using HUF.Utils.Runtime.Configs.API;
using UnityEngine;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public class GameServerBaseConfig : FeatureConfigBase
    {
        [Header("URLs")]
        [SerializeField] string prodServerUrl = default;
        [SerializeField] string devServerUrl = default;

        [Header("Authorization")]
        [SerializeField] string productionKey = default;
        [SerializeField] string developmentKey = default;

        [Header("Common")]
        [Tooltip("Always use production server url and validation token.")]
        [SerializeField] bool forceProdEnvironment = default;
        [Tooltip("Connection time out in seconds.")]
        [SerializeField] int connectionTimeOut = 15;
        [SerializeField] int errorRetryCount = 5;
        [Tooltip("Timeout between failed requests retries")]
        [SerializeField] float errorRetryWait = 2.5f;

        public string ServerUrl => Debug.isDebugBuild && forceProdEnvironment == false ? devServerUrl : prodServerUrl;
        public bool ForceProdEnvironment => forceProdEnvironment;
        public int ConnectionTimeOutInSeconds => connectionTimeOut;
        public int ErrorRetryCount => errorRetryCount;
        public float ErrorRetryWait => errorRetryWait;
        public string ValidationToken => Debug.isDebugBuild && !ForceProdEnvironment ? developmentKey : productionKey;
    }
}