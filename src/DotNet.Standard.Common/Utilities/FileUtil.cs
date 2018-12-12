using System.IO;

namespace DotNet.Standard.Common.Utilities
{
    public static class FileUtil
    {
        public static bool Save(this byte[] data, string path, string fileName)
        {
            if (data == null)
                return false;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            using (var file = new FileStream(path + "\\" + fileName, FileMode.CreateNew, FileAccess.Write))
            {
                file.Write(data, 0, data.Length);
                file.Close();
            }
            return true;
        }
    }
}
