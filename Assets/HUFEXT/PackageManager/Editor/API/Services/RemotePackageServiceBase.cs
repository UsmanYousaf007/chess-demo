using System.Collections;
using System.Collections.Generic;
using HUFEXT.PackageManager.Editor.Implementation.Remote.Requests;
using UnityEngine;

namespace HUFEXT.PackageManager.Editor.API.Services
{
    public abstract class RemotePackageServiceBase
    {
        readonly protected List<BaseRequest> requests = new List<BaseRequest>();

        protected void EnqueueRequest( BaseRequest request )
        {
            request.OnComplete += ( x ) => DisposeRequest( request.ID );
            requests.Add( request );
            request.Send();
        }

        protected void DisposeRequest( System.Guid guid )
        {
            var id = requests.FindIndex( q => q.ID.Equals( guid ) );
            if ( id >= 0 )
            {
                requests[id].Dispose();
                requests.RemoveAt( id );
            }
        }
    }
}