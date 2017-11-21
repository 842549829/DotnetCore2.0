using System;

namespace DotnetCore.Code.Struct
{
    /// <summary>
    /// 区间
    /// </summary>
    /// <typeparam name="T">区间类型</typeparam>
    [Serializable]
    public class Range<T>
    {
        /// <summary>
        /// 下限(时间开始)
        /// </summary>
        public T Lower
        {
            get;
            set;
        }

        /// <summary>
        /// 上限(时间结束)
        /// </summary>
        public T Upper
        {
            get;
            set;
        }
    }
}