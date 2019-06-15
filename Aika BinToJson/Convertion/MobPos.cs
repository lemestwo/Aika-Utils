using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class MobPos : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<MobPosJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new MobPosJson()
                    {
                        LoopId = i,
                        MobId = (ushort) stream.ReadUInt32(),
                        Name = Encode.GetString(stream.ReadBytes(12)).Trim('\u0000'),
                        Position = new List<MobPosition>()
                    };
                    for (var j = 0; j < 50; j++)
                    {
                        var temp2 = new MobPosition
                        {
                            CoordX = (ushort) stream.ReadUInt32(),
                            CoordY = (ushort) stream.ReadUInt32()
                        };
                        if (temp2.CoordX > 0)
                        {
                            temp.Position.Add(temp2);
                        }
                    }

                    i++;
                    if (temp.MobId > 0)
                    {
                        list.Add(temp);
                    }
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}