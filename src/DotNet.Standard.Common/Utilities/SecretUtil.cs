using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DotNet.Standard.Common.Utilities
{
    public class SecretUtil
    {
        #region MD5数据加密

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string sText)
        {
            return MD5Encrypt(sText, false, true, false);
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="zerofill"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(string sText, bool zerofill)
        {
            return MD5Encrypt(sText, false, true, zerofill);
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string sText)
        {
            return MD5Encrypt(sText, true, true, false);
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="zerofill"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string sText, bool zerofill)
        {
            return MD5Encrypt(sText, true, true, zerofill);
        }

        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string MD5Encrypt16Lower(string sText)
        {
            return MD5Encrypt(sText, false, false, false);
        }
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="zerofill"></param>
        /// <returns></returns>
        public static string MD5Encrypt16Lower(string sText, bool zerofill)
        {
            return MD5Encrypt(sText, false, false, zerofill);
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <returns></returns>
        public static string MD5Encrypt32Lower(string sText)
        {
            return MD5Encrypt(sText, true, false, false);
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="zerofill"></param>
        /// <returns></returns>
        public static string MD5Encrypt32Lower(string sText, bool zerofill)
        {
            return MD5Encrypt(sText, true, false, zerofill);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <param name="is32"></param>
        /// <param name="upper"></param>
        /// <param name="zerofill"></param>
        /// <returns></returns>
        private static string MD5Encrypt(string password, bool is32, bool upper, bool zerofill)
        {
            var pwd = "";
            var md5 = MD5.Create();
            var s = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            var sI = 0;
            var eI = s.Length;
            if (!is32)
            {
                sI = 4;
                eI = 12;
            }
            for (var i = sI; i < eI; i++)
            {
                pwd += s[i].ToString(upper ? (zerofill ? "X2" : "X") : (zerofill ? "x2" : "x"));
            }
            return pwd;
        }

        #endregion

        #region RSA数据加密

        /// <summary>
        /// RSA获取公钥私钥
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static bool RsaGetKeys(out string publicKey, out string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                return true;
            }
        }

        /// <summary>
        /// RSA数据加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static byte[] RsaEncript(string sText, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(Encoding.UTF8.GetBytes(sText), false);
            }
        }

        /// <summary>
        /// RSA数据加密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="publicKey"></param>
        /// <returns>Base64</returns>
        public static string RsaEncriptToString(string sText, string publicKey)
        {
            var tempByte = RsaEncript(sText, publicKey);
            return Convert.ToBase64String(tempByte);
        }

        /// <summary>
        /// RSA数据解密
        /// </summary>
        /// <param name="sText"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RsaDecript(string sText, string privateKey)
        {
            return RsaDecript(Convert.FromBase64String(sText), privateKey);
        }

        /// <summary>
        /// RSA数据解密
        /// </summary>
        /// <param name="rgb"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RsaDecript(byte[] rgb, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                return Encoding.UTF8.GetString(rsa.Decrypt(rgb, false));
            }
        }

        #endregion
    }
}