namespace Aika_BinToJson.Models
{
	public class SetItemJson
	{
		public ushort LoopId { get; set; }
		public string Name { get; set; }
		public string Name2 { get; set; }
		public uint Quantity { get; set; }
		public ushort[] ItemsId { get; set; }
		public ushort[] EffectNumber { get; set; }
		public ushort[] Unk { get; set; }
		public ushort[] CastChance { get; set; }
		public ushort[] EffectId { get; set; }
		public ushort[] EffectValue { get; set; }
	}
}