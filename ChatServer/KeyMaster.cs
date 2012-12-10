using System.Linq;
using System.Security.Cryptography;

namespace ChatServer
{
	class KeyMaster
	{
		public const int IterationCount = 65536;

		private static RandomNumberGenerator rng = new RNGCryptoServiceProvider();

		private ChatServerContainer container = new ChatServerContainer();

		void CreateUser(string name, string password)
		{
			byte[] salt = new byte[PBKDF2_SHA256.StateSize];
			rng.GetBytes(salt);
			PBKDF2_SHA256 hasher = new PBKDF2_SHA256(password, salt, IterationCount);

			User user = new User();
			user.Name = name;
			user.PasswordHash = hasher.GetBytes(PBKDF2_SHA256.StateSize * 2);
			user.PasswordSalt = (byte[])salt.Clone();

			container.Users.AddObject(user);
			container.SaveChanges();
		}

		User ValidateUser(string name, string password)
		{
			User user = container.Users.Single(u => u.Name == name);
			PBKDF2_SHA256 hasher = new PBKDF2_SHA256(password, user.PasswordSalt, IterationCount);

			byte[] passwordHash = hasher.GetBytes(PBKDF2_SHA256.StateSize * 2);
			if (passwordHash.SequenceEqual(user.PasswordHash))
				return user;
			else
				return null;
		}
	}
}
