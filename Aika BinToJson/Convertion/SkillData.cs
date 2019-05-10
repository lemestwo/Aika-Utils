using System;
using System.Collections.Generic;
using System.IO;
using Aika_BinToJson.Models;
using Newtonsoft.Json;

namespace Aika_BinToJson.Convertion
{
    public class SkillData : BaseConvert
    {
        public override void Convert()
        {
            using (var stream = new BinaryReader(File.OpenRead(Path), Encode))
            {
                var size = stream.BaseStream.Length;
                ushort i = 0;

                var list = new List<SkillDataJson>();
                while (stream.BaseStream.Position < size - 4)
                {
                    var temp = new SkillDataJson();
                    temp.Id = i;
                    temp.Idx = (ushort)Math.DivRem(i, 16, out var rem);
                    if (rem == 0) temp.Idx--;
                    temp.IconId = stream.ReadUInt32();
                    temp.RequiredLevel = stream.ReadUInt32();
                    temp.MaxLevel = stream.ReadUInt32();
                    temp.Level = stream.ReadUInt32();
                    temp.Tier = (uint) (i > 6060 ? 0 : (temp.Id - 1) / (16 * 6) % 10);
                    temp.TierPos = (ushort) (i > 6060 ? 0 : (temp.Id - 1) / 16 % 6);
                    temp.Unk = stream.ReadInt32();
                    temp.Name = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000');
                    temp.Name2 = Encode.GetString(stream.ReadBytes(64)).Trim('\u0000');
                    temp.LearnSkillPoint = stream.ReadUInt32();
                    temp.LearnPrice = stream.ReadUInt32();
                    temp.Profession = stream.ReadUInt32();
                    temp.GuildSkill = stream.ReadUInt32();
                    temp.Unk2 = stream.ReadUInt32();
                    temp.Unk3 = stream.ReadInt32();
                    temp.Mp = stream.ReadUInt32();
                    temp.Facion = stream.ReadUInt32();
                    temp.Unk4 = stream.ReadInt32();
                    temp.Cooldown = stream.ReadUInt32();
                    temp.Unk5 = stream.ReadInt32();
                    temp.Unk6 = stream.ReadInt32();
                    temp.Unk7 = stream.ReadInt32() == 1;
                    temp.Unk8 = stream.ReadInt32();
                    temp.Unk9 = stream.ReadInt32();
                    temp.Unk10 = stream.ReadInt32();
                    temp.Unk11 = stream.ReadInt32();
                    temp.Unk12 = stream.ReadInt32() == 1;
                    temp.Unk13 = stream.ReadInt32();
                    temp.Unk14 = stream.ReadInt32();
                    temp.Unk15 = stream.ReadInt32();
                    temp.Unk16 = stream.ReadInt32();

                    // cast
                    temp.InstantCastEffect = new[]
                    {
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                    };

                    //spellDuration
                    temp.SpellDurationEffect = new[]
                    {
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                    };

                    temp.EffectDuration = stream.ReadUInt32();

                    // passive
                    temp.PassiveEffect = new[]
                    {
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                        stream.ReadInt32(),
                    };

                    temp.CastTime = stream.ReadUInt32();
                    temp.Unk37 = stream.ReadInt32();
                    temp.Unk38 = stream.ReadInt32();
                    temp.Unk39 = stream.ReadInt32();
                    temp.Unk40 = stream.ReadInt32();
                    temp.Unk41 = stream.ReadInt32();
                    temp.Unk42 = stream.ReadInt32();
                    temp.Unk43 = stream.ReadInt32();
                    temp.Unk44 = stream.ReadInt32();
                    temp.Unk45 = stream.ReadInt32();
                    temp.Unk46 = stream.ReadInt32();
                    temp.Unk47 = stream.ReadInt32();
                    temp.Unk48 = stream.ReadInt32();
                    temp.Unk49 = stream.ReadInt32();
                    temp.Unk50 = stream.ReadInt32();
                    temp.Unk51 = stream.ReadInt32();
                    temp.Unk52 = stream.ReadInt32();
                    temp.Unk53 = stream.ReadInt32();
                    temp.Unk54 = stream.ReadInt32();
                    temp.Unk55 = stream.ReadInt32();
                    temp.Unk56 = stream.ReadInt32();
                    temp.Unk57 = stream.ReadInt32();
                    temp.Unk58 = stream.ReadInt32();
                    temp.Unk59 = stream.ReadInt32();
                    temp.Unk60 = stream.ReadInt32();
                    temp.Unk61 = stream.ReadInt32();
                    temp.Unk62 = stream.ReadInt32();
                    temp.Description = Encode.GetString(stream.ReadBytes(256)).Trim('\u0000');
                    temp.Unk70 = stream.ReadInt32();
                    temp.Unk63 = stream.ReadInt32();
                    temp.Unk64 = stream.ReadInt32();
                    temp.Unk65 = stream.ReadInt32();
                    temp.Unk66 = stream.ReadInt32();
                    temp.Unk67 = stream.ReadInt32();
                    temp.Unk68 = stream.ReadInt32();
                    temp.Unk69 = stream.ReadInt32();
                    i++;

                    if (!string.IsNullOrEmpty(temp.Name2))
                        list.Add(temp);
                }

                JsonData = JsonConvert.SerializeObject(list, Formatting.Indented);
            }
        }
    }
}