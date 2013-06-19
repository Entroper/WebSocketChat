using System.Net;
using WebSocketListener;

namespace ChatServer
{
	class GateKeeper : IHttpServer
	{
		private readonly KeyMaster keyMaster = new KeyMaster();
		private readonly StaticContentServer staticContentServer = new StaticContentServer();

		public void HandleContext(HttpListenerContext context)
		{
			if (context.Request.IsAuthenticated)
			{
				var identity = context.User.Identity as HttpListenerBasicIdentity;
				var user = keyMaster.ValidateUser(identity.Name, identity.Password);
				if (user == null)
				{
					staticContentServer.ServeFile(context, "failedlogin.html");
				}
			}
		}
	}
}
