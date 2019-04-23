namespace Aika_BinToJson.Models
{
	public class RecipeJson
	{
		public ushort LoopId { get; set; }
		public uint ItemId { get; set; }
		public ushort TargetItemId { get; set; }
		public ushort TargetItemIdSup { get; set; }
		public uint Price { get; set; }
		public byte Quantity { get; set; }
		public byte MinLevel { get; set; }
		public ushort Chance { get; set; }
		public ushort SuperiorChance { get; set; }
		public ushort DoubleChance { get; set; }
		public ushort[] IngredientsItemId { get; set; }
		public uint[] IngredientsQuantity { get; set; }
	}
}