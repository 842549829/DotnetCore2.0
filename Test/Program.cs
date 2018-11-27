

using DotnetCore.Code.Code;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DotnetCore.Code.Encrypt;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            //IConfigurationBuilder config = new ConfigurationBuilder();
            //IConfigurationSource autofacJsonConfigSource = new JsonConfigurationSource()
            //{
            //    Path = "config.json",
            //};
            //config.Add(autofacJsonConfigSource);
            //var c = config.Build();
            //var d = c["ProviderName"];
            RSATest();
            Console.ReadLine();
            Console.ReadKey();
        }

        static void RSATest()
        {
            //2048 公钥
            string publicKey = @"MIGeMA0GCSqGSIb3DQEBAQUAA4GMADCBiAKBgGqucce3JH4mfxatBklmYhMLbU2R
mcJrSxAh3fNLCh7sJXh0nwXRsaWaW+TnqTKIFjD8/vl159dx9ClzeWUFXrNOm2fQ
u614f8Kt7oKb0KFyUqlAHTuisK6ZqW599WOe4rMYltnvHS6s/C3nQypPiM5E5zwq
PrMkrPib5n/LvQH5AgMBAAE=";
            //2048 私钥
            string privateKey =
                @"MIICXAIBAAKBgGqucce3JH4mfxatBklmYhMLbU2RmcJrSxAh3fNLCh7sJXh0nwXR
saWaW+TnqTKIFjD8/vl159dx9ClzeWUFXrNOm2fQu614f8Kt7oKb0KFyUqlAHTui
sK6ZqW599WOe4rMYltnvHS6s/C3nQypPiM5E5zwqPrMkrPib5n/LvQH5AgMBAAEC
gYA7Ut56zOFCNW4e0gDY+FI5fPU/WWRDtR58zhh6npP2NiNwJIn51m4PRRMs65Yv
P3X3r/iqCGLwb7HzCv/KqX3LSFcfzuN6OAVenaRfxl7JMJX95npwI3xlM0+KU3h1
Cstgu8LaCODw8qISqxiZU7IDIb3Y7jRLsriNE5IXu3X5sQJBAKsJaLIZzP3rrHv7
b3cIMHwcEvKw/DxupMD+8GzFsnTBR5YiieyoWe9PXt4AYR80HtP/kp7C7fo2bq1N
oWtfPk8CQQCfrQaUXhjRko0HMQx+SO+hrCGbyRfElDEhGOCrbJrNzEIvAakU87VU
mINN2l8BNxtBIbGbzLwnQhTY4h/yLrE3AkAzX5cf79n/5xse/m4DneUaUkBqvzh0
WnOIOMs0kMlCgo+jC+rLt+GTnQ6MtiZ1/ezIlrqOj1R11IW37lpu6uPHAkEAnqIS
WFJHi+VxsIRdKhUh5NeSqCeXMlgbmwsRIZ2LCgv0cKjQpx0buAiw4iahnh52ODXW
Mreq8cGn3nSAUmVq7wJBAIJnCtEG+Kwu8n5aJXo70XIGg4FHc8AEyrIIAW87t7hz
c6+ifWUF0a98jUQE5/WvktJp/vkVh7vMq+74usR85Pg=";

            var rsa = new RSA2(HashAlgorithmName.SHA256, Encoding.UTF8, privateKey, publicKey);

            string str = "博客园 http://www.cnblogs.com/";

            Console.WriteLine("原始字符串：" + str);

            //加密
            string enStr = rsa.Encrypt(str);

            Console.WriteLine("加密字符串：" + enStr);

            //解密
            string deStr = rsa.Decrypt(enStr);

            Console.WriteLine("解密字符串：" + deStr);

            //私钥签名
            string signStr = rsa.Sign(str);

            Console.WriteLine("字符串签名：" + signStr);

            //公钥验证签名
            bool signVerify = rsa.Verify(str, signStr);

            Console.WriteLine("验证签名：" + signVerify);

            Console.ReadKey();
        }
    }
}