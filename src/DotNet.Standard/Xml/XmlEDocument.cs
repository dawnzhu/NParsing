using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace DotNet.Standard.Xml
{
    public class XmlEDocument : XmlDocument
    {
        private readonly string _rgbKey;
        private const int READ_SIZE = 16*1024;

        public XmlEDocument(string rgbKey = "zhi_dian")
        {
            _rgbKey = rgbKey;
        }

        public override void Load(string filename)
        {
            var fileStream = new FileStream(filename, FileMode.Open);
            Load(fileStream);
        }

        public override void Load(Stream inStream)
        {
            var bsXml = new List<byte>();
            var b = new byte[READ_SIZE];
            int iLength;
            while ((iLength = inStream.Read(b, 0, READ_SIZE)) > 0)
            {
                for (var i = 0; i < iLength; i++)
                {
                    bsXml.Add(b[i]);
                }
            }
            inStream.Close();
            var msEXml = new MemoryStream();
            var des = new DESCryptoServiceProvider();
            byte[] byKey = Encoding.UTF8.GetBytes(_rgbKey.Substring(0, 8));
            byte[] iv = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
            var cs = new CryptoStream(msEXml, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(bsXml.ToArray(), 0, bsXml.ToArray().Length);
            cs.FlushFinalBlock();
            base.Load(new MemoryStream(msEXml.ToArray()));
        }

        public override void Save(string filename)
        {
            if(!File.Exists(filename))
                File.Create(filename).Close();

            var fileStream = new FileStream(filename, FileMode.Truncate);
            Save(fileStream);
            fileStream.Close();
        }

        public override void Save(Stream outStream)
        {
            var msXml = new MemoryStream();
            base.Save(msXml);
            var des = new DESCryptoServiceProvider();
            byte[] byKey = Encoding.UTF8.GetBytes(_rgbKey.Substring(0, 8));
            byte[] iv = {0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF};
            var cs = new CryptoStream(outStream, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(msXml.ToArray(), 0, msXml.ToArray().Length);
            cs.FlushFinalBlock();
            msXml.Close();
        }
    }
}