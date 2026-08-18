using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000002 RID: 2
internal class SecurityHandler
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	public static string BuildKey()
	{
		byte[] r_key = new byte[12];
		Random rnd = new Random(DateTime.Now.Millisecond);
		for (int i = 0; i < r_key.Length; i++)
		{
			r_key[i] = Convert.ToByte(rnd.Next(97, 122));
		}
		return Encoding.UTF8.GetString(r_key);
	}

	// Token: 0x06000002 RID: 2 RVA: 0x000020A4 File Offset: 0x000002A4
	public static string Encrypt(string toEncrypt, string key, bool useHashing)
	{
		byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
		new AppSettingsReader();
		byte[] keyArray;
		if (useHashing)
		{
			MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
			keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
			md5CryptoServiceProvider.Clear();
		}
		else
		{
			keyArray = Encoding.UTF8.GetBytes(key);
		}
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		tripleDESCryptoServiceProvider.Key = keyArray;
		tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
		tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		byte[] resultArray = tripleDESCryptoServiceProvider.CreateEncryptor().TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		tripleDESCryptoServiceProvider.Clear();
		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}

	// Token: 0x06000003 RID: 3 RVA: 0x0000212C File Offset: 0x0000032C
	public static string Decrypt(string cipherString, string key, bool useHashing)
	{
		byte[] toEncryptArray = Convert.FromBase64String(cipherString);
		new AppSettingsReader();
		byte[] keyArray;
		if (useHashing)
		{
			MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
			keyArray = md5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
			md5CryptoServiceProvider.Clear();
		}
		else
		{
			keyArray = Encoding.UTF8.GetBytes(key);
		}
		TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
		tripleDESCryptoServiceProvider.Key = keyArray;
		tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
		tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
		byte[] resultArray = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
		tripleDESCryptoServiceProvider.Clear();
		return Encoding.UTF8.GetString(resultArray);
	}
}
