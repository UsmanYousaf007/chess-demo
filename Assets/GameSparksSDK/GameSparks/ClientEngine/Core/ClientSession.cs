using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SuperSocket.ClientEngine
{
    public abstract class ClientSession : IClientSession, IBufferSetter
    {
        protected Socket Client { get; set; }

        protected EndPoint RemoteEndPoint { get; set; }

        public bool IsConnected { get; private set; }

#if !__IOS__
        public bool NoDeplay { get; set; }
#endif

        public ClientSession()
        {

        }

        public ClientSession(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint == null)
                throw new ArgumentNullException("remoteEndPoint");

            RemoteEndPoint = remoteEndPoint;
        }

        public int SendingQueueSize { get; set; }

        public abstract void Connect();

        public abstract bool TrySend(ArraySegment<byte> segment);

        public abstract bool TrySend(IList<ArraySegment<byte>> segments);

        public void Send(byte[] data, int offset, int length)
        {
            this.Send(new ArraySegment<byte>(data, offset, length));
        }

#if NO_SPINWAIT_CLASS
        public void Send(ArraySegment<byte> segment)
        {
            if (TrySend(segment))
                return;

            while (true)
            {
                Thread.SpinWait(1);

                if (TrySend(segment))
                    return;
            }
        }

        public void Send(IList<ArraySegment<byte>> segments)
        {
            if (TrySend(segments))
                return;

            while (true)
            {
                Thread.SpinWait(1);

                if (TrySend(segments))
                    return;
            }
        }
#else
        public void Send(ArraySegment<byte> segment)
        {
            if (TrySend(segment))
                return;

            var spinWait = new SpinWait();

            while (true)
            {
                spinWait.SpinOnce();

                if (TrySend(segment))
                    return;
            }
        }

        public void Send(IList<ArraySegment<byte>> segments)
        {
            if (TrySend(segments))
                return;

            var spinWait = new SpinWait();

            while (true)
            {
                spinWait.SpinOnce();

                if (TrySend(segments))
                    return;
            }
        }
#endif

        public abstract void Close();

        private EventHandler m_Closed;

        public event EventHandler Closed
        {
            add { m_Closed += value; }
            remove { m_Closed -= value; }
        }

        protected virtual void OnClosed()
        {
            IsConnected = false;

            var handler = m_Closed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private EventHandler<ErrorEventArgs> m_Error;

        public event EventHandler<ErrorEventArgs> Error
        {
            add { m_Error += value; }
            remove { m_Error -= value; }
        }

        protected virtual void OnError(Exception e)
        {
			var handler = m_Error;
            if (handler == null)
                return;

            handler(this, new ErrorEventArgs(e));
        }

        private EventHandler m_Connected;

        public event EventHandler Connected
        {
            add { m_Connected += value; }
            remove { m_Connected -= value; }
        }

        protected virtual void OnConnected()
        {
            var client = Client;

#if !__IOS__
            if(client != null)
            {
                if(client.NoDelay != NoDeplay)
                    client.NoDelay = NoDeplay;
            }
#endif

            IsConnected = true;

            var handler = m_Connected;
            if (handler == null)
                return;

            handler(this, EventArgs.Empty);
        }

        private EventHandler<DataEventArgs> m_DataReceived;

        public event EventHandler<DataEventArgs> DataReceived
        {
            add { m_DataReceived += value; }
            remove { m_DataReceived -= value; }
        }

        private DataEventArgs m_DataArgs = new DataEventArgs();

        protected virtual void OnDataReceived(byte[] data, int offset, int length)
        {
            var handler = m_DataReceived;
            if (handler == null)
                return;

            m_DataArgs.Data = data;
            m_DataArgs.Offset = offset;
            m_DataArgs.Length = length;

            handler(this, m_DataArgs);
        }

        public virtual int ReceiveBufferSize { get; set; }

        public IProxyConnector Proxy { get; set; }

        protected ArraySegment<byte> Buffer { get; set; }

        void IBufferSetter.SetBuffer(ArraySegment<byte> bufferSegment)
        {
            SetBuffer(bufferSegment);
        }

        protected virtual void SetBuffer(ArraySegment<byte> bufferSegment)
        {
            Buffer = bufferSegment;
        }
    }
}
