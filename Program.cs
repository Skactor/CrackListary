using System;
using System.Security.Cryptography;
using System.Text;

namespace CrackListary
{
    class Program
	{
		internal const int licenseLength = 192;
		private const string encryptKey = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
		internal static readonly char[] chars = encryptKey.ToCharArray();

		public static string GetUniqueKey(int size)
		{
			byte[] data = new byte[4 * size];
			using (var crypto = RandomNumberGenerator.Create())
			{
				crypto.GetBytes(data);
			}
			StringBuilder result = new StringBuilder(size);
			for (int i = 0; i < size; i++)
			{
				var rnd = BitConverter.ToUInt32(data, i * 4);
				var idx = rnd % chars.Length;

				result.Append(chars[idx]);
			}

			return result.ToString();
		}

		public static bool CheckLicense(string email, string license)
		{
			if (email == null || license == null)
			{
				return false;
			}
			if (license.Length != 192)
			{
				return false;
			}
			email = email.ToLowerInvariant();
			ulong num = ((ulong)En1(email) << 32) + En2(email);
			string text = "";
			for (int i = 0; i < 12; i++)
			{
				int index = (int)(num >> 64 - (i + 1) * 5) & 0x1F;
				text += encryptKey[index];
			}
			return license.Substring(160, 12) == text;
		}

		public static string GenerateLicense(string email)
		{
			if (email == null)
			{
				return "";
			}
			email = email.ToLowerInvariant();
			ulong num = ((ulong)En1(email) << 32) + En2(email);
			string text = "";
			for (int i = 0; i < 12; i++)
			{
				int index = (int)(num >> 64 - (i + 1) * 5) & 0x1F;
				text += encryptKey[index];
			}
			return text;
		}

		private static uint En1(string email)
		{
			uint num = 0u;
			foreach (char c in email)
			{
				num = 43 * num + c;
			}
			return num;
		}

		private static uint En2(string email)
		{
			uint num = 0u;
			foreach (char c in email)
			{
				num = (num << 4) + c;
				uint num2 = num & 0xF0000000u;
				if (num2 != 0)
				{
					num ^= num2 >> 24;
					num ^= num2;
				}
			}
			return num;
		}

		static void Main(string[] args)
        {
			Console.WriteLine("Enter email:");
			string email = Console.ReadLine();
			string licenseKey = GetUniqueKey(160) + GenerateLicense(email) + GetUniqueKey(20);
			Console.WriteLine(licenseKey);
			if (CheckLicense(email,licenseKey))
            {
				Console.WriteLine("check passed");
            }
            else
			{
				Console.WriteLine("license key validation failed");
			}
        }
    }
}
