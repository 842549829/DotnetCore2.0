using DotnetCore.Code.Code;
using DotnetCore.Code.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace DotnetCore.Code.Utility
{
    /// <summary>
    /// HttpClientUtility
    /// </summary>
    public class HttpClientUtility
    {
        /// <summary>
        /// 将对象属性转换为key-value对
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>字典</returns>
        private static Dictionary<string, string> ToMap<T>(T obj)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Type t = typeof(T);
            foreach (PropertyInfo property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                string name = property.Name;
                DynamicExpressionAttribute dynamicExpressionAttribute = CustomAttributeExtension<DynamicExpressionAttribute>.GetCustomAttributeValue(t, property);
                string key = dynamicExpressionAttribute.Name ?? name;
                var val = property.GetValue(obj);
                string value;
                if (dynamicExpressionAttribute.IsDynamicExpression)
                {
                    if (!string.IsNullOrWhiteSpace(dynamicExpressionAttribute.Operator))
                    {
                        if (dynamicExpressionAttribute.Operator == "yyyy-MM-dd")
                        {
                            if (val != null)
                            {
                                value = Convert.ToDateTime(val).ToString(dynamicExpressionAttribute.Operator);
                            }
                            else
                            {
                                value = null;
                            }
                        }
                        else
                        {
                            value = val?.ToString();
                        }
                    }
                    else
                    {
                        value = val?.ToString();
                    }
                }
                else
                {
                    value = val.SerializeObject();
                }
                if (value != null)
                {
                    result.Add(key, value);
                }
            }
            if (result.Count <= 0)
            {
                var str = obj.SerializeObject();
                string pattern = "(\"(?<key>[^,^\"]+)\":\"(?<value>[^:^\"]+)\")|(\"(?<key>[^,^\"]+)\":(?<value>[\\d\\.]+))";
                Regex regex = new Regex(pattern);
                var match = regex.Matches(str);
                foreach (Match item in match)
                {
                    var key = item.Groups["key"].ToString();
                    var value = item.Groups["value"].ToString();
                    result.Add(key, value);
                }
            }
            return result;
        }

        /// <summary>
        /// POST请求(FormUrlEncodedContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V PostFormUrl<T, V>(string url, T t)
        {
            var rel = PostFormUrl(url, t);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// POST请求(FormUrlEncodedContent)
        /// </summary>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>返回结果</returns>
        public static V PostDictionary<V>(string url, Dictionary<string, string> parameters)
        {
            string result = Post(url, parameters, null);
            var rel = result;
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// POST请求(FormUrlEncodedContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string PostFormUrl<T>(string url, T t)
        {
            var map = ToMap(t);
            string result = Post(url, map, null);
            return result;
        }

        /// <summary>
        /// POST请求(ByteArrayContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V PostByte<T, V>(string url, T t)
        {
            var rel = PostByte(url, t);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// POST请求(ByteArrayContent)
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string PostByte<T>(string url, T t)
        {
            var map = t.SerializeObject();
            var data = Encoding.UTF8.GetBytes(map);
            string result = Post(url, null, data);
            return result;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameters">请求参数</param>
        /// <param name="datas">请求参数</param>
        /// <returns>返回结果</returns>
        public static string Post(string url, Dictionary<string, string> parameters, byte[] datas)
        {
            Uri uri;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = parameters != null ? GetHttpContent(parameters) : GetHttpContent(datas);
                var response = http.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();
                var rel = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                return rel;
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>HttpContent</returns>
        private static HttpContent GetHttpContent(Dictionary<string, string> parameters)
        {
            return new FormUrlEncodedContent(parameters);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="datas">datas</param>
        /// <returns>HttpContent</returns>
        private static HttpContent GetHttpContent(byte[] datas)
        {
            return new ByteArrayContent(datas);
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static string GetString<T>(string url, T t)
        {
            var map = ToMap(t);
            string result = Get(url, map);
            return result;
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <typeparam name="T">请求参数类型</typeparam>
        /// <typeparam name="V">返回结果类型</typeparam>
        /// <param name="url">请求Url</param>
        /// <param name="t">请求参数</param>
        /// <returns>返回结果</returns>
        public static V GetModel<T, V>(string url, T t)
        {
            var map = ToMap(t);
            var rel = GetString(url, map);
            return rel.DeserializeObject<V>();
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="parameters">请求参数</param>
        /// <returns>结果</returns>
        public static string Get(string url, Dictionary<string, string> parameters)
        {
            url = url + GteParameters(parameters);
            Uri uri;
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);
            using (var http = new HttpClient())
            {
                var response = http.GetAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
                var rel = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                return rel;
            }
        }

        /// <summary>
        /// GteParameters
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>string</returns>
        private static string GteParameters(Dictionary<string, string> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return string.Empty;
            }
            var param = parameters.Join("&", p => p.Key + "=" + p.Value);
            return "?" + param;
        }
    }
}