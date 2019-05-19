using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class Title : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                ushort i = 0;

                var list = new List<TitleJson>();
                var txt = new StringBuilder();
                while (stream.BaseStream.Position < 57334)
                {
                    var temp = new TitleJson();
                    temp.LoopId = i;
                    temp.UnkId = stream.ReadUInt16();
                    temp.MobsKilled = stream.ReadUInt16();
                    temp.Effect1 = stream.ReadUInt16();
                    temp.Effect2 = stream.ReadUInt16();
                    temp.Effect3 = stream.ReadUInt16();
                    temp.Effect1Value = stream.ReadUInt16();
                    temp.Effect2Value = stream.ReadUInt16();
                    temp.Effect3Value = stream.ReadUInt16();
                    temp.Desc = Encode.GetString(stream.ReadBytes(32)).Trim('\u0000');
                    temp.Unk = stream.ReadByte();
                    temp.Id = stream.ReadByte();
                    stream.ReadInt16();
                    var r = stream.ReadByte();
                    var g = stream.ReadByte();
                    var b = stream.ReadByte();
                    var a = stream.ReadByte();
                    temp.Rgba = Color.FromArgb(a, r, g, b);
                    i++;

                    if (temp.UnkId == 0) continue;
                    list.Add(temp);
                    txt.AppendLine($"INSERT INTO `data_titles` VALUES (" +
                                   $"{temp.LoopId}, {temp.Id}, {temp.UnkId}, {temp.MobsKilled}, " +
                                   $"{temp.Effect1}, {temp.Effect2}, {temp.Effect3}, {temp.Effect1Value}, {temp.Effect2Value}, {temp.Effect3Value}, " +
                                   $"'{temp.Desc.Trim()}', {temp.Unk}, '{temp.Rgba}');");
                }

                SqlData = txt.ToString();
                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}