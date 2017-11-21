using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// Common
    /// </summary>
    public class Common
    {
        /// <summary>
        /// 随机种子
        /// </summary>
        /// <returns>结构</returns>
        public static int CreateRandomSeed()
        {
            byte[] bytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        /// <summary>
        /// 计算时间相差月份
        /// </summary>
        /// <param name="d1">d1</param>
        /// <param name="d2">d2</param>
        /// <returns>结果</returns>
        public static int Calc(DateTime d1, DateTime d2)
        {
            DateTime max = d1 > d2 ? d1 : d2;
            DateTime min = d1 > d2 ? d2 : d1;
            int yeardiff = max.Year - min.Year;
            int monthdiff = max.Month - min.Month;
            return yeardiff * 12 + monthdiff + 1;
        }

        /// <summary>
        /// 手机号打码
        /// </summary>
        /// <param name="phoneNumber">原手机号</param>
        /// <returns>打码后的手机号</returns>
        public static string CellphoneMask(string phoneNumber)
        {
            return Regex.Replace(phoneNumber, "(\\d{3})\\d{4}(\\d{4})", "$1****$2");
        }

        /// <summary>
        /// 验证是否手机号
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>正确的手机号=true</returns>
        public static bool IsCellphone(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^1(3|4|5|7|8)[0-9]\d{8}$");
        }
    }
}