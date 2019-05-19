using System;
using System.IO;
using System.Text;

namespace Aika_BinToJson.Convertion
{
    public abstract class BaseConvert
    {
        protected string Path;
        private string _outPath;
        private string _outPathSql;
        protected string JsonData;
        protected string SqlData;

        // Fix accentuation
        protected readonly Encoding Encode = Encoding.GetEncoding("iso-8859-1");

        public void SetupFile(string path, string outPath, string outPathSql)
        {
            Path = path;
            _outPath = outPath;
            _outPathSql = outPathSql;
            SqlData = string.Empty;
        }

        public abstract void Convert();

        public virtual void Save()
        {
            if (!string.IsNullOrEmpty(JsonData))
                File.WriteAllText(_outPath, JsonData);

            if (_outPathSql != null && !string.IsNullOrEmpty(SqlData))
                File.WriteAllText(_outPathSql, SqlData);
        }
    }
}