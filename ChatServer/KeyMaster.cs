using System.Linq;
using System.Security.Cryptography;

namespace ChatServer
{
	public class KeyMaster
	{
		public const int IterationCount = 65536;

		private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();

		private readonly ChatServerContainer container = new ChatServerContainer();

		public void CreateUser(string name, string password)
		{
			byte[] salt = new byte[PBKDF2_SHA256.StateSize];
			rng.GetBytes(salt);
			var hasher = new PBKDF2_SHA256(password, salt, IterationCount);

			var user = new User();
			user.Name = name;
			user.PasswordHash = hasher.GetBytes(PBKDF2_SHA256.StateSize * 2);
			user.PasswordSalt = (byte[])salt.Clone();

			container.Users.AddObject(user);
			container.SaveChanges();
		}

		public User ValidateUser(string name, string password)
		{
			var user = container.Users.Single(u => u.Name == name);
			var hasher = new PBKDF2_SHA256(password, user.PasswordSalt, IterationCount);

			byte[] passwordHash = hasher.GetBytes(PBKDF2_SHA256.StateSize * 2);
			if (passwordHash.SequenceEqual(user.PasswordHash))
				return user;
			else
				return null;
		}
	}
}
