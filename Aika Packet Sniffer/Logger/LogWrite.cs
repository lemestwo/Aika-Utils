using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Aika_Packet_Sniffer.Model;

namespace Aika_Packet_Sniffer.Logger
{
    public class LogWrite
    {
        private readonly Action<List<ListViewModel>> _onFinish;
        private List<byte[]> _packets;
        private byte[] storedData;

        public LogWrite(Action<List<ListViewModel>> onFinish)
        {
            _onFinish = onFinish;
            _packets = new List<byte[]>();
            storedData = new byte[0];
        }

        public void WritePacket(byte[] data, PacketOrigin origin, ushort port)
        {
            if (data.Length > 0 && data[0] == 0x11 && data[1] == 0xF3)
            {
                var newData = new byte[data.Length - 4];
                Array.Copy(data, 4, newData, 0, data.Length - 4);
                data = newData;
            }

            var fullData = new byte[storedData.Length + data.Length];
            Buffer.BlockCopy(storedData, 0, fullData, 0, storedData.Length);
            Buffer.BlockCopy(data, 0, fullData, storedData.Length, data.Length);

            using (var ms = new MemoryStream(fullData))
            {
                using (var stream = new BinaryReader(ms))
                {
                    while (stream.BaseStream.Position < stream.BaseStream.Length)
                    {
                        var size = BitConverter.ToUInt16(stream.ReadBytes(2), 0);
                        stream.BaseStream.Position -= 2;
                        if (stream.BaseStream.Position + size > stream.BaseStream.Length)
                        {
                            var pData = stream.ReadBytes((int) (stream.BaseStream.Length - stream.BaseStream.Position));
                            storedData = pData;
                        }
                        else
                        {
                            var pData = stream.ReadBytes(size);
                            AddPacket(pData, origin, port);
                        }

                        // TODO - Find whats causing the problem
                        if (stream.BaseStream.Length > 7000)
                        {
                            storedData = new byte[0];
                            return;
                        }
                    }
                }
            }
        }

        private void AddPacket(byte[] data, PacketOrigin origin, ushort port)
        {
            var decryption = new Decryption(data);
            if (!decryption.Decrypt()) return;

            data = decryption.Data;
            var newData = new byte[data.Length + 3];
            using (var stream = new MemoryStream(newData.Length))
            {
                stream.WriteByte((byte) origin);
                stream.Write(BitConverter.GetBytes(port), 0, 2);
                stream.Write(data, 0, data.Length);
                newData = stream.GetBuffer();
            }

            _packets.Add(newData);
            var logReader = new LogReader(newData, _onFinish);
            logReader.Parse();
        }
    }
}