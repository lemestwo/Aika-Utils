using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ReinforceA : BaseConvert
    {
        public bool IsReinforceW = false;

        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<ReinforceAJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new ReinforceAJson
                    {
                        LoopId = i,
                        Unk = stream.ReadInt32(),
                        Price = stream.ReadInt32(),
                        Chance = new ushort[16]
                    };

                    for (var j = 0; j < 16; j++)
                        temp.Chance[j] = stream.ReadUInt16();

                    i++;

                    if (temp.Price == 999999) continue;

                    list.Add(temp);
                    txt.AppendLine(
                        $"INSERT INTO `data_reinforce_{(IsReinforceW ? "w" : "a")}` VALUES ({temp.LoopId + 1}, {temp.Price}, {temp.Unk}, {string.Join(", ", temp.Chance)}); ");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class ReinforceAJson
    {
        public ushort LoopId { get; set; }
        public int Unk { get; set; }
        public int Price { get; set; }
        public ushort[] Chance { get; set; }
    }
}