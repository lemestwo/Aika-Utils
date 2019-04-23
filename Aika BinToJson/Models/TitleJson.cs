using System.Windows.Media;

namespace Aika_BinToJson.Models
{
	public class TitleJson
	{
		public ushort LoopId { get; set; }
		public ushort UnkId { get; set; }
		public ushort MobsKilled { get; set; }
		public ushort Effect1 { get; set; }
		public ushort Effect2 { get; set; }
		public ushort Effect3 { get; set; }
		public ushort Effect1Value { get; set; }
		public ushort Effect2Value { get; set; }
		public ushort Effect3Value { get; set; }
		public string Desc { get; set; }
		public byte Unk { get; set; }
		public byte Id { get; set; }
		public Color Rgba { get; set; }
	}
}