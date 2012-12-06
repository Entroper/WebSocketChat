using System;
using System.Net.WebSockets;
using WebSocketListener;

namespace ChatServer
{
    public class ChatSocket : IWebSocketServer
    {
		public string Protocol
		{
			get { return "chat"; }
		}

		public void HandleContext(HttpListenerWebSocketContext context)
		{
			throw new NotImplementedException();
		}
    }
}
