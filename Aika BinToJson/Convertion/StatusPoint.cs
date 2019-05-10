using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class StatusPoint : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;

                var j = 0u;
                var list = new List<StatusPointJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new StatusPointJson
                    {
                        LoopId = j,
                        Ints = new int[25]
                    };
                    for (var i = 0; i < 25; i++)
                    {
                        temp.Ints[i] = stream.ReadInt32();
                    }

                    j++;
                    list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class StatusPointJson
    {
        public uint LoopId { get; set; }
        public int[] Ints { get; set; }
    }
}