using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class GearCore : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<GearCoreJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new GearCoreJson
                    {
                        LoopId = i,
                        GcId = stream.ReadByte(),
                        UnkId = stream.ReadByte(),
                        SourceItem = stream.ReadUInt16(),
                        DestItem = stream.ReadUInt16(),
                        Chance = stream.ReadByte(),
                        ExtChance = stream.ReadByte(),
                        ConcExtChance = stream.ReadByte(),
                        UnkData = stream.ReadBytes(7)
                    };
                    i++;
                    if (temp.GcId <= 0) continue;
                    
                    list.Add(temp);
                    txt.AppendLine($"INSERT INTO `data_gearcores` VALUES ({temp.LoopId + 1}, {temp.GcId}, {temp.UnkId}, {temp.SourceItem}, " +
                                   $"{temp.DestItem}, {temp.Chance}, {temp.ExtChance}, {temp.ConcExtChance});");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class GearCoreJson
    {
        public ushort LoopId { get; set; }
        public byte GcId { get; set; }
        public byte UnkId { get; set; }
        public ushort SourceItem { get; set; }
        public ushort DestItem { get; set; }
        public byte Chance { get; set; }
        public byte ExtChance { get; set; }
        public byte ConcExtChance { get; set; }
        public byte[] UnkData { get; set; }
    }
}