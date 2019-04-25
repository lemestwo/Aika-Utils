using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ObjPos : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<ObjPosJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new ObjPosJson
                    {
                        Id = stream.ReadUInt32(),
                        Name = Encode.GetString(stream.ReadBytes(12)).Trim('\u0000'),
                        Coords = new List<Coord>(),
                        ItemsId = new List<uint>()
                    };

                    for (var j = 0; j < 10; j++)
                    {
                        var coord = new Coord
                        {
                            X = stream.ReadInt32(),
                            Y = stream.ReadInt32()
                        };
                        if (coord.X != 0 || coord.Y != 0)
                            temp.Coords.Add(coord);
                    }

                    for (var j = 0; j < 10; j++)
                    {
                        var itemId = stream.ReadUInt32();
                        if (itemId != 0) temp.ItemsId.Add(itemId);
                    }

                    i++;
                    if (!string.IsNullOrEmpty(temp.Name))
                    {
                        list.Add(temp);
                    }
                }

                JsonData = JsonConvert.SerializeObject(list);
            }
        }
    }

    public class ObjPosJson
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public List<Coord> Coords { get; set; }
        public List<uint> ItemsId { get; set; }
    }

    public class Coord
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}