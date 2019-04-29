namespace Aika_Packet_Sniffer.PacketDump.Model
{
    public class StoreData
    {
        public ushort NpcId { get; set; }
        public ushort StoreType { get; set; }
        public ushort[] Items { get; set; }
    }
}