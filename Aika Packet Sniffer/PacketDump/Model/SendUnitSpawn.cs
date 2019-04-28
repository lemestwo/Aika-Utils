using System.Collections.Generic;

namespace Aika_Packet_Sniffer.PacketDump.Model
{
    public class SendUnitSpawn
    {
        public ushort ConnectionId { get; set; }
        public string Name { get; set; }
        public ushort NpcId { get; set; }
        public ushort Hair { get; set; }
        public ushort Face { get; set; }
        public ushort Helmet { get; set; }
        public ushort Armor { get; set; }
        public ushort Gloves { get; set; }
        public ushort Pants { get; set; }
        public ushort Weapon { get; set; }
        public ushort Shield { get; set; }
        public byte[] Refinements { get; set; }
        public float CoordX { get; set; }
        public float CoordY { get; set; }
        public short Rotation { get; set; }
        public int Hp { get; set; }
        public int Mp { get; set; }
        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public ushort Unk { get; set; }
        public short SpawnType { get; set; }
        public short Width { get; set; }
        public short Chest { get; set; }
        public short Leg { get; set; }
        public ushort ConId { get; set; }
        public short Unk2 { get; set; }
        public List<ushort> Buffs { get; }
        public List<int> Unk3 { get; }
        public string Title { get; set; }
        public ushort Unk4 { get; set; }

        public SendUnitSpawn()
        {
            Buffs = new List<ushort>();
            Unk3 = new List<int>();
        }
    }
}