using System;
using System.Collections.Generic;

namespace DotnetCore.Code.Utility
{
    /// <summary>
    /// LatLonUtil
    /// </summary>
    public class LatLonUtil
    {
        private const double PI = 3.14159265;

        private static readonly double RAD = Math.PI / 180.0;

        /// <summary>
        /// 根据提供的经度和纬度、以及半径，取得此半径内的最大最小经纬度
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <param name="raidus">半径(米)</param>
        /// <returns></returns>
        public static double[] GetAround(double lat, double lon, int raidus)
        {

            double latitude = lat;
            double longitude = lon;

            double degree = (24901 * 1609) / 360.0;
            double raidusMile = raidus;

            double dpmLat = 1 / degree;
            double radiusLat = dpmLat * raidusMile;
            double minLat = latitude - radiusLat;
            double maxLat = latitude + radiusLat;

            double mpdLng = degree * Math.Cos(latitude * (PI / 180));
            double dpmLng = 1 / mpdLng;
            double radiusLng = dpmLng * raidusMile;
            double minLng = longitude - radiusLng;
            double maxLng = longitude + radiusLng;
            return new double[] { minLat, minLng, maxLat, maxLng };
        }

        private static readonly double EARTH_RADIUS = 6378.137;//地球半径

        /// <summary>
        /// 根据提供的两个经纬度计算距离(米)
        /// </summary>
        /// <param name="lng1">经度1</param>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng2">经度2</param>
        /// <param name="lat2">纬度2</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);

            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s * 1000;
        }

        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 计算经纬度区域的中心点
        /// </summary>
        /// <param name="pointsList">经纬度集合</param>
        /// <returns>中心点的经纬度</returns>
        public static GpsPoint GetCenterPos(IList<GpsPoint> pointsList)
        {
            int total = pointsList.Count;
            double X = 0, Y = 0, Z = 0;
            foreach (GpsPoint point in pointsList)
            {
                var lon = point.Longitude * Math.PI / 180;
                var lat = point.Latitude * Math.PI / 180;
                var x = Math.Cos(lat) * Math.Cos(lon);
                var y = Math.Cos(lat) * Math.Sin(lon);
                var z = Math.Sin(lat);
                X += x;
                Y += y;
                Z += z;
            }
            X /= total;
            Y /= total;
            Z /= total;

            var Lon = Math.Atan2(Y, X);
            var Hyp = Math.Sqrt(X * X + Y * Y);
            var Lat = Math.Atan2(Z, Hyp);
            return new GpsPoint { Longitude = Lon * 180 / Math.PI, Latitude = Lat * 180 / Math.PI };
        }

        /// <summary>
        /// GPS坐标点
        /// </summary>
        public class GpsPoint
        {
            /// <summary>
            /// 经度
            /// </summary>
            public double Longitude { get; set; }

            /// <summary>
            /// 纬度
            /// </summary>
            public double Latitude { get; set; }
        }
    }
}