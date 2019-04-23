using System;
using System.IO;
using System.Text;

namespace Aika_BinToJson.Convertion
{
    public abstract class BaseConvert
    {
        protected readonly string Path;
        private readonly string _outPath;
        protected string JsonData;
        
        // Fix accentuation
        protected readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");

        protected BaseConvert(string path, string outPath)
        {
            Path = path;
            _outPath = outPath;
        }

        public abstract void Convert();

        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(JsonData))
                File.WriteAllText(_outPath, JsonData);
        }
    }
}