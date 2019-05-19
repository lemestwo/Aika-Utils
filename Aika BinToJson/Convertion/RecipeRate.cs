using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new RecipeRateJson
                    {
                        LoopId = i,
                        RecipeId = stream.ReadUInt32(),
                        IngredientId = stream.ReadUInt32(),
                        IncreasedChance = stream.ReadInt32() == 1,
                        Chance = new ushort[16],
                    };
                    for (var j = 0; j < 16; j++)
                        temp.Chance[j] = stream.ReadUInt16();

                    i++;
                    if (temp.RecipeId == 0) continue;

                    list.Add(temp);
                    var queryStart = $"INSERT INTO `data_recipe_rates` VALUES ({temp.RecipeId}, {temp.IngredientId}, {temp.IncreasedChance}, ";
                    var rates = new StringBuilder();
                    for (var j = 0; j < 16; j++)
                        rates.AppendFormat("{0}, ", temp.Chance[j]);

                    var queryMiddle = rates.ToString().Trim(' ', ',');
                    var queryEnd = ");";
                    txt.AppendLine(string.Concat(queryStart, queryMiddle, queryEnd));
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class RecipeRateJson
    {
        public ushort LoopId { get; set; }
        public uint RecipeId { get; set; }
        public uint IngredientId { get; set; }
        public bool IncreasedChance { get; set; }
        public ushort[] Chance { get; set; }
    }
}