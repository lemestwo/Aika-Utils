using System.Collections.Generic;
using System.IO;
using System.Text;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class ItemList : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<ItemsJson>();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new ItemsJson();
                    temp.LoopId = i;
                    temp.ItemName = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000');
                    temp.ItemName2 = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000');
                    temp.Description = Encode.GetString(stream.ReadBytes(128)).Trim('\u0000');

                    temp.IsStackable = stream.ReadUInt16() == 1;
                    temp.ItemType = stream.ReadUInt16();
                    temp.CaeliumId = stream.ReadUInt32();
                    temp.SubType = stream.ReadInt32();
                    temp.GearCoreLevel = stream.ReadUInt32();
                    temp.Unk6 = stream.ReadUInt32(); // buff related
                    temp.Unk7 = stream.ReadUInt32(); // buff related
                    temp.Unk8 = stream.ReadUInt32(); // buff related
                    temp.HonorCost = stream.ReadUInt32();
                    temp.MedalCost = stream.ReadUInt32();
                    temp.BuyPrice = stream.ReadUInt32();
                    temp.SellPrice = stream.ReadUInt32();
                    temp.Profession = stream.ReadUInt16();
                    temp.Unk9 = stream.ReadUInt16(); // kind of face / monster / mount
                    temp.Unk10 = stream.ReadUInt16();
                    temp.Unk11 = stream.ReadUInt16();
                    temp.Unk12 = stream.ReadUInt16();
                    temp.Unk13 = stream.ReadByte();
                    temp.Unk14 = stream.ReadByte();
                    temp.Unk15 = stream.ReadUInt16();
                    temp.Unk16 = stream.ReadUInt16();
                    temp.Unk17 = stream.ReadUInt16();
                    temp.Unk18 = stream.ReadUInt16();
                    temp.ImageId = stream.ReadUInt16();
                    temp.Unk19 = stream.ReadUInt16();
                    // 68
                    temp.Unk20 = stream.ReadUInt16();
                    temp.Unk21 = stream.ReadUInt16();
                    temp.Unk22 = stream.ReadUInt16();
                    // 74
                    temp.MinLevel = stream.ReadUInt16();
                    temp.Unk23 = stream.ReadByte();
                    temp.Unk24 = stream.ReadByte();
                    stream.ReadInt16(); // empty
                    //80
                    temp.TimeLimit = stream.ReadInt32();
                    stream.ReadByte(); // empty
                    temp.Unk25 = stream.ReadByte() == 1; // consumable / food
                    temp.DressId = stream.ReadByte();
                    temp.IsHolyWater = stream.ReadByte() == 1;
                    stream.ReadUInt32(); // empty
                    stream.ReadUInt32(); // empty
                    temp.Unk28 = stream.ReadUInt16();
                    stream.ReadUInt32(); // empty
                    temp.PAtk = stream.ReadUInt16();
                    temp.PDef = stream.ReadUInt16();
                    temp.MAtk = stream.ReadUInt16();
                    temp.MDef = stream.ReadUInt16();
                    stream.ReadUInt16(); // empty
                    temp.Always30 = stream.ReadUInt16(); // potions food and boxes, always 30
                    temp.Unk30 = stream.ReadUInt16();
                    stream.ReadUInt32(); // empty
                    temp.Unk31 = stream.ReadUInt16();
                    temp.Unk32 = stream.ReadUInt16(); // only no image item
                    stream.ReadUInt16(); // empty
                    temp.Unk33 = stream.ReadUInt16(); // colored sets with more parts
                    stream.ReadUInt16(); // empty
                    stream.ReadUInt16(); // empty
                    stream.ReadUInt16(); // empty
                    temp.Quality = stream.ReadUInt16();
                    // 136
                    temp.Tradeable = (stream.ReadUInt16() & 256) == 0;
                    stream.ReadUInt32(); // empty
                    stream.ReadUInt32(); // empty
                    stream.ReadUInt32(); // empty
                    temp.Durability = stream.ReadUInt16();
                    stream.ReadUInt16(); // empty
                    stream.ReadUInt16(); // empty
                    temp.Effect1 = stream.ReadUInt16();
                    temp.Effect2 = stream.ReadUInt16();
                    temp.Effect3 = stream.ReadUInt16();
                    temp.Effect1Value = stream.ReadUInt16();
                    temp.Effect2Value = stream.ReadUInt16();
                    temp.Effect3Value = stream.ReadUInt16();
                    temp.Unk37 = stream.ReadByte() == 1;
                    temp.Unk38 = stream.ReadByte() == 1;
                    //170
                    temp.Reinforceable = (stream.ReadByte() & 1) == 0;
                    temp.Rank = stream.ReadByte();
                    temp.Unk41 = stream.ReadByte();
                    temp.Unk42 = stream.ReadByte();
                    temp.TestItem = stream.ReadByte(); // item test, only one
                    stream.ReadByte();
                    temp.MaxLevel = stream.ReadUInt16();
                    stream.ReadUInt16(); // empty
                    temp.Unk44 = stream.ReadUInt16();
                    stream.ReadUInt16(); // empty
                    temp.Unk45 = stream.ReadByte();
                    temp.Unk46 = stream.ReadByte();
                    temp.Unk47 = stream.ReadUInt16();
                    temp.Unk48 = stream.ReadByte();
                    temp.Unk49 = stream.ReadByte();
                    temp.Unk50 = stream.ReadByte();
                    temp.Unk51 = stream.ReadByte();
                    temp.Unk52 = stream.ReadInt32();
                    temp.Unk53 = stream.ReadByte();
                    temp.Unk54 = stream.ReadByte();
                    temp.Always15 = stream.ReadUInt16(); // potions and bullet, always 15
                    temp.Unk56 = stream.ReadByte();
                    temp.Unk57 = stream.ReadByte();
                    temp.Unk58 = stream.ReadUInt16();
                    temp.Unk59 = stream.ReadByte() == 1;
                    temp.Unk60 = stream.ReadByte();
                    stream.ReadByte();
                    temp.Unk61 = stream.ReadByte() == 1;
                    i++;
                    if (!string.IsNullOrEmpty(temp.ItemName))
                    {
                        list.Add(temp);
                    }
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}