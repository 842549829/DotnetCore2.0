using System;
using System.Security.Cryptography;
using System.Text;

namespace DotnetCore.Code.Encrypt
{
    /// <summary>
    /// SHA1
    /// </summary>	
    public class SHA
    {
        /// <summary>
        ///  对字符串进行SHA1加密
        /// </summary>
        /// <param name="source">数据</param>
        /// <returns>密文</returns>
        public static string SHA1Encrypt(string source)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] str1 = Encoding.UTF8.GetBytes(source);
            byte[] str2 = sha1.ComputeHash(str1);
            sha1.Clear();
            ((IDisposable)sha1).Dispose();
            return Convert.ToBase64String(str2);
        }

        /// <summary>
        /// SHA256加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string SHA256Encrypt(string str)
        {
            SHA256 s256 = new SHA256Managed();
            byte[] byte1;
            byte1 = s256.ComputeHash(Encoding.UTF8.GetBytes(str));
            s256.Clear();
            return Convert.ToBase64String(byte1);
        }

        /// <summary>
        /// SHA384加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string SHA384Encrypt(string str)
        {
            SHA384 s384 = new SHA384Managed();
            byte[] byte1;
            byte1 = s384.ComputeHash(Encoding.UTF8.GetBytes(str));
            s384.Clear();
            return Convert.ToBase64String(byte1);
        }

        /// <summary>
        /// SHA512加密，不可逆转
        /// </summary>
        /// <param name="str">string str:被加密的字符串</param>
        /// <returns>返回加密后的字符串</returns>
        public static string SHA512Encrypt(string str)
        {
            SHA512 s512 = new SHA512Managed();
            byte[] byte1;
            byte1 = s512.ComputeHash(Encoding.UTF8.GetBytes(str));
            s512.Clear();
            return Convert.ToBase64String(byte1);
        }
    }
}