using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace DotnetCore.Code.Extension
{
    /// <summary>
    /// 序列化
    /// </summary>
    public static class SerializerExtension
    {
        /// <summary>
        /// JSON序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>JSON字符串</returns>
        public static string SerializeObject(this object obj)
        {
            try
            {
                var jsonSerializerSettings = new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd HH:mm:ss" };
                return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">对象</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>对象</returns>
        public static T DeserializeObject<T>(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json">JSON字符串</param>
        /// <returns>对象</returns>
        public static dynamic DeserializeObject(this string json)
        {
            try
            {
                return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// XML序列化方式深复制
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>复制对象</returns>
        public static T DeepCopy<T>(this T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }

            return (T)retval;
        }

        /// <summary>
        /// 将一个实体对象转换为另一个实体对象
        /// </summary>
        /// <typeparam name="T1">第一个实体对象</typeparam>
        /// <typeparam name="T2">第二个实体对象</typeparam>
        /// <param name="source">转换的实体对象</param>
        /// <returns></returns>
        public static T2 CopyToModel<T1, T2>(T1 source)
        {
            T2 model = default(T2);
            if (source == null)
            {
                return model;
            }
            PropertyInfo[] pi = typeof(T2).GetProperties();
            PropertyInfo[] pi1 = typeof(T1).GetProperties();

            model = Activator.CreateInstance<T2>();
            for (int i = 0; i < pi.Length; i++)
            {
                for (int j = 0; j < pi1.Length; j++)
                {
                    if (pi[i].Name == pi1[j].Name)
                    {
                        pi[i].SetValue(model, pi1[j].GetValue(source, null), null);
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// 将一个实体对象转换为另一个实体对象
        /// </summary>
        /// <typeparam name="T1">第一个实体对象</typeparam>
        /// <typeparam name="T2">第二个实体对象</typeparam>
        /// <param name="source">转换的实体对象</param>
        /// <returns></returns>
        public static List<T2> CopyToModel<T1, T2>(List<T1> source)
        {
            List<T2> modelList = new List<T2>();
            if (!source.Any())
            {
                return modelList;
            }
            PropertyInfo[] pi = typeof(T2).GetProperties();
            PropertyInfo[] pi1 = typeof(T1).GetProperties();
            foreach (T1 obj in source)
            {
                T2 model = Activator.CreateInstance<T2>();
                for (int i = 0; i < pi.Length; i++)
                {
                    for (int j = 0; j < pi1.Length; j++)
                    {
                        if (pi[i].Name == pi1[j].Name)
                        {
                            pi[i].SetValue(model, pi1[j].GetValue(obj, null), null);
                        }
                    }
                }
                modelList.Add(model);
            }

            return modelList;
        }
    }
}