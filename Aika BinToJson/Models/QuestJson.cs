using System.Collections.Generic;

namespace Aika_BinToJson.Models
{
	public class QuestJson
	{
		public ushort LoopId { get; set; }
		public ushort Id { get; set; }
		public short Unk { get; set; }
		public int StartNpc { get; set; }
		public int EndNpc { get; set; }
		public int Unk2 { get; set; }
		public string Name { get; set; }
		public short StartDialog { get; set; }
		public short UnfinishedDialog { get; set; }
		public short EndDialog { get; set; }
		public short Summary { get; set; }
		public ushort Level { get; set; }
		public short Unk3 { get; set; }
		public short Unk4 { get; set; }
		public int Unk5 { get; set; }
		public int Unk6 { get; set; }

		public List<QuestData> PreConditions { get; set; }
		public List<QuestData> Requires { get; set; }
		public List<QuestData> Rewards { get; set; }
		public List<QuestData> Removes { get; set; }
		public List<QuestData> Choices { get; set; }
		public List<QuestData> Misc { get; set; }
	}

	public class QuestData
	{
		public int TypeId { get; set; }
		public int Quantity1 { get; set; }
		public int Quantity2 { get; set; }
		public int Unk1 { get; set; }
		public ushort ItemId1 { get; set; }
		public ushort ItemId2 { get; set; }
		public int Unk2 { get; set; }
		public short Unk3 { get; set; }
		public short Unk8 { get; set; }
		public int Unk4 { get; set; }
		public int Quantity3 { get; set; }
		public int Unk5 { get; set; }
		public int Unk6 { get; set; }
		public int Unk7 { get; set; }
	}
}