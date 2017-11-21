using System;
using System.IO;
using System.Text;

namespace DotnetCore.Code.Code
{
    /// <summary>
    /// 编码帮助类
    /// </summary>	
    public class EncodingHelper
    {
        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name="fileName">文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static Encoding GetEncoding(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return GetEncoding(fs);
            }
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name="fs">文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static Encoding GetEncoding(FileStream fs)
        {
            //byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            //byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            //byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 

            try
            {
                //如果小于3字节，直接退出
                if (fs.Length < 3)
                {
                    return Encoding.Default;
                }

                byte[] ss = new byte[3];
                fs.Read(ss, 0, 3);

                if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                {
                    return Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    return Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    return Encoding.Unicode;
                }
                else if (IsUTF8Bytes(fs))
                {
                    return Encoding.UTF8;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("读取文件编码类型出错" + ex.Message);
            }
            return Encoding.Default;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if [is ut f8 bytes] [the specified data]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.Exception">非预期的byte格式</exception>
        public static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        public static bool IsUTF8Bytes(FileStream fs)
        {
            byte[] ss = new byte[1];
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            fs.Seek(0, SeekOrigin.Begin);

            for (int i = 0; i < fs.Length; i++)
            {
                fs.Read(ss, 0, 1);
                curByte = ss[0];

                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
    }
}