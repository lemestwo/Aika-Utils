using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
	public class NpcPos : BaseConvert
	{
		public NpcPos(string path, string outPath) : base(path, outPath)
		{
		}

		public override void Convert()
		{
			using (var stream = new BinaryReader(File.OpenRead(Path)))
			{
				var size = stream.BaseStream.Length;
				ushort i = 0;

				var list = new List<NpcPosJson>();
				stream.ReadInt32();
				while (stream.BaseStream.Position < size - 4)
				{
					var temp = new NpcPosJson
					{
						LoopId = i,
						NpcId = (ushort) stream.ReadUInt32(),
						CoordX = (ushort) stream.ReadUInt32(),
						CoordY = (ushort) stream.ReadUInt32()
					};
					i++;
					if (temp.NpcId > 0)
					{
						list.Add(temp);
					}
				}

				JsonData = JsonConvert.SerializeObject(list);
			}
		}
	}

	public class NpcPosJson
	{
		public ushort LoopId { get; set; }
		public ushort NpcId { get; set; }
		public ushort CoordX { get; set; }
		public ushort CoordY { get; set; }
	}
}