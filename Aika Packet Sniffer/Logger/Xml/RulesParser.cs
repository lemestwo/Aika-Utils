using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Xml;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Logger.Xml
{
    public static class RulesParser
    {
        private static readonly List<RulesModel> Nodes = new List<RulesModel>();

        public static void Init()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("packet_rules.xml");
            var rootElement = xmlDoc.DocumentElement;
            if (rootElement == null || rootElement.Name != "root") return;

            foreach (XmlNode pType in rootElement.ChildNodes)
            {
                ushort port = 0;
                if (pType.Name == "type" && pType.Attributes != null)
                {
                    var portXml = pType.Attributes["port"];
                    if (portXml != null)
                        port = ushort.Parse(portXml.Value);

                    foreach (XmlNode pTypeInfos in pType.ChildNodes)
                    {
                        var template = new RulesModel
                        {
                            Port = port,
                            Packets = pTypeInfos
                        };
                        Nodes.Add(template);
                    }
                }
            }
        }

        public static RulesModel GetRule(ushort port, string origin)
        {
            foreach (var node in Nodes)
            {
                if (node.Port == port && node.Packets.Name == origin) return node;
            }

            return null;
        }
    }
}