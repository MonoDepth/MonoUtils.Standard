using System;
using System.Linq;

namespace MonoUtilities.Validation
{
	public static class ChecksumUtil
	{
		/// <summary>
		/// Calculates the checksum of a file
		/// </summary>
		/// <param name="filePath">path to the file to be checked</param>
		/// <param name="hashingAlgorithm">The hashing algo to be used. Defaults to SHA256</param>
		/// <returns>Byte array representing the checksum of the file</returns>
		/// <exception cref="ArgumentException">The hashing algo supplied is not supported by the OS/Language Version</exception>
		public static byte[] GetChecksum(string filePath, HashingAlgorithm hashingAlgorithm = HashingAlgorithm.SHA256)
		{
			using (var hasher = System.Security.Cryptography.HashAlgorithm.Create(hashingAlgorithm.ToString()))
			{
				if (hasher == null)
                {
					throw new ArgumentException($"{hashingAlgorithm} not a valid hashing algorithm");
                }
				using (var stream = System.IO.File.OpenRead(filePath))
				{
					return hasher.ComputeHash(stream);
				}
			}
		}

		/// <summary>
		/// Validates that the file has the same checksum signature as the supplied checksum
		/// </summary>
		/// <param name="filePath">Path to the file to check</param>
		/// <param name="checksum">The checksum we assume is correct</param>
		/// <param name="algorithm">The hasing algorithm used to produce the supplied checksum</param>
		/// <returns>True if the checksums are equal</returns>
		public static bool ValidateChecksum(string filePath, byte[] checksum, HashingAlgorithm algorithm = HashingAlgorithm.SHA256)
        {
			byte[] fileSum = GetChecksum(filePath, algorithm);
			return fileSum.SequenceEqual(checksum);
		}

		/// <summary>
		/// Converts a byte array to its string representation using a BitConverter
		/// </summary>
		/// <param name="checksum">The checksum in byte array form</param>
		/// <returns>A string representing the byte array</returns>
		public static string ChecksumToString(byte[] checksum)
        {
			return BitConverter.ToString(checksum).Replace("-", "");
		}

	}
	public enum HashingAlgorithm
	{
		MD5,
		SHA1,
		SHA256,
		SHA384,
		SHA512
	}
}
