using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

public static class EncryptionHelper
{
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("Moja tajna sól"); // Zast¹p to w³asn¹ unikaln¹ wartoœci¹

    public static string EncryptString(string plainText, string password)
    {
        using (var aes = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(password, Salt, 1000);
            aes.Key = key.GetBytes(32); // 256-bitowy klucz
            aes.IV = key.GetBytes(16); // 128-bitowy wektor inicjalizacji
            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string DecryptString(string cipherText, string password)
    {
        using (var aes = Aes.Create())
        {
            var key = new Rfc2898DeriveBytes(password, Salt, 1000);
            aes.Key = key.GetBytes(32); // 256-bitowy klucz
            aes.IV = key.GetBytes(16); // 128-bitowy wektor inicjalizacji
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
