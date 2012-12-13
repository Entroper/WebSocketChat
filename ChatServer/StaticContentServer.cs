using System.IO;
using System.Net;
using System.Web;
using WebSocketListener;

namespace ChatServer
{
	class StaticContentServer : IHttpServer
	{
		private const int BufferSize = 65536;
		byte[] fileBuffer = new byte[BufferSize];

		public async void HandleContext(HttpListenerContext context)
		{
			string localPath = HttpUtility.UrlDecode(context.Request.Url.LocalPath);
			if (File.Exists(localPath))
			{
				FileStream fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);

				context.Response.ContentLength64 = fs.Length;
				context.Response.ContentType = "text/html";

				while(fs.Position != fs.Length)
				{
					int bytesRead = await fs.ReadAsync(fileBuffer, 0, BufferSize);
					await context.Response.OutputStream.WriteAsync(fileBuffer, 0, bytesRead);
				}

				context.Response.Close();
			}
		}
	}
}
