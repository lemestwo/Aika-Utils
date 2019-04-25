namespace Aika_BinToJson.Models
{
    public class MapJson
    {
        public ushort LoopId { get; set; }
        public CoordRec Coord { get; set; }

        public string Name { get; set; }

        // public string Desc { get; set; }
        public int Unk1 { get; set; }
        public int Unk2 { get; set; }
        public short Unk3 { get; set; }
        public int Unk4 { get; set; }
    }

    public class CoordRec
    {
        public int X1 { get; set; }
        public int Y1 { get; set; }
        public int X2 { get; set; }
        public int Y2 { get; set; }
    }
}