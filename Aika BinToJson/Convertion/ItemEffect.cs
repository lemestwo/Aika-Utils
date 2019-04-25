using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ItemEffect : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;

                var list = new List<ItemEffectJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new ItemEffectJson
                    {
                        Name = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
                        Index = stream.ReadUInt32()
                    };
                    if (!string.IsNullOrEmpty(temp.Name))
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list);
            }
        }
    }

    public class ItemEffectJson
    {
        public string Name { get; set; }
        public uint Index { get; set; }
    }
}