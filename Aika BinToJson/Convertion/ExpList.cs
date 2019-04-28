using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ExpList : BaseConvert
    {
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

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class ExpListJson
    {
        public byte Level { get; set; }
        public ulong Experience { get; set; }
    }
}