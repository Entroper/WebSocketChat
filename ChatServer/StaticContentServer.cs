using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using WebSocketListener;

namespace ChatServer
{
	class StaticContentServer : IHttpServer
	{
		public void HandleContext(HttpListenerContext context)
		{
			string localPath = HttpUtility.UrlDecode(context.Request.Url.LocalPath);
			ServeFile(context, localPath);
		}

		public async void ServeFile(HttpListenerContext context, string localPath)
		{
			if (File.Exists(localPath))
			{
				var fs = new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read);
				var response = context.Response;

				response.ContentLength64 = fs.Length;
				response.ContentType = GetContentType(localPath);

				await fs.CopyToAsync(response.OutputStream);

				response.Close();
			}
		}

		static readonly Dictionary<string, string> MimeTypes;

		static StaticContentServer()
		{
			MimeTypes = new Dictionary<string, string>();
			MimeTypes["htm"] = "text/html";
			MimeTypes["html"] = "text/html";
			MimeTypes["css"] = "text/css";
			MimeTypes["js"] = "application/javascript";
			MimeTypes["jpg"] = "text/html";
			MimeTypes["jpeg"] = "text/html";
			MimeTypes["gif"] = "text/html";
			MimeTypes["png"] = "text/html";
		}

		private static string GetContentType(string localPath)
		{
			int suffixPos = localPath.LastIndexOf('.');
			if (suffixPos == -1)
				return "text/plain";

			string suffix = localPath.Substring(suffixPos + 1);
			if (MimeTypes.ContainsKey(suffix))
				return MimeTypes[suffix];
			else
				throw new NotImplementedException("File extension \"." + suffix + "\" is not implemented");
		}
	}
}
