using Aika_Packet_Sniffer.Logger;

namespace Aika_Packet_Sniffer.Model
{
    public class PacketListView
    {
        public uint Index { get; set; }
        public string Time { get; set; }
        public ushort Lenght { get; set; }
        public string Opcode { get; set; }
        public string Name { get; set; }
        public ushort Port { get; set; }
        
        public PacketOrigin Origin { get; set; }
        public byte[] Data { get; set; }
    }
}