using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
	public class SetItem : BaseConvert
	{
		public SetItem(string path, string outPath) : base(path, outPath)
		{
		}

		public override void Convert()
		{
			using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
			{
				var size = stream.BaseStream.Length;
				ushort i = 0;

				var list = new List<SetItemJson>();
				while (stream.BaseStream.Position < size - 4)
				{
					var temp = new SetItemJson
					{
						LoopId = i,
						Name = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
						Name2 = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000'),
						Quantity = stream.ReadUInt32(),
						ItemsId = new ushort[12],
						EffectNumber = new ushort[6],
						Unk = new ushort[6],
						CastChance = new ushort[6],
						EffectId = new ushort[6],
						EffectValue = new ushort[6],
					};

					for (var j = 0; j < 12; j++)
						temp.ItemsId[j] = stream.ReadUInt16();

					for (var j = 0; j < 6; j++)
						temp.EffectNumber[j] = stream.ReadUInt16();

					for (var j = 0; j < 6; j++)
						temp.Unk[j] = stream.ReadUInt16();

					for (var j = 0; j < 6; j++)
						temp.CastChance[j] = stream.ReadUInt16();

					for (var j = 0; j < 6; j++)
						temp.EffectId[j] = stream.ReadUInt16();

					for (var j = 0; j < 6; j++)
						temp.EffectValue[j] = stream.ReadUInt16();

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
}