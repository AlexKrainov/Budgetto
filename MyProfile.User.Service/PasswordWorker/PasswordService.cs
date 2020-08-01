using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Linq;

namespace MyProfile.User.Service.PasswordWorker
{
	public class PasswordService : IPasswordService
	{
		private readonly Random Random = new Random();
		// character l removed to avoid ambiguity
		private readonly char[] AlphabetLowercase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
		// characters O, I removed to avoid ambiguity
		private readonly char[] AlphabetUppercase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
		// character 0 removed to avoid ambiguity
		private readonly char[] AlphabetNumbers = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		public string GeneratePassword(int length)
		{
			if (length < 3)
				throw new Exception("Wrong password length");

			var password = new char[length];

			var shuffledLowercase = ShuffleArray(AlphabetLowercase);
			var shuffledUppercase = ShuffleArray(AlphabetUppercase);
			var shuffledNumbers = ShuffleArray(AlphabetNumbers);

			var alphabetFullSet = AlphabetLowercase
				.Concat(AlphabetUppercase)
				.Concat(AlphabetNumbers).ToArray();

			var shuffledFullSet = ShuffleArray(alphabetFullSet);

			// fill from 1 char in every alphabet to guarantee passing validation
			password[0] = shuffledLowercase[Random.Next(shuffledLowercase.Length)];
			password[1] = shuffledUppercase[Random.Next(shuffledUppercase.Length)];
			password[2] = shuffledNumbers[Random.Next(shuffledNumbers.Length)];

			// fill from full alphabet 
			for (var i = 3; i < length; ++i)
			{
				password[i] = shuffledFullSet[Random.Next(shuffledFullSet.Length)];
			}

			// shuffle password array to add random behavior
			var result = ShuffleArray(password);
			return string.Join("", result);
		}

		private char[] ShuffleArray(char[] arr)
		{
			var shuffleArr = arr.ToArray();
			Random rng = new Random();
			int n = shuffleArr.Length;
			while (n > 1)
			{
				n--;
				int k = rng.Next(n + 1);
				var value = shuffleArr[k];
				shuffleArr[k] = shuffleArr[n];
				shuffleArr[n] = value;
			}
			return shuffleArr;
		}

		public string GenerateSalt(int length = 32)
		{
			var cryptoRandom = new SecureRandom();
			var salt = new byte[length];
			cryptoRandom.NextBytes(salt);
			return Convert.ToBase64String(salt);
		}

		public string GenerateHashSHA256(string password, string saltAsBase64String, int iterations = 10000, int hashByteSize = 32)
		{
			var saltBytes = Convert.FromBase64String(saltAsBase64String);
			var hash = GenerateHashSHA256(password, saltBytes, iterations, hashByteSize);
			return Convert.ToBase64String(hash);
		}

		public byte[] GenerateHashSHA256(string password, byte[] salt, int iterations = 10000, int hashByteSize = 32)
		{
			var pdb = new Pkcs5S2ParametersGenerator(new Org.BouncyCastle.Crypto.Digests.Sha256Digest());
			pdb.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(password.ToCharArray()), salt, iterations);
			var key = (KeyParameter)pdb.GenerateDerivedMacParameters(hashByteSize * 8);
			return key.GetKey();
		}
	}
}
