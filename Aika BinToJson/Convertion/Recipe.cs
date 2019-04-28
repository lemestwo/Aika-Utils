using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Recipe : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<RecipeJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new RecipeJson
                    {
                        LoopId = i,
                        ItemId = stream.ReadUInt32(),
                        TargetItemId = stream.ReadUInt16(),
                        TargetItemIdSup = stream.ReadUInt16(),
                        Price = stream.ReadUInt32(),
                        Quantity = stream.ReadByte(),
                        MinLevel = stream.ReadByte(),
                        Chance = stream.ReadUInt16(),
                        SuperiorChance = stream.ReadUInt16(),
                        DoubleChance = stream.ReadUInt16(),
                        IngredientsItemId = new ushort[12],
                        IngredientsQuantity = new uint[12]
                    };

                    for (var j = 0; j < 12; j++)
                        temp.IngredientsItemId[j] = stream.ReadUInt16();

                    for (var j = 0; j < 12; j++)
                        temp.IngredientsQuantity[j] = stream.ReadUInt32();

                    i++;

                    if (temp.ItemId != 0)
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}