using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class SPosition : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path)))
            {
                var size = stream.BaseStream.Length;
                byte i = 0;

                var list = new List<SPositionJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new SPositionJson()
                    {
                        LoopId = i,
                        Unk1 = stream.ReadInt32(),
                        Coord = new int[]
                        {
                            stream.ReadUInt16(), stream.ReadUInt16()
                        },
                        Map = stream.ReadInt16(),
                        Location = stream.ReadInt16(),
                        TpLevel = stream.ReadInt32(),
                        MapName = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
                        RegionName = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
                    };

                    i++;
                    if (!string.IsNullOrEmpty(temp.MapName))
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class SPositionJson
    {
        public ushort LoopId { get; set; }
        public int Unk1 { get; set; }
        public int[] Coord { get; set; }
        public short Map { get; set; }
        public short Location { get; set; }
        public int TpLevel { get; set; }
        public string MapName { get; set; }
        public string RegionName { get; set; }
    }
}