using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.NetworkRequests
{
    public class TcpServer
    {
        readonly Func<string, string> responder;
        readonly TcpListener listener;

        /// <summary>
        /// Creates server instance
        /// </summary>
        /// <param name="port">A free port</param>
        /// <param name="responder">HTTP response parser. Receives a raw HTTP request and expects a raw HTTP response. </param>
        [PublicAPI]
        public TcpServer( int port, [NotNull] Func<string,string> responder )
        {
            this.responder = responder;
            listener = new TcpListener( IPAddress.Loopback, port );
            listener.Start();
            IntervalManager.Instance.EveryFrame += RunSynchronousServer;
        }

        void RunSynchronousServer()
        {
            if ( !listener.Pending() )
                return;

            TcpClient handler = listener.AcceptTcpClient();

            var stream = handler.GetStream();

            if ( stream.DataAvailable )
            {
                StringBuilder sb = new StringBuilder();

                byte[] buffer = new byte[handler.ReceiveBufferSize];

                if ( stream.CanRead )
                {
                    do
                    {
                        int bytesCount = stream.ReadAsync( buffer, 0, buffer.Length ).Result;
                        sb.Append( Encoding.UTF8.GetString( buffer, 0, bytesCount ) );
                    } while ( stream.DataAvailable );
                }

                byte[] msg = Encoding.ASCII.GetBytes( responder( sb.ToString() ) );
                stream.Write( msg, 0, msg.Length );
            }

            stream.Close();
            handler.Close();
        }

        ~TcpServer()
        {
            listener?.Stop();
        }
    }
}
