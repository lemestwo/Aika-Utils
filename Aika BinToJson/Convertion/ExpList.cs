using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ExpList : Base
    {
        public ExpList(string path, string outPath) : base(path, outPath)
        {
        }

        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path)))
            {
                var size = stream.BaseStream.Length;
                byte i = 0;

                var list = new List<ExpListJson>();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new ExpListJson
                    {
                        Level = i,
                        Experience = stream.ReadUInt64()
                    };

                    i++;
                    list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list);
            }
        }
    }
}