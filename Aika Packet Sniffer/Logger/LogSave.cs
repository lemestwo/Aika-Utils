using System;
using System.Collections.Generic;
using System.IO;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Logger
{
    public class LogSave
    {
        private readonly List<ListViewModel> _packets;

        public LogSave(List<ListViewModel> packets)
        {
            _packets = packets;
        }

        public void Save(string file)
        {
            try
            {
                using (var stream = new FileStream(file, FileMode.Create))
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (var packet in _packets)
                    {
                        writer.Write((byte) packet.PacketListView.Origin);
                        writer.Write(packet.PacketListView.Port);
                        writer.Write(packet.PacketListView.Data);
                    }

                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}