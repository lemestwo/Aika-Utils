using System.Collections.Generic;
using System.IO;
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
                while (stream.BaseStream.Position < size)
                {
                    var temp = new GearCoreJson
                    {
                        LoopId = i,
                        GcId = stream.ReadByte(),
                        UnkId = stream.ReadByte(),
                        SourceItem = stream.ReadUInt16(),
                        DestItem = stream.ReadUInt16(),
                        UnkData = stream.ReadBytes(10)
                    };
                    i++;
                    if (temp.GcId != 0)
                    {
                        list.Add(temp);
                    }
                }

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
        public byte[] UnkData { get; set; }
    }
}