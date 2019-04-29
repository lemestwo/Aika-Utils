using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;
using Aika_Packet_Sniffer.Model;
using Aika_Packet_Sniffer.PacketDump;

namespace Aika_Packet_Sniffer.Logger.Xml
{
    public class Parser
    {
        private readonly byte[] _data;
        private readonly PacketOrigin _origin;
        private readonly ushort _port;
        private readonly ushort _opcode;
        private long _pos;

        public Parser(byte[] data, PacketOrigin origin, ushort port, ushort opcode)
        {
            _data = data;
            _origin = origin;
            _port = port;
            _opcode = opcode;
            _pos = 0;

            if (!Dumper.IsEnabled) return;
            switch (opcode)
            {
                case 0x3049:
                    Dumper.ParseNpcToFile(data);
                    break;
                case 0x100e:
                    Dumper.ParseOpenChat(data);
                    break;
                case 0x1015:
                    Dumper.ParsePlaySound(data);
                    break;
                case 0x1012:
                    Dumper.ParseOption(data);
                    break;
                case 0x100f:
                    Dumper.ParseCloseChat();
                    break;
                case 0x1006:
                    Dumper.ParseStoreItems(data);
                    break;
            }
        }

        public List<PacketParseListView> ParseData(ref string packetName)
        {
            var list = new List<PacketParseListView>();
            var rules = RulesParser.GetRule(_port, _origin == PacketOrigin.ClientToServer ? "client" : "server");
            if (rules == null) return null;

            foreach (XmlNode node in rules.Packets)
            {
                switch (node.Name)
                {
                    case "header":
                    {
                        foreach (XmlNode param in node.ChildNodes)
                        {
                            var result = ParseParamNode(param);
                            if (result != null) list.Add(result);
                        }

                        break;
                    }
                    case "packet" when node.Attributes != null:
                    {
                        var opcode = (ushort) ParseLong(node.Attributes["opcode"].Value);
                        if (_opcode != opcode) continue;

                        packetName = node.Attributes["name"].Value;
                        foreach (XmlNode param in node.ChildNodes)
                        {
                            switch (param.Name)
                            {
                                case "param" when param.Attributes != null:
                                {
                                    var result = ParseParamNode(param);
                                    if (result != null) list.Add(result);
                                    break;
                                }
                                case "loop" when param.Attributes != null:
                                {
                                    var count = "0";
                                    if (param.Attributes["count"] != null)
                                        count = param.Attributes["count"].Value;

                                    for (var i = 0; i < int.Parse(count); i++)
                                    {
                                        list.Add(ParseParam("", $"Loop {i + 1}/{count}", "0"));
                                        foreach (XmlNode loopNode in param.ChildNodes)
                                        {
                                            var result = ParseParamNode(loopNode);
                                            if (result != null) list.Add(result);
                                        }
                                    }

                                    list.Add(ParseParam("", "Loop End", "0"));
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
            }

            return list;
        }

        private PacketParseListView ParseParamNode(XmlNode node)
        {
            if (node.Name != "param" || node.Attributes == null) return null;

            var type = "";
            var name = "";
            var size = "0";
            if (node.Attributes["type"] != null)
                type = node.Attributes["type"].Value;
            if (node.Attributes["name"] != null)
                name = node.Attributes["name"].Value;
            if (node.Attributes["size"] != null)
                size = node.Attributes["size"].Value;
            return ParseParam(type, name, size);
        }

        private PacketParseListView ParseParam(string type, string name, string size)
        {
            PacketParseListView param;
            var intSize = int.Parse(size);
            using (var ms = new MemoryStream(_data))
            using (var stream = new BinaryReader(ms))
            {
                if (stream.BaseStream.Length < _pos) return null;

                stream.BaseStream.Position = _pos;
                var tempStart = _pos;
                object value;
                switch (type)
                {
                    case "int":
                        value = stream.ReadInt32();
                        break;
                    case "uint":
                        value = stream.ReadUInt32();
                        break;
                    case "byte":
                        value = stream.ReadByte();
                        break;
                    case "sbyte":
                        value = stream.ReadSByte();
                        break;
                    case "bytes":
                        stream.ReadBytes(intSize);
                        value = $"Byte[{intSize}]";
                        break;
                    case "short":
                        value = stream.ReadInt16();
                        break;
                    case "ushort":
                        value = stream.ReadUInt16();
                        break;
                    case "long":
                        value = stream.ReadInt64();
                        break;
                    case "ulong":
                        value = stream.ReadUInt64();
                        break;
                    case "bool":
                        value = stream.ReadByte() == 1;
                        break;
                    case "float":
                        value = stream.ReadSingle();
                        break;
                    case "mac":
                        value = BitConverter.ToString(stream.ReadBytes(6)).Replace("-", ":");
                        break;
                    case "timei":
                        var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        var seconds = (long) stream.ReadInt32();
                        value = dtDateTime.AddSeconds(seconds);
                        break;
                    case "string":
                        var tmpByte = stream.ReadBytes(intSize);
                        for (var i = 0; i < tmpByte.Length; i++)
                            if (tmpByte[i].Equals(0xCC))
                                tmpByte[i] = 0x00;
                        value = Encoding.UTF8.GetString(tmpByte).Trim('\u0000');
                        break;
                    default:
                        value = "";
                        break;
                }

                // TODO - Need to find behavior for 255+ players online
                if (name == "hHash2" && type == "ushort" && (ushort) value > 250 && (ushort) value < 400)
                {
                    MessageBox.Show("found it!");
                }

                _pos = stream.BaseStream.Position;
                var tempEnd = stream.BaseStream.Position;
                var template = new PacketParseListView
                {
                    Type = type,
                    Name = name,
                    Value = value.ToString(),
                    Start = (int) tempStart,
                    End = (int) tempEnd
                };
                param = template;
            }

            return param;
        }

        public static long ParseLong(string v)
        {
            return v.IndexOf('x') == -1 ? long.Parse(v) : long.Parse(v.Substring(2), NumberStyles.HexNumber);
        }
    }
}