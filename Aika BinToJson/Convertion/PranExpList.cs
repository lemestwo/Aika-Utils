using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class PranExpList : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path)))
            {
                var size = stream.BaseStream.Length;
                byte i = 0;

                var list = new List<PranExpListJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size - 202)
                {
                    var temp = new PranExpListJson
                    {
                        Level = i,
                        Experience = stream.ReadUInt32()
                    };

                    i++;
                    list.Add(temp);
                    txt.AppendLine($"INSERT INTO `data_pran_exp` VALUES ({temp.Level}, {temp.Experience});");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class PranExpListJson
    {
        public byte Level { get; set; }
        public uint Experience { get; set; }
    }
}