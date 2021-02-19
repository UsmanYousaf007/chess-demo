using System;
using UnityEngine;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public interface IRoutingTableBase { }

    [Serializable]
    public abstract class GameServerRoutingConfig<T> : GameServerBaseConfig where T : IRoutingTableBase
    {
        [Header( "Routing" )] [SerializeField] T routingTable = default;

        public T RoutingTable => routingTable;
    }
}
