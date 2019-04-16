using System.Collections.Generic;

namespace Aika_BinToJson.Models
{
    public class MobPosJson
    {
        public ushort LoopId { get; set; }
        public ushort MobId { get; set; }
        public string Name { get; set; }
        public List<MobPosition> Position { get; set; }
    }

    public class MobPosition
    {
        public ushort CoordX { get; set; }
        public ushort CoordY { get; set; }
    }
}