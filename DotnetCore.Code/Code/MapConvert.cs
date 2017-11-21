using System;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// 地图坐标转换
    /// WGS坐标系是国际通用的一种地心坐标系，WGS本身也有多个版本（不赘述了），GCJ-02是国内官方采用的一种坐标系，国内许多坐标系也是基于GCJ-02变种而来的，比如百度坐标系BD-09
    /// </summary>
    public class MapConvert
    {
        /// <summary>
        /// 圆周率
        /// </summary>
        static readonly double pi = 3.14159265358979324;

        /// <summary>
        /// 卫星椭球坐标投影到平面地图坐标系的投影因子。
        /// </summary>
        static readonly double a = 6378245.0;

        /// <summary>
        /// 椭球的偏心率。
        /// </summary>
        static readonly double ee = 0.00669342162296594323;

        /// <summary>
        ///  圆周率转换量
        /// </summary>
        public static readonly double XPi = 3.14159265358979324 * 3000.0 / 180.0;

        /// <summary>
        /// wgs坐标转换为百度坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double[] Wgs2Bd(double lat, double lon)
        {
            double[] wgs2Gcj = Wgs2Gcj(lat, lon);

            double[] gcj2Bd = Gcj2Bd(wgs2Gcj[0], wgs2Gcj[1]);

            return gcj2Bd;
        }

        /// <summary>
        /// GCJ坐标转换为百度坐标。
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double[] Gcj2Bd(double lat, double lon)
        {

            double x = lon, y = lat;

            double z = Math.Sqrt(x * x + y * y) + 0.00002 * Math.Sin(y * XPi);

            double theta = Math.Atan2(y, x) + 0.000003 * Math.Cos(x * XPi);

            double bdLon = z * Math.Cos(theta) + 0.0065;

            double bdLat = z * Math.Sin(theta) + 0.006;

            return new[] { bdLat, bdLon };

        }

        /// <summary>
        /// 百度坐标转换为GCJ坐标。
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double[] Bd2Gcj(double lat, double lon)
        {

            double x = lon - 0.0065, y = lat - 0.006;

            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * XPi);

            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * XPi);

            double ggLon = z * Math.Cos(theta);

            double ggLat = z * Math.Sin(theta);

            return new[] { ggLat, ggLon };

        }

        /// <summary>
        /// WGS坐标转换为GCJ坐标。
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public static double[] Wgs2Gcj(double lat, double lon)
        {

            double dLat = TransformLat(lon - 105.0, lat - 35.0);

            double dLon = TransformLon(lon - 105.0, lat - 35.0);

            double radLat = lat / 180.0 * pi;

            double magic = Math.Sin(radLat);

            magic = 1 - ee * magic * magic;

            double sqrtMagic = Math.Sign(magic);

            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);

            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);

            double mgLat = lat + dLat;

            double mgLon = lon + dLon;

            double[] loc = { mgLat, mgLon };

            return loc;

        }

        /// <summary>
        /// 转换方法，比较复杂，不必深究了。输入：横纵坐标，输出：转换后的横坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private static double TransformLat(double lat, double lon)
        {

            double ret = -100.0 + 2.0 * lat + 3.0 * lon + 0.2 * lon * lon + 0.1 * lat * lon + 0.2 * Math.Sqrt(Math.Abs(lat));

            ret += (20.0 * Math.Sin(6.0 * lat * pi) + 20.0 * Math.Sin(2.0 * lat * pi)) * 2.0 / 3.0;

            ret += (20.0 * Math.Sin(lon * pi) + 40.0 * Math.Sin(lon / 3.0 * pi)) * 2.0 / 3.0;

            ret += (160.0 * Math.Sin(lon / 12.0 * pi) + 320 * Math.Sin(lon * pi / 30.0)) * 2.0 / 3.0;

            return ret;

        }

        /// <summary>
        /// 转换方法，同样复杂，自行脑补吧。输入：横纵坐标，输出：转换后的纵坐标。
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        private static double TransformLon(double lat, double lon)
        {

            double ret = 300.0 + lat + 2.0 * lon + 0.1 * lat * lat + 0.1 * lat * lon + 0.1 * Math.Sqrt(Math.Abs(lat));

            ret += (20.0 * Math.Sin(6.0 * lat * pi) + 20.0 * Math.Sin(2.0 * lat * pi)) * 2.0 / 3.0;

            ret += (20.0 * Math.Sin(lat * pi) + 40.0 * Math.Sin(lat / 3.0 * pi)) * 2.0 / 3.0;

            ret += (150.0 * Math.Sin(lat / 12.0 * pi) + 300.0 * Math.Sin(lat / 30.0 * pi)) * 2.0 / 3.0;

            return ret;

        }

        private static double[] delta(double lat, double lon)
        {
            var a = 6378245.0; //  a: 卫星椭球坐标投影到平面地图坐标系的投影因子。
            var ee = 0.00669342162296594323; //  ee: 椭球的偏心率。
            var dLat = TransformLat(lon - 105.0, lat - 35.0);
            var dLon = TransformLon(lon - 105.0, lat - 35.0);
            var radLat = lat / 180.0 * pi;
            var magic = Math.Sin(radLat);
            magic = 1 - ee * magic * magic;
            var sqrtMagic = Math.Sqrt(magic);
            dLat = (dLat * 180.0) / ((a * (1 - ee)) / (magic * sqrtMagic) * pi);
            dLon = (dLon * 180.0) / (a / sqrtMagic * Math.Cos(radLat) * pi);
            return new[] { dLat, dLon };
        }
    }
}