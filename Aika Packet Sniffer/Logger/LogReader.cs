using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Aika_Packet_Sniffer.Logger.Xml;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Logger
{
    public class LogReader
    {
        private readonly string _file;
        private readonly Action<List<ListViewModel>> _onFinish;
        private readonly byte[] _data;

        private readonly List<ListViewModel> _packets;

        public LogReader(byte[] data, Action<List<ListViewModel>> onFinish)
        {
            _packets = new List<ListViewModel>();
            _data = data;
            _onFinish = onFinish;
        }

        public LogReader(string file, Action<List<ListViewModel>> onFinish)
        {
            _packets = new List<ListViewModel>();
            _file = file;
            _onFinish = onFinish;
        }

        public void Load()
        {
            if (!File.Exists(_file)) return;

            var fileThread = new Thread(LoadFileThread);
            fileThread.Start();
        }

        public void Parse()
        {
            if (_data == null) return;

            var parseThread = new Thread(ParseStreamThread);
            parseThread.Start();
        }

        private void LoadFileThread()
        {
            try
            {
                var stream = File.OpenRead(_file);
                ParseStream(stream, false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _onFinish(_packets);
            }
        }

        private void ParseStreamThread()
        {
            try
            {
                using (var stream = new MemoryStream(_data))
                {
                    ParseStream(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
            finally
            {
                _onFinish(_packets);
            }
        }

        private void ParseStream(Stream stream, bool fromStream = true)
        {
            using (var reader = new BinaryReader(stream))
            {
                var i = 0u;
                while (reader.BaseStream.Length != reader.BaseStream.Position)
                {
                    var origin = (PacketOrigin) reader.ReadByte();
                    var port = reader.ReadUInt16();
                    var size = reader.ReadUInt16();
                    reader.BaseStream.Position -= 2;
                    _packets.Add(FormatData(reader.ReadBytes(size), origin, port, fromStream ? 0 : i));
                    i++;
                }
            }
        }

        private static ListViewModel FormatData(byte[] data, PacketOrigin origin, ushort port, uint index)
        {
            var packet = new ListViewModel();
            using (var ms = new MemoryStream(data))
            {
                using (var stream = new BinaryReader(ms))
                {
                    var size = stream.ReadUInt16();
                    stream.ReadInt32();
                    var opcode = stream.ReadUInt16();
                    var time = stream.ReadInt32();
                    var template = new PacketListView
                    {
                        Lenght = size,
                        Index = index,
                        Time = time.ToString(),
                        Opcode = $"{opcode:x2}",
                        Origin = origin,
                        Data = data,
                        Port = port,
                        Name = ""
                    };
                    var parser = new Parser(data, origin, port, opcode);
                    var packetName = "";
                    packet.PacketParseListView = parser.ParseData(ref packetName);
                    template.Name = packetName;
                    packet.PacketListView = template;
                }
            }

            return packet;
        }
    }
}