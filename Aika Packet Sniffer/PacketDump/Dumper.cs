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
        public static bool IsEnabled = false;

        private static uint _lastNpcId = 0;
        private static DialogOptions _dialogTemp;
        private static Dictionary<uint, SendUnitSpawn> _parsedNpcs; // conId, Data
        private static List<ushort> _storeType;
        private const string StoreTypePath = Path + "Enums\\StoreType.json";
        private static readonly Encoding Encoding = Encoding.GetEncoding("iso-8859-1");

        private static void NewStoreType(ushort id)
        {
            _storeType.Add(id);
            var temp = _storeType.Distinct().ToList();
            SaveToFile(StoreTypePath, temp, false);
        }

        public static void ParseStoreItems(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding))
            {
                stream.ReadBytes(12);
                var temp = new StoreData();
                var conId = stream.ReadUInt16();
                temp.StoreType = stream.ReadUInt16();
                NewStoreType(temp.StoreType);
                temp.Items = new ushort[40];
                for (var i = 0; i < 40; i++)
                    temp.Items[i] = stream.ReadUInt16();

                if (!_parsedNpcs.ContainsKey(conId)) return;
                temp.NpcId = _parsedNpcs[conId].NpcId;

                var pathFile = $"{Path}Npcs\\{temp.NpcId}\\StoreData.json";
                SaveToFile(pathFile, temp);
            }
        }

        public static void ParseCloseChat()
        {
            if (_lastNpcId <= 0 || _dialogTemp == null || _dialogTemp.DialogData.Count <= 0) return;

            _dialogTemp.NpcId = _lastNpcId;
            var pathFile = $"{Path}Npcs\\{_lastNpcId}\\DialogData.json";
            SaveToFile(pathFile, _dialogTemp);

            _lastNpcId = 0;
            _dialogTemp = null;
        }

        public static void ParseOption(byte[] data)
        {
            if (_lastNpcId <= 0 || _dialogTemp == null) return;
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding))
            {
                stream.ReadBytes(12);
                var temp = new DialogData
                {
                    OptionId = stream.ReadUInt32(),
                    SubOptionId = stream.ReadUInt32(),
                    Text = Encoding.GetString(stream.ReadBytes(60)).Trim('\u0000'),
                    Unk = stream.ReadInt32()
                };
                _dialogTemp.DialogData.Add(temp);
            }
        }

        public static void ParsePlaySound(byte[] data)
        {
            if (_lastNpcId <= 0 || _dialogTemp == null) return;
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding))
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
            using (var stream = new BinaryReader(ms, Encoding))
            {
                stream.ReadBytes(12);
                var conId = stream.ReadUInt32();
                if (_parsedNpcs.ContainsKey(conId))
                {
                    _lastNpcId = _parsedNpcs[conId].NpcId;
                }
            }
        }

        public static void ParseMobTofile(byte[] data)
        {
            var temp = new MobData();
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding))
            {
                stream.ReadBytes(12);
                temp.MobModel = stream.ReadUInt32();
                stream.ReadBytes(8);
                temp.Unk7 = stream.ReadInt32();
                stream.ReadBytes(12);
                temp.Hp1 = stream.ReadUInt32();
                temp.Hp2 = stream.ReadUInt32();
                temp.Hp3 = stream.ReadUInt32();
                stream.ReadBytes(5);
                temp.Unk1 = stream.ReadByte();
                temp.Unk2 = stream.ReadByte();
                temp.Unk3 = stream.ReadByte();
                stream.ReadBytes(9);
                temp.Width = stream.ReadByte();
                temp.Chest = stream.ReadByte();
                temp.Leg = stream.ReadByte();
                stream.ReadBytes(2);
                temp.Unk4 = stream.ReadByte();
                temp.Unk5 = stream.ReadByte();
                temp.MobId = stream.ReadUInt32();
                temp.Unk6 = stream.ReadUInt32();
            }

            var pathFile = $"{Path}Mobs\\{temp.MobId}\\MobData.json";
            SaveToFile(pathFile, temp);
        }

        public static void ParseNpcToFile(byte[] data)
        {
            var temp = new SendUnitSpawn();
            using (var ms = new MemoryStream(data))
            using (var stream = new BinaryReader(ms, Encoding))
            {
                stream.ReadInt32();
                temp.ConnectionId = stream.ReadUInt16();
                stream.ReadUInt16();
                stream.ReadInt32();
                temp.Name = Encoding.GetString(stream.ReadBytes(16)).Trim('\u0000');
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

                for (var i = 0; i < 120; i++)
                {
                    var tUnk = stream.ReadUInt16();
                    if (tUnk > 0) temp.Unk3.Add(tUnk);
                }

                temp.Title = Encoding.GetString(stream.ReadBytes(32)).Trim('\u0000');
                stream.ReadInt32();
                temp.Unk4 = stream.ReadUInt16();
            }

            if (!ushort.TryParse(temp.Name, out var npcId)) return;

            temp.NpcId = npcId;
            temp.NpcIdX = NpcPos.GetNpcIdX(temp.NpcId);

            var pathFile = NpcPos.IsStaticNpc(npcId)
                ? $"{Path}Npcs\\{temp.NpcId}\\UnitData.json"
                : $"{Path}Npcs\\NonStatic\\{temp.NpcId}\\UnitData.json";

            SaveToFile(pathFile, temp);
        }

        private static void SaveToFile(string pathFile, object data, bool checkFileExists = true)
        {
            new FileInfo(pathFile).Directory?.Create();
            if (checkFileExists && File.Exists(pathFile)) return;

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

            var storeTypeData = File.ReadAllText(StoreTypePath);
            _storeType = JsonConvert.DeserializeObject<List<ushort>>(storeTypeData) ?? new List<ushort>();
        }
    }
}