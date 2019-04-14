using System.Xml;

namespace Aika_Packet_Sniffer.Logger.Xml
{
    public class RulesModel
    {
        public ushort Port { get; set; }
        public XmlNode Packets { get; set; }
    }
}