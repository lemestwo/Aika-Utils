namespace Aika_BinToJson.Models
{
	public class MakeItemsJson
	{
		public ushort LoopId { get; set; }
		public uint TargetItemId { get; set; }
		public uint Price { get; set; }
		public byte Quantity { get; set; }
		public byte Level { get; set; }
		public ushort Rate { get; set; }
		public ushort SuperiorRate { get; set; }
		public ushort DoubleRate { get; set; }
		public ushort[] IngredientsItemId { get; set; }
		public ushort[] IngredientsQuantity { get; set; }
	}
}