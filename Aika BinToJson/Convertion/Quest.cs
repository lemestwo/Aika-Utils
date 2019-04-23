using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
	public class Quest : BaseConvert
	{
		public Quest(string path, string outPath) : base(path, outPath)
		{
		}

		public override void Convert()
		{
			using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
			{
				var size = stream.BaseStream.Length;
				ushort i = 0;

				var list = new List<QuestJson>();
				// 2696 bytes each
				while (stream.BaseStream.Position < size)
				{
					var temp = new QuestJson();
					temp.LoopId = i;
					temp.Id = stream.ReadUInt16();
					temp.Unk = stream.ReadInt16();
					temp.StartNpc = stream.ReadInt32();
					temp.EndNpc = stream.ReadInt32();
					temp.Unk2 = stream.ReadInt32();
					temp.Name = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000');
					temp.StartDialog = stream.ReadInt16();
					temp.UnfinishedDialog = stream.ReadInt16();
					temp.EndDialog = stream.ReadInt16();
					temp.Summary = stream.ReadInt16();
					stream.ReadBytes(1155);
					temp.Level = stream.ReadByte();
					temp.Unk3 = stream.ReadInt16();
					temp.Unk4 = stream.ReadInt16();
					temp.Unk5 = stream.ReadInt32();
					temp.Unk6 = stream.ReadInt32();
					temp.PreConditions = new List<QuestData>();
					temp.Requires = new List<QuestData>();
					temp.Rewards = new List<QuestData>();
					temp.Removes = new List<QuestData>();
					temp.Choices = new List<QuestData>();
					temp.Misc = new List<QuestData>();

					for (var j = 0; j < 30; j++) // 0-4
					{
						var temp2 = new QuestData
						{
							TypeId = stream.ReadInt32(),
							Quantity1 = stream.ReadInt32(),
							Quantity2 = stream.ReadInt32(),
							Unk1 = stream.ReadInt32(),
							ItemId1 = stream.ReadUInt16(),
							ItemId2 = stream.ReadUInt16(),
							Unk2 = stream.ReadInt32(),
							Unk3 = stream.ReadInt16(),
							Unk8 = stream.ReadInt16(),
							Unk4 = stream.ReadInt32(),
							Quantity3 = stream.ReadInt32(),
							Unk5 = stream.ReadInt32(),
							Unk6 = stream.ReadInt32(),
							Unk7 = stream.ReadInt32(),
						};
						if (temp2.TypeId <= 0) continue;

						if (j >= 0 && j < 5) // 5
							temp.PreConditions.Add(temp2);
						else if (j >= 5 && j < 10) // 5
							temp.Requires.Add(temp2);
						else if (j >= 10 && j < 18) // 8
							temp.Rewards.Add(temp2);
						else if (j >= 18 && j < 23) // 5
							temp.Removes.Add(temp2);
						else if (j >= 23 && j < 26) // 3
							temp.Choices.Add(temp2);
						else if (j >= 26 && j < 30) // 4
							temp.Misc.Add(temp2);
					}

					i++;

					if (temp.Id != 0)
						list.Add(temp);
				}

				JsonData = JsonConvert.SerializeObject(list);
			}
		}
	}
}