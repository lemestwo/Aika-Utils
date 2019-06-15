using System.Collections.Generic;
using System.IO;
using System.Text;
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
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new MakeItemsJson()
                    {
                        ItemId = i,
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

                    if (temp.TargetItemId == 0) continue;

                    list.Add(temp);
                    txt.AppendLine($"INSERT INTO `data_make_items` VALUES ({temp.ItemId}, {temp.TargetItemId}, {temp.Level}, {temp.Price}, " +
                                   $"{temp.Quantity}, {temp.Rate}, {temp.SuperiorRate}, {temp.DoubleRate});");
                    for (var j = 0; j < 12; j++)
                    {
                        if (temp.IngredientsItemId[j] > 0)
                            txt.AppendLine(
                                $"INSERT INTO `data_make_item_ingredients` VALUES ({temp.ItemId}, {temp.IngredientsItemId[j]}, {temp.IngredientsQuantity[j]});");
                    }
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}