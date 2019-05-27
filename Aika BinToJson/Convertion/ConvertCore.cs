using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ConvertCore : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<ConvertCoreJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new ConvertCoreJson
                    {
                        LoopId = i,
                        ResultItemId = stream.ReadUInt16(),
                        ItemId =
                        {
                            [0] = stream.ReadUInt16(),
                            [1] = stream.ReadUInt16(),
                            [2] = stream.ReadUInt16(),
                            [3] = stream.ReadUInt16(),
                            [4] = stream.ReadUInt16(),
                        },
                        GearLevel = stream.ReadByte(),
                        BaseChance = stream.ReadByte(),
                        ExtChance = stream.ReadByte(),
                        ConcExtChance = stream.ReadByte()
                    };
                    i++;
                    if (temp.ResultItemId <= 0) continue;

                    list.Add(temp);
                    txt.AppendLine($"INSERT INTO `data_gearconverts` VALUES ({temp.LoopId + 1}, {temp.ResultItemId}, " +
                                   $"{temp.ItemId[0]}, {temp.ItemId[1]}, {temp.ItemId[2]}, {temp.ItemId[3]}, {temp.ItemId[4]}, " +
                                   $"{temp.GearLevel}, {temp.BaseChance}, {temp.ExtChance}, {temp.ConcExtChance});");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class ConvertCoreJson
    {
        public ushort LoopId { get; set; }
        public ushort ResultItemId { get; set; }
        public ushort[] ItemId { get; set; }
        public byte GearLevel { get; set; }
        public byte BaseChance { get; set; }
        public byte ExtChance { get; set; }
        public byte ConcExtChance { get; set; }

        public ConvertCoreJson()
        {
            ItemId = new ushort[5];
        }
    }
}