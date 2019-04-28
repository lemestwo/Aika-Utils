using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Aika_Packet_Sniffer.PacketDump.Model;
using Newtonsoft.Json;

namespace Aika_Packet_Sniffer.PacketDump
{
    public static class Dumper
    {
        public const string Path = "D:\\Aika Npc Dump\\";
        private static uint _lastNpcId = 0;
        private static DialogOptions _dialogTemp;
        private static Dictionary<uint, SendUnitSpawn> _parsedNpcs; // conId, Data

        public static void ParseCloseChat()
        {
            if (_lastNpcId <= 0 || _dialogTemp == null || _dialogTemp.DialogData.Count <= 0) return;

            var pathFile = $"{Path}Npcs\\{_lastNpcId}\\DialogData.json";
            SaveToFile(pathFile, _dialogTemp);

            _lastNpcId = 0;
            _dialogTemp = null;
        }

        public static void ParseOption(byte[] data)
        {
            if (_lastNpcId <= 0 || _dialogTemp == null) return;
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding.GetEncoding("iso-8859-1")))
            {
                stream.ReadBytes(12);
                var temp = new DialogData
                {
                    OptionId = stream.ReadUInt32(),
                    SubOptionId = stream.ReadUInt32(),
                    Text = Encoding.GetEncoding("iso-8859-1").GetString(stream.ReadBytes(60)).Trim('\u0000'),
                    Unk = stream.ReadInt32()
                };
                _dialogTemp.DialogData.Add(temp);
            }
        }

        public static void ParsePlaySound(byte[] data)
        {
            if (_lastNpcId <= 0 || _dialogTemp == null) return;
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding.GetEncoding("iso-8859-1")))
            {
                stream.ReadBytes(12);
                _dialogTemp.SoundId = stream.ReadInt32();
                _dialogTemp.SoundType = stream.ReadInt32();
            }
        }

        public static void ParseOpenChat(byte[] data)
        {
            _dialogTemp = new DialogOptions();
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding.GetEncoding("iso-8859-1")))
            {
                stream.ReadBytes(12);
                var conId = stream.ReadUInt32();
                if (_parsedNpcs.ContainsKey(conId))
                {
                    _lastNpcId = _parsedNpcs[conId].NpcId;
                }
            }
        }

        public static void ParseNpcToFile(byte[] data)
        {
            var temp = new SendUnitSpawn();
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding.GetEncoding("iso-8859-1")))
            {
                stream.ReadInt32();
                temp.ConnectionId = stream.ReadUInt16();
                stream.ReadUInt16();
                stream.ReadInt32();
                temp.Name = Encoding.GetEncoding("iso-8859-1").GetString(stream.ReadBytes(16)).Trim('\u0000');
                temp.Hair = stream.ReadUInt16();
                temp.Face = stream.ReadUInt16();
                temp.Helmet = stream.ReadUInt16();
                temp.Armor = stream.ReadUInt16();
                temp.Gloves = stream.ReadUInt16();
                temp.Pants = stream.ReadUInt16();
                temp.Weapon = stream.ReadUInt16();
                temp.Shield = stream.ReadUInt16();
                temp.Refinements = stream.ReadBytes(12);
                temp.CoordX = stream.ReadSingle();
                temp.CoordY = stream.ReadSingle();
                temp.Rotation = stream.ReadInt16();
                stream.ReadUInt16();
                temp.Hp = stream.ReadInt32();
                temp.Mp = stream.ReadInt32();
                temp.MaxHp = stream.ReadInt32();
                temp.MaxMp = stream.ReadInt32();
                temp.Unk = stream.ReadUInt16();
                temp.SpawnType = stream.ReadByte();
                temp.Width = stream.ReadByte();
                temp.Chest = stream.ReadByte();
                temp.Leg = stream.ReadByte();
                temp.ConId = stream.ReadUInt16();
                temp.Unk2 = stream.ReadInt16();
                stream.ReadUInt16();
                for (var i = 0; i < 60; i++)
                {
                    var tBuff = stream.ReadUInt16();
                    if (tBuff > 0) temp.Buffs.Add(tBuff);
                }

                for (var i = 0; i < 60; i++)
                {
                    var tUnk = stream.ReadInt32();
                    if (tUnk > 0) temp.Unk3.Add(tUnk);
                }

                temp.Title = Encoding.GetEncoding("iso-8859-1").GetString(stream.ReadBytes(32)).Trim('\u0000');
                stream.ReadInt32();
                temp.Unk4 = stream.ReadUInt16();
            }

            if (!ushort.TryParse(temp.Name, out var npcId)) return;

            temp.NpcId = npcId;

            var pathFile = NpcPos.IsStaticNpc(npcId)
                ? $"{Path}Npcs\\{temp.NpcId}\\UnitData.json"
                : $"{Path}Npcs\\NonStatic\\{temp.NpcId}\\UnitData.json";

            SaveToFile(pathFile, temp);
        }

        private static void SaveToFile(string pathFile, object data)
        {
            new FileInfo(pathFile).Directory?.Create();
            if (File.Exists(pathFile)) return;

            using (var sw = new StreamWriter(pathFile, false))
            using (var writer = new JsonTextWriter(sw))
            {
                var json = new JsonSerializer {Formatting = Formatting.Indented};
                json.Serialize(writer, data);
            }
        }

        public static void Init()
        {
            if (!NpcPos.LoadFile("Data\\npcpos.bin.json"))
            {
                throw new FileNotFoundException("Missing npcpos.bin.json file.");
            }

            _parsedNpcs = new Dictionary<uint, SendUnitSpawn>();
            _parsedNpcs.Clear();
            var filePaths = Directory.GetDirectories(Path + "Npcs\\").SelectMany(dir => Directory.GetFiles(dir, "*.json")).ToList();

            foreach (var file in filePaths)
            {
                var tData = File.ReadAllText(file);
                var temp = JsonConvert.DeserializeObject<SendUnitSpawn>(tData);
                if (!_parsedNpcs.ContainsKey(temp.ConnectionId))
                    _parsedNpcs.Add(temp.ConnectionId, temp);
            }
        }
    }
}