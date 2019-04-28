using System.Collections.Generic;

namespace Aika_Packet_Sniffer.PacketDump.Model
{
    public class DialogOptions
    {
        public uint NpcId { get; set; }
        public int SoundId { get; set; }
        public int SoundType { get; set; }
        public List<DialogData> DialogData { get; set; }

        public DialogOptions()
        {
            DialogData = new List<DialogData>();
        }
    }

    public class DialogData
    {
        public uint OptionId { get; set; }
        public uint SubOptionId { get; set; }
        public string Text { get; set; }
        public int Unk { get; set; }
    }
}