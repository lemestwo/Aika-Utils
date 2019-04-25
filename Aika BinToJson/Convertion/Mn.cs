using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Mn : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<MnJson>();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new MnJson()
                    {
                        Id = i,
                        Name = Encode.GetString(stream.ReadBytes(128)).Trim('\u0000')
                    };
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

    public class MnJson
    {
        public ushort Id { get; set; }
        public string Name { get; set; }
    }
}