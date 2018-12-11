using System.IO;
using System.IO.Compression;

namespace DotNet.Standard.Utilities
{
    public static class GZipUtil
    {
        /// <summary>
        /// 压缩流
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GZipCompress(this byte[] value)
        {
            using (var tempms = new MemoryStream())
            {
                try
                {
                    using (var zipStream = new GZipStream(tempms, CompressionMode.Compress))
                    {
                        zipStream.Write(value, 0, value.Length);
                        zipStream.Close();
                    }
                    return tempms.ToArray();
                }
                finally
                {
                    tempms.Close();
                }
            }
        }

        /// <summary>
        /// 解压流
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GZipDecompress(this byte[] value)
        {
            using (var tempms = new MemoryStream())
            {
                try
                {
                    using (var ms = new MemoryStream(value))
                    using (var zipStream = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        var buffer = new byte[1024];
                        int length;
                        while ((length = zipStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            tempms.Write(buffer, 0, length);
                        }
                        zipStream.Close();
                        ms.Close();
                    }
                    return tempms.ToArray();
                }
                finally
                {
                    tempms.Close();
                }
            }
        }
    }
}
