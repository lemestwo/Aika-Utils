using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Map : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;

                var list = new List<MapJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new MapJson[64];
                    for (var j = 0; j < 64; j++)
                    {
                        temp[j] = new MapJson {LoopId = (ushort) j};
                        var range = new CoordRec
                        {
                            X1 = stream.ReadInt32(),
                            Y1 = stream.ReadInt32(),
                            X2 = stream.ReadInt32(),
                            Y2 = stream.ReadInt32(),
                        };
                        temp[j].Coord = range;
                    }

                    for (var j = 0; j < 64; j++)
                        temp[j].Name = Encode.GetString(stream.ReadBytes(28)).Trim('\u0000');

                    for (var j = 0; j < 64; j++)
                    {
                        //temp[j].Desc = Encode.GetString(stream.ReadBytes(1024)).Trim('\u0000');
                        stream.ReadBytes(1024);
                    }
                    // 65536

                    for (var j = 0; j < 64; j++)
                        temp[j].Unk1 = stream.ReadInt32();

                    for (var j = 0; j < 64; j++)
                        temp[j].Unk2 = stream.ReadInt32();

                    for (var j = 0; j < 64; j++)
                        temp[j].Unk3 = stream.ReadByte();

                    for (var j = 0; j < 64; j++)
                        temp[j].Unk4 = stream.ReadInt32();

                    list.AddRange(temp.Where(mapJson => !string.IsNullOrEmpty(mapJson.Name)));
                }

                foreach (var map in list)
                {
                    var newName = map.Name.Replace("'", "''").Trim();
                    txt.AppendLine($"INSERT INTO `data_maps` VALUES ({map.LoopId + 1}, '{newName}', " +
                                   $"{map.Coord.X1}, {map.Coord.Y1}, {map.Coord.X2}, {map.Coord.Y2}, " +
                                   $"{map.Unk1}, {map.Unk2}, {map.Unk3}, {map.Unk4});");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}