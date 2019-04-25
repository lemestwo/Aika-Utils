using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class MakeItems : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<MakeItemsJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new MakeItemsJson()
                    {
                        LoopId = i,
                        TargetItemId = stream.ReadUInt32(),
                        Price = stream.ReadUInt32(),
                        Quantity = stream.ReadByte(),
                        Level = stream.ReadByte(),
                        Rate = stream.ReadUInt16(),
                        SuperiorRate = stream.ReadUInt16(),
                        DoubleRate = stream.ReadUInt16(),
                        IngredientsItemId = new ushort[12],
                        IngredientsQuantity = new ushort[12]
                    };

                    for (var j = 0; j < 12; j++)
                        temp.IngredientsItemId[j] = stream.ReadUInt16();

                    for (var j = 0; j < 12; j++)
                        temp.IngredientsQuantity[j] = stream.ReadByte();

                    i++;

                    if (temp.TargetItemId != 0)
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list);
            }
        }
    }
}