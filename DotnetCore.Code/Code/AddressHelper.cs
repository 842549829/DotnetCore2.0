using DotnetCore.Code.Extension;
using System;
using System.Text;
using DotnetCore.Code.Utility;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// AddressHelper
    /// </summary>	
    public class AddressHelper
    {
        /// <summary>
        /// 获取地址信息
        /// </summary>
        /// <param name="address">地理位置</param>
        /// <returns>百度地图位置坐标</returns>
        public static AddressInfo GetAddressInfo(string address)
        {
            AddressInfo addInfo = new AddressInfo();
            try
            {
                string requstUrl = "http://apis.map.qq.com/jsapi?qt=poi&wd=" + address;
                var helper = new HttpHelper();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var addressInfo = helper.HttpGet(requstUrl, string.Empty, Encoding.GetEncoding("gb18030"), false, false, 20000).DeserializeObject();
                string pointX;
                string pointY;
                if (addressInfo["detail"].Property("pois") != null)
                {
                    pointX = addressInfo["detail"]["pois"][0]["pointx"].Value + "0000";
                    pointY = addressInfo["detail"]["pois"][0]["pointy"].Value + "0000";
                }
                else
                {
                    pointX = addressInfo["detail"]["city"]["pointx"].Value + "0000";
                    pointY = addressInfo["detail"]["city"]["pointy"].Value + "0000";
                }
                requstUrl = "http://www.gpsspg.com/apis/maps/geo/?output=jsonp&lat=" + pointY + "&lng=" + pointX +
                            "&type=3";
                string referer = "http://www.gpsspg.com/iframe/maps/qq_161128.htm?mapi=2";
                for (int i = 0; i < 3; i++)
                {
                    helper.UserAgent = HttpHelper.RandomUserAgent();
                    var mapsInfo = helper.HttpGet(requstUrl, referer, Encoding.GetEncoding("utf-8"), false, false,
                        20000);
                    mapsInfo = mapsInfo.Substring(mapsInfo.IndexOf("(", StringComparison.Ordinal) + 1).TrimEnd(')');
                    var newMapsInfo = mapsInfo.DeserializeObject();
                    if (newMapsInfo["msg"].Value.Equals("ok"))
                    {
                        addInfo.IsSuccessed = true;
                        addInfo.Latitude = newMapsInfo["result"][0]["lat"][2].Value.ToString();
                        addInfo.Longitude = newMapsInfo["result"][0]["lng"][2].Value.ToString();
                        break;
                    }
                }
            }
            catch (Exception)
            {
                return addInfo;
            }
            return addInfo;
        }
    }

    /// <summary>
    /// 地址经纬度
    /// </summary>
    public class AddressInfo
    {

        /// <summary>
        /// 是否请求成功
        /// </summary>
        public bool IsSuccessed { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
    }
}