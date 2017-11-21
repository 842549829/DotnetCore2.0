using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// IPHelper
    /// </summary>	
    public class IPHelper
    {
        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns>System.String.</returns>
        public static string GetLocalHostIP()
        {
            //获取说有网卡信息
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    //获取以太网卡网络接口信息
                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    //获取单播地址集
                    UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                    foreach (UnicastIPAddressInformation ipadd in ipCollection)
                    {
                        if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                        //判断是否为ipv4
                        {
                            return ipadd.Address.ToString();//获取ip
                        }
                    }
                }
            }
            return null;
        }
    }
}