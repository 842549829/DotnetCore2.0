using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// 动态表达式
    /// </summary>
    public static class DynamicExpression
    {
        /// <summary>
        /// 获取动态表达式
        /// </summary>
        /// <typeparam name="TResult">表达式数据类型</typeparam>
        /// <typeparam name="TCondition">表达式条件类型</typeparam>
        /// <param name="data">数据</param>
        /// <param name="condtion">条件</param>
        /// <returns>表达式</returns>
        public static Func<TResult, bool> GetDynamicExpression<TResult, TCondition>(this IEnumerable<TResult> data, TCondition condtion)
            where TResult : class
            where TCondition : class
        {
            Type tConditionType = typeof(TCondition);
            Type tResultType = typeof(TResult);

            Expression totalExpr = Expression.Constant(true);
            ParameterExpression param = Expression.Parameter(typeof(TResult), "n");
            foreach (PropertyInfo property in tConditionType.GetProperties())
            {
                string key = property.Name;
                object value = property.GetValue(condtion);
                if (value != null && value.ToString() != string.Empty)
                {
                    DynamicExpressionAttribute dynamicExpressionAttribute = CustomAttributeExtension<DynamicExpressionAttribute>.GetCustomAttributeValue(tConditionType, property);
                    if (!dynamicExpressionAttribute.IsDynamicExpression)
                    {
                        continue;
                    }

                    //等式左边的值
                    string name = dynamicExpressionAttribute.Name ?? key;
                    Expression left = Expression.Property(param, tResultType.GetProperty(name));
                    //等式右边的值
                    Expression right = Expression.Constant(value);

                    Expression filter;
                    switch (dynamicExpressionAttribute.Operator)
                    {
                        case "!=":
                            filter = Expression.NotEqual(left, right);
                            break;
                        case ">":
                            filter = Expression.GreaterThan(left, right);
                            break;
                        case ">=":
                            filter = Expression.GreaterThanOrEqual(left, right);
                            break;
                        case "<":
                            filter = Expression.LessThan(left, right);
                            break;
                        case "<=":
                            filter = Expression.LessThanOrEqual(left, right);
                            break;
                        case "Like":
                        case "Contains":
                            filter = Expression.Call(Expression.Property(param, tResultType.GetProperty(name)), typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value));
                            break;
                        default:
                            filter = Expression.Equal(left, right);
                            break;
                    }
                    totalExpr = Expression.And(filter, totalExpr);
                }
            }
            var predicate = Expression.Lambda(totalExpr, param);
            var dynamic = (Func<TResult, bool>)predicate.Compile();
            return dynamic;
        }
    }

    /// <summary>
    /// 动态表达式特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class DynamicExpressionAttribute : Attribute
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 运行符号
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 是否参与表达式计算
        /// </summary>
        public bool IsDynamicExpression { get; set; } = true;
    }

    /// <summary>
    /// 获取实体对象的自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">自定义特性类型</typeparam>
    public class CustomAttributeExtension<TAttribute>
        where TAttribute : Attribute
    {
        /// <summary>
        /// Cache Data
        /// </summary>
        private static readonly Dictionary<string, TAttribute> Cache = new Dictionary<string, TAttribute>();

        /// <summary>
        /// 获取CustomAttribute Value
        /// </summary>
        /// <param name="type">实体对象类型</param>
        /// <param name="propertyInfo">实体对象属性信息</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        public static TAttribute GetCustomAttributeValue(Type type, PropertyInfo propertyInfo)
        {
            var key = BuildKey(type, propertyInfo);
            if (!Cache.ContainsKey(key))
            {
                CacheAttributeValue(type, propertyInfo);
            }
            return Cache[key];
        }

        /// <summary>
        /// 获取CustomAttribute Value
        /// </summary>
        /// <param name="sourceType">实体对象数据类型</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        public static TAttribute GetCustomAttributeValue(Type sourceType)
        {
            var key = BuildKey(sourceType, null);
            if (!Cache.ContainsKey(key))
            {
                CacheAttributeValue(sourceType, null);
            }
            return Cache[key];
        }

        /// <summary>
        /// 获取实体类上的特性
        /// </summary>
        /// <param name="type">实体对象类型</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        private static TAttribute GetClassAttributes(Type type)
        {
            var attribute = type.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
            return (TAttribute)attribute;
        }

        /// <summary>
        /// 获取实体属性上的特性
        /// </summary>
        /// <param name="propertyInfo">属性信息</param>
        /// <returns>返回Attribute的值，没有则返回null</returns>
        private static TAttribute GetPropertyAttributes(PropertyInfo propertyInfo)
        {
            var attribute = propertyInfo?.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault();
            return (TAttribute)attribute;
        }

        /// <summary>
        /// 缓存Attribute Value
        /// <param name="type">实体对象类型</param>
        /// <param name="propertyInfo">实体对象属性信息</param>
        /// </summary>
        private static void CacheAttributeValue(Type type, PropertyInfo propertyInfo)
        {
            var key = BuildKey(type, propertyInfo);
            var value = propertyInfo == null ? GetClassAttributes(type) : GetPropertyAttributes(propertyInfo);
            lock (key + "_attributeValueLockKey")
            {
                if (!Cache.ContainsKey(key))
                {
                    Cache[key] = value;
                }
            }
        }

        /// <summary>
        /// 缓存Collection Name Key
        /// <param name="type">type</param>
        /// <param name="propertyInfo">propertyInfo</param>
        /// </summary>
        private static string BuildKey(Type type, PropertyInfo propertyInfo)
        {
            if (string.IsNullOrEmpty(propertyInfo?.Name))
            {
                return type.FullName;
            }
            return type.FullName + "." + propertyInfo.Name;
        }
    }
}
