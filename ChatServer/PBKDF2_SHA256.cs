using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatServer
{
	class PBKDF2_SHA256 : DeriveBytes
	{
		public const int StateSize = 32;

		public byte[] Message { get; set; }
		public byte[] Salt { get; set; }
		public int IterationCount { get; set; }

		private SHA256 hasher;

		private byte[] lastSalt;
		private byte[] state;

		public PBKDF2_SHA256(string message, byte[] salt, int iterationCount)
		{
			Message = Encoding.UTF8.GetBytes(message);
			Salt = (byte[])salt.Clone();
			IterationCount = iterationCount;

			hasher = new SHA256Cng();
			Reset();
		}

		public PBKDF2_SHA256(byte[] message, byte[] salt, int iterationCount)
		{
			Message = (byte[])message.Clone();
			Salt = (byte[])salt.Clone();
			IterationCount = iterationCount;

			hasher = new SHA256Cng();
			Reset();
		}
		
		public override byte[] GetBytes(int cb)
		{
			if (cb % StateSize != 0 || cb <= 0)
				throw new ArgumentOutOfRangeException("cb", cb, String.Format("Must request a multiple of {0} bytes", StateSize));

			byte[] returnBuffer = new byte[cb];
			for (int byteBlock = 0; byteBlock < cb / StateSize; byteBlock++)
			{
				for (int iteration = 0; iteration < IterationCount; iteration++)
				{
					byte[] toHash = new byte[Message.Length + lastSalt.Length + 4];

					byte[] iterationBytes = BitConverter.GetBytes(iteration);
					if (BitConverter.IsLittleEndian)
						ReverseBytes(iterationBytes);

					iterationBytes.CopyTo(toHash, Message.Length + lastSalt.Length);

					lastSalt = hasher.ComputeHash(toHash);
					for (int i = 0; i < StateSize; i++)
						state[i] ^= lastSalt[i];
				}

				state.CopyTo(returnBuffer, byteBlock * StateSize);
			}

			return returnBuffer;
		}

		public override void Reset()
		{
			state = new byte[StateSize];
			lastSalt = (byte[])Salt.Clone();
		}

		private void ReverseBytes(byte[] toReverse)
		{
			for (int i = 0; i < toReverse.Length / 2; i++)
			{
				byte temp = toReverse[i];
				toReverse[i] = toReverse[toReverse.Length - 1 - i];
				toReverse[toReverse.Length - 1 - i] = temp;
			}
		}
	}
}
