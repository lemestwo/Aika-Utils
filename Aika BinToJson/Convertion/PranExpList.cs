using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class PranExpList : Base
    {
        public PranExpList(string path, string outPath) : base(path, outPath)
        {
        }

        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path)))
            {
                var size = stream.BaseStream.Length;
                byte i = 0;

                var list = new List<PranExpListJson>();
                while (stream.BaseStream.Position < size - 202)
                {
                    var temp = new PranExpListJson
                    {
                        Level = i,
                        Experience = stream.ReadUInt32()
                    };

                    i++;
                    list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list);
            }
        }
    }
}