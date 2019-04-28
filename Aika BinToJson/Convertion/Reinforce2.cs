using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Reinforce2 : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<Reinforce2Json>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new Reinforce2Json
                    {
                        LoopId = i,
                        Bonus1 = new uint[16],
                        Bonus2 = new uint[16]
                    };

                    for (var j = 0; j < 16; j++)
                        temp.Bonus1[j] = stream.ReadUInt32();

                    for (var j = 0; j < 16; j++)
                        temp.Bonus2[j] = stream.ReadUInt32();

                    i++;
                    var hasBonus = false;
                    foreach (var b in temp.Bonus1)
                        if (b > 0)
                            hasBonus = true;

                    foreach (var b in temp.Bonus2)
                        if (b > 0)
                            hasBonus = true;

                    if (hasBonus)
                    {
                        list.Add(temp);
                    }
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class Reinforce2Json
    {
        public ushort LoopId { get; set; }
        public uint[] Bonus1 { get; set; }
        public uint[] Bonus2 { get; set; }
    }
}