﻿using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocketListener;

namespace WebSocketApplication
{
	class Program
	{
		static void Main(string[] args)
		{
			var httpServer = new HelloWorldServer();
			var webSocketServer = new EchoSocket();
			var listener = new HttpWebSocketListener(httpServer, webSocketServer);

			Task.Run(() => listener.Run());

			while (true)
				Thread.Yield();
		}
	}
}
