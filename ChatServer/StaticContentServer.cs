using System;
using System.Collections.Generic;
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
				context.Response.ContentType = GetContentType(localPath);

				while(fs.Position != fs.Length)
				{
					int bytesRead = await fs.ReadAsync(fileBuffer, 0, BufferSize);
					await context.Response.OutputStream.WriteAsync(fileBuffer, 0, bytesRead);
				}

				context.Response.Close();
			}
		}

		static Dictionary<string, string> mimeTypes;

		static StaticContentServer()
		{
			mimeTypes = new Dictionary<string, string>();
			mimeTypes["htm"] = "text/html";
			mimeTypes["html"] = "text/html";
			mimeTypes["css"] = "text/css";
			mimeTypes["js"] = "application/javascript";
			mimeTypes["jpg"] = "text/html";
			mimeTypes["jpeg"] = "text/html";
			mimeTypes["gif"] = "text/html";
			mimeTypes["png"] = "text/html";
		}

		private static string GetContentType(string localPath)
		{
			int suffixPos = localPath.LastIndexOf('.');
			if (suffixPos == -1)
				return "text/plain";

			string suffix = localPath.Substring(suffixPos + 1);
			if (mimeTypes.ContainsKey(suffix))
				return mimeTypes[suffix];
			else
				throw new NotImplementedException("File extension \"." + suffix + "\" is not implemented");
		}
	}
}
