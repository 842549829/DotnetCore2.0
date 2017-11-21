using System;
using System.Security.Cryptography;
using System.Text;

namespace DotnetCore.Code.Encrypt
{
    /// <summary>
    /// AES
    /// </summary>
    public class AES
    {
        /// <summary>
        /// 默认密钥
        /// </summary>
        public const string DefaultKey = "ABCDEFGABCDEFG12ABCDEFGABCDEFG12";

        /// <summary>
        /// 获取Aes32位密钥
        /// </summary>
        /// <param name="key">Aes密钥字符串</param>
        /// <returns>Aes32位密钥</returns>
        private static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "Aes密钥不能为空");
            }
            if (key.Length < 32)
            {
                // 不足32补全
                key = key.PadRight(32, '0');
            }
            if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return Encoding.UTF8.GetBytes(key);
        }

        /// <summary>
        /// Aes加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptAes(string source, string key)
        {
            try
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.Key = GetAesKey(key);
                    aesProvider.Mode = CipherMode.ECB;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                    {
                        byte[] inputBuffers = Encoding.UTF8.GetBytes(source);
                        byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                        aesProvider.Clear();
                        aesProvider.Dispose();
                        return Convert.ToBase64String(results, 0, results.Length);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Aes解密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="key">aes密钥，长度必须32位</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptAes(string source, string key)
        {
            try
            {
                using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.Key = GetAesKey(key);
                    aesProvider.Mode = CipherMode.ECB;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                    {
                        byte[] inputBuffers = Convert.FromBase64String(source);
                        byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                        aesProvider.Clear();
                        return Encoding.UTF8.GetString(results);
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
