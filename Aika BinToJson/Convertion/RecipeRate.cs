using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class RecipeRate : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path)))
            {
                var size = stream.BaseStream.Length;
                byte i = 0;

                var list = new List<RecipeRateJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new RecipeRateJson()
                    {
                        LoopId = i,
                        RecipeId = stream.ReadUInt32(),
                        IngredientId = stream.ReadUInt32(),
                        IncreasedChance = stream.ReadInt32(),
                    };
                    temp.Chance = new ushort[16];
                    for (var j = 0; j < 16; j++)
                        temp.Chance[j] = stream.ReadUInt16();

                    i++;
                    if (temp.RecipeId != 0)
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class RecipeRateJson
    {
        public ushort LoopId { get; set; }
        public uint RecipeId { get; set; }
        public uint IngredientId { get; set; }
        public int IncreasedChance { get; set; }
        public ushort[] Chance { get; set; }
    }
}