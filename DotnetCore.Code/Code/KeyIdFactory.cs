using System;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// 主键工厂类
    /// </summary>
    public class KeyIdFactory
    {
        /// <summary>
        /// Fields
        /// </summary>
        private static long lastIdentity;

        /// <summary>
        /// Fields
        /// </summary>
        private static long lastIdentityTrain;

        /// <summary>
        /// locker
        /// </summary>
        private static readonly object lockerTrain = new object();

        /// <summary>
        /// locker
        /// </summary>
        private static readonly object locker = new object();

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <returns>主键ID</returns>
        public static string NewKeyId()
        {
            return NewKeyId(32, null);
        }

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <returns>主键ID</returns>
        public static string NewKeyId(DateTime time)
        {
            return NewKeyId(32, time);
        }

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <param name="code">业务码</param>
        /// <returns>主键ID</returns>
        public static string NewKeyId(string code)
        {
            return NewKeyId(32, null) + code;
        }

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <param name="length">ID长度(不能小于24)</param>
        /// <param name="time">创建时间</param>
        /// <returns>主键ID</returns>
        public static string NewKeyId(int length, DateTime? time)
        {
            if (length <= 24)
            {
                length = 24;
            }

            string strNum = string.Empty;
            for (int i = 24; i <= length; i++)
            {
                strNum += "9";
            }

            lock (locker)
            {
                string str = lastIdentity.ToString().PadLeft(strNum.Length, '0');
                string str2 = time?.ToString("yyyyMMddHHmmssfff") ?? DateTime.Now.ToString("yyyyMMddHHmmssfff");

                lastIdentity++;

                if (lastIdentity > long.Parse(strNum))
                {
                    lastIdentity = 0;
                }
                Random random = new Random(Common.CreateRandomSeed());
                var r = random.Next(1000000, 9999999);
                return $"{str2}{r}{str}";
            }
        }

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <param name="code">业务码</param>
        /// <param name="length">ID长度(不能小于24)</param>
        /// <returns>主键ID</returns>
        public static string NewKeyId(string code, int length)
        {
            if (length <= 24)
            {
                length = 24;
            }

            string strNum = string.Empty;
            for (int i = 24; i <= length; i++)
            {
                strNum += "9";
            }

            lock (locker)
            {
                string str = lastIdentity.ToString().PadLeft(strNum.Length, '0');
                string str2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");

                lastIdentity++;

                if (lastIdentity > long.Parse(strNum))
                {
                    lastIdentity = 0;
                }
                Random random = new Random(Common.CreateRandomSeed());
                var r = random.Next(1000000, 9999999);
                return $"{str2}{r}{str}{code}";
            }
        }

        /// <summary>
        /// 新的主键ID
        /// </summary>
        /// <param name="length">ID长度(不能小于24)</param>
        /// <param name="time">创建时间</param>
        /// <returns>主键ID</returns>
        public static string NewTrainKeyId(int length, DateTime? time)
        {
            if (length <= 24)
            {
                length = 24;
            }

            string strNum = string.Empty;
            for (int i = 24; i <= length; i++)
            {
                strNum += "9";
            }

            lock (lockerTrain)
            {
                string str = lastIdentityTrain.ToString().PadLeft(strNum.Length - 1, '0');
                string str2 = time?.ToString("yyyyMMddHHmmssfff") ?? DateTime.Now.ToString("yyyyMMddHHmmssfff");

                lastIdentityTrain++;

                if (lastIdentityTrain > long.Parse(strNum))
                {
                    lastIdentityTrain = 0;
                }
                Random random = new Random(Common.CreateRandomSeed());
                var r = random.Next(1000000, 9999999);
                if (length == 24)
                {
                    return $"{str2}{r}";
                }
                else
                {
                    return $"{str2}{r}{str}";
                }
            }
        }
    }
}