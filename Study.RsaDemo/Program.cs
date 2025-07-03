using Microsoft.VisualBasic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Simple.Common.Checksum;

namespace Study.RsaDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);
            var keyGen = RandomNumberGenerator.Create();
            var key = new Byte[4096];
            keyGen.GetBytes(key);

            CRC crc = new CRC();
            crc.Update(key, 0, (uint)key.Length);
            var dig1 = crc.GetDigest();

            Crc32 crc2 = new Crc32();
            crc2.Update(key);
            var dig2 = crc2.Value;


            var input = new Byte[1024];
            keyGen.GetBytes(input);
            var keyTxt = Convert.ToBase64String(key);
            var raw = "aaa-" + Convert.ToBase64String(input);
            var e1= AESUtils.Encrypt(raw, keyTxt);
            var d1 = AESUtils.Decrypt(e1, keyTxt);

            Console.WriteLine("Hello World!");
        }

        public static string Encrypt(string data, string publicKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(publicKey);
                encryptedData = rsa.Encrypt(dataToEncrypt, false);
            }
            return Convert.ToBase64String(encryptedData);
        }

        public static string AesEncrypt(string data, string publicKey)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(data);
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.Zeros;
                
                aes.Key = Encoding.UTF8.GetBytes("github.com/yuanrui");
                aes.IV = Encoding.UTF8.GetBytes("github.com/yuanrui");


                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(plaintext, 0, plaintext.Length);
                        cs.Close();
                    }
                    encrypted = ms.ToArray();

                    return Convert.ToBase64String(encrypted);
                }
            }
        }

        public static string AesDecrypt(string data, string publicKey)
        {
            byte[] encrypted = Encoding.UTF8.GetBytes(data);

            using (Aes aes = Aes.Create())
            {
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.Zeros;

                aes.Key = Encoding.UTF8.GetBytes("github.com/yuanrui");
                aes.IV = Encoding.UTF8.GetBytes("github.com/yuanrui");


                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader reader = new StreamReader(cs))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string SignHash(string data, string privateKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(privateKey);
                encryptedData = rsa.SignHash(dataToEncrypt, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            return Convert.ToBase64String(encryptedData);
        }

        public static bool VerifyHash(string data, string sig, string publicKey)
        {
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(data);
            byte[] encryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(publicKey);
                return rsa.VerifyHash(dataToEncrypt, Encoding.UTF8.GetBytes(sig), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static string Decrypt(string data, string privateKey)
        {
            byte[] dataToDecrypt = Convert.FromBase64String(data);
            byte[] decryptedData;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                rsa.FromXmlString(privateKey);
                
                decryptedData = rsa.Decrypt(dataToDecrypt, false);
            }
            return Encoding.UTF8.GetString(decryptedData);
        }
    }

    public class AESUtils
    {
        /// AES解密
        /// </summary>
        /// <param name="decryptStr">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Decrypt(byte[] ciphertext, byte[] key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
            return resultArray;
        }

        /// AES加密
        /// </summary>
        /// <param name="encryptStr">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] plaintext, byte[] key)
        {
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = key;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.Zeros;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);
            return resultArray;
        }

        public static string Decrypt(string ciphertext, string key)
        {
            byte[] keyArray = Convert.FromBase64String(key);
            byte[] toEncryptArray = Convert.FromBase64String(ciphertext);
            Aes aes = Aes.Create();
            aes.Key = keyArray;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = aes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        public static string Encrypt(string rawText, string key)
        {
            byte[] keyArray = Convert.FromBase64String(key);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(rawText);
            Aes aes = Aes.Create();
            
            aes.Key = keyArray;
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = aes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray);
        }
    }
}
