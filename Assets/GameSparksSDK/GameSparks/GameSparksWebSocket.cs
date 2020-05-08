using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using BestHTTP;
using BestHTTP.WebSocket;

namespace GameSparks
{
	/// <summary>
	/// Default WebSocket implementation used in the sdk. 
	/// </summary>
	public class GameSparksWebSocket : IGameSparksWebSocket
	{
		private Action<String> onMessage;
		private Action<byte[]> onBinaryMessage;
		private Action onClose;
		private Action onOpen;
		private Action<String> onError;

		protected WebSocket ws;

		public static System.Net.EndPoint Proxy { get; set; }

		private void Initialize(String url,
			Action onClose,
			Action onOpen,
			Action<String> onError)
		{

			this.onOpen = onOpen;
			this.onError = onError;
			this.onClose = onClose;

			ws = new WebSocket(new Uri(url));

			if (Proxy != null)
			{
				//ws.Proxy = new SuperSocket.ClientEngine.Proxy.HttpConnectProxy(Proxy);
				ws.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);

			}

#if !__IOS__
			//ws.NoDelay = true;
#endif

			ws.OnOpen += OnOpen;
			ws.OnClosed += OnClosed;
			ws.OnError += OnError;

			ws.StartPingThread = true;
			ws.PingFrequency = 30 * 1000; // TODO: GS default was 30 seconds!!!

			ws.Open();
		}

		void OnOpen(WebSocket ws)
		{
            onOpen?.Invoke();
        }

		void OnMessageReceived(WebSocket ws, string message)
		{
            onMessage?.Invoke(message);
        }

		void OnBinaryMessage(WebSocket ws, byte[] data)
		{
            onBinaryMessage?.Invoke(data);
        }

		void OnClosed(WebSocket ws, UInt16 code, string message)
		{
            onClose?.Invoke();
        }

		void OnError(WebSocket ws, string error)
		{
            onError?.Invoke(error);

			ws.Close();
        }


		/// <summary>
		/// 
		/// </summary>
		public void Initialize(String url,
			Action<String> onMessage,
			Action onClose,
			Action onOpen,
			Action<String> onError)
		{

			Initialize(url, onClose, onOpen, onError);

			this.onMessage = onMessage;

			ws.OnMessage = OnMessageReceived;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Initialize(String url,
			Action<byte[]> onBinaryMessage,
			Action onClose,
			Action onOpen,
			Action<String> onError)
		{

			Initialize(url, onClose, onOpen, onError);

			this.onBinaryMessage = onBinaryMessage;

			ws.OnBinary = OnBinaryMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Open()
		{
			GameSparks.Core.GameSparksUtil.Log("Opening Websocket");
			try
			{
				ws.Open();
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Close()
		{
			Terminate();
			GameSparks.Core.GameSparksUtil.Log("Closing Websocket");
			try
			{
				ws.Close(1000, "Bye! Socket closed!");
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Terminate()
		{
			GameSparks.Core.GameSparksUtil.Log("Closing Websocket");
			try
			{
				ws.OnOpen -= OnOpen;
				ws.OnClosed -= OnClosed;
				ws.OnError -= OnError;
				ws.OnMessage -= OnMessageReceived;
				ws.OnBinary -= OnBinaryMessage;

				ws.Close();

				if (onClose != null)
				{
					onClose();
				}
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void Send(String request)
		{
			try
			{
				ws.Send(request);
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

		public void SendBinary(byte[] request, int offset, int length)
		{
			try
			{
				List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();

				list.Add(new ArraySegment<byte>(request, offset, length));

				var binFormatter = new BinaryFormatter();
				var mStream = new MemoryStream();
				binFormatter.Serialize(mStream, list);

				ws.Send(mStream.GetBuffer());
			}
			catch (Exception e)
			{
				GameSparks.Core.GameSparksUtil.LogException(e);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public GameSparksWebSocketState State
		{
			get
			{
				try
				{
					switch (ws.State)
					{
						case WebSocketStates.Closed:
							return GameSparksWebSocketState.Closed;
						case WebSocketStates.Closing:
							return GameSparksWebSocketState.Closing;
						case WebSocketStates.Connecting:
							return GameSparksWebSocketState.Connecting;
						case WebSocketStates.Open:
							return GameSparksWebSocketState.Open;
					}
				}
				catch { }
				return GameSparksWebSocketState.None;
			}
		}
	}
}
