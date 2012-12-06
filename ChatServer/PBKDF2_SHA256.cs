using System.Security.Cryptography;

namespace ChatServer
{
	class PBKDF2_SHA256 : DeriveBytes
	{
		public override byte[] GetBytes(int cb)
		{
			throw new System.NotImplementedException();
		}

		public override void Reset()
		{
			throw new System.NotImplementedException();
		}
	}
}
