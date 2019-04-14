using System.Collections.Generic;

namespace Aika_Packet_Sniffer.Model
{
    public class ListViewModel
    {
        public PacketListView PacketListView { get; set; }
        public List<PacketParseListView> PacketParseListView { get; set; }
    }
}