using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
	public class Title : BaseConvert
	{
		public Title(string path, string outPath) : base(path, outPath)
		{
		}

		public override void Convert()
		{
			using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
			{
				ushort i = 0;

				var list = new List<TitleJson>();
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
					if (temp.UnkId != 0)
					{
						list.Add(temp);
					}
				}

				JsonData = JsonConvert.SerializeObject(list);
			}
		}
	}
}