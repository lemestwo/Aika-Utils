using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Dialog : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<DialogJson>();
                while (stream.BaseStream.Position < size)
                {
                    var temp = new DialogJson
                    {
                        LoopId = i,
                    };
                    stream.ReadBytes(68);
                    for (var j = 0; j < 40; j++)
                    {
                        var temp2 = new MessagesDialog
                        {
                            A = stream.ReadInt32(),
                            Message = Encode.GetString(stream.ReadBytes(128)).Trim('\u0000'),
                            B = stream.ReadInt32()
                        };
                        if (!string.IsNullOrEmpty(temp2.Message))
                        {
                            temp.Dialogs.Add(temp2);
                        }
                    }

                    i++;
                    if (temp.Dialogs.Count > 0)
                    {
                        list.Add(temp);
                    }
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }

    public class DialogJson
    {
        public ushort LoopId { get; set; }
        public List<MessagesDialog> Dialogs { get; set; }

        public DialogJson()
        {
            Dialogs = new List<MessagesDialog>();
        }
    }

    public class MessagesDialog
    {
        public int A { get; set; }
        public string Message { get; set; }
        public int B { get; set; }
    }
}