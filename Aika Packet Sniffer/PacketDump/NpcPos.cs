using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace Aika_Packet_Sniffer.PacketDump
{
    public static class NpcPos
    {
        private static List<NpcPosJson> _npcPositions;

        public static bool LoadFile(string path)
        {
            var filePath = Dumper.Path + path;
            if (!File.Exists(filePath)) return false;

            try
            {
                string content;
                using (var reader = File.OpenText(filePath))
                {
                    content = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(content)) return false;

                _npcPositions = JsonConvert.DeserializeObject<List<NpcPosJson>>(content);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public static bool IsStaticNpc(ushort id)
        {
            foreach (var npc in _npcPositions)
                if (npc.NpcId == id)
                    return true;

            return false;
        }
    }

    public class NpcPosJson
    {
        public ushort NpcId { get; set; }
    }
}