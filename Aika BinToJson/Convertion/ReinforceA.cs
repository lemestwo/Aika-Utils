using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
	public class ReinforceA : BaseConvert
	{
		public ReinforceA(string path, string outPath) : base(path, outPath)
		{
		}

		public override void Convert()
		{
			using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
			{
				var size = stream.BaseStream.Length;
				ushort i = 0;

				var list = new List<ReinforceAJson>();
				while (stream.BaseStream.Position < size - 4)
				{
					var temp = new ReinforceAJson
					{
						LoopId = i,
						Unk = stream.ReadInt32(),
						Price = stream.ReadInt32(),
						Chance = new ushort[16]
					};

					for (var j = 0; j < 16; j++)
						temp.Chance[j] = stream.ReadUInt16();

					i++;

					if (temp.Price != 999999)
						list.Add(temp);
				}

				JsonData = JsonConvert.SerializeObject(list);
			}
		}
	}

	public class ReinforceAJson
	{
		public ushort LoopId { get; set; }
		public int Unk { get; set; }
		public int Price { get; set; }
		public ushort[] Chance { get; set; }
	}
}