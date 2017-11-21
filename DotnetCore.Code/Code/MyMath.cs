using System;
using System.Collections.Generic;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// MyMath
    /// </summary>	
    public static class MyMath
    {
        /// <summary>
        /// 均分总额，若不能均分则将余数加到最后一个上面
        /// </summary>
        /// <param name="totalSum">总额</param>
        /// <param name="count">要均分的份数</param>
        /// <returns>均分结果集合</returns>
        public static List<decimal> AvgSplit(this decimal totalSum, uint count)
        {
            List<decimal> res = new List<decimal>();
            if (count == 0)
            {
                throw new DivideByZeroException("除数不能为零");
            }
            decimal avg = (totalSum / count).Rounded(2);
            for (int i = 0; i < count; i++)
            {
                if (i != count - 1)
                {
                    res.Add(avg);
                }
                else
                {
                    res.Add(totalSum - avg * (count - 1));
                }
            }
            return res;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="precision">精度</param>
        /// <returns>结果</returns>
        public static decimal Rounded(this decimal value, int precision)
        {
            return Math.Round(value, precision, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 全舍
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="precision">精度</param>
        /// <returns>结果</returns>
        public static decimal Floor(this decimal value, int precision)
        {
            var tmp = (decimal)Math.Pow(10, precision);
            return Math.Floor(value * tmp) / tmp;
        }

        /// <summary>
        /// 全入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="precision">精度</param>
        /// <returns>结果</returns>
        public static decimal Ceiling(this decimal value, int precision)
        {
            var tmp = (decimal)Math.Pow(10, precision);
            return Math.Ceiling(value * tmp) / tmp;
        }
    }
}