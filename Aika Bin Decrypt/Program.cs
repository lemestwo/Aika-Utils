using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Aika_Bin_Decrypt
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Need the input file.");
                Console.Read();
                return;
            }

            var inFile = args[0];
            var outFile = inFile + ".out";

            Console.WriteLine("1 - ItemList, PI or SetItem.");
            Console.WriteLine("2 - Others.");
            Console.WriteLine("3 - SL.bin");
            Console.WriteLine("Type 1-3 then press ENTER...");
            var isInt = int.TryParse(Console.ReadLine(), out var type);

            if (!isInt)
            {
                Console.WriteLine("Can only accept numbers as input.");
                Console.Read();
                return;
            }

            if (!File.Exists(inFile))
            {
                Console.WriteLine("File not found.");
                Console.Read();
                return;
            }

            try
            {
                var cItemList = false;
                var binKey = type == 2 ? BinKey2 : BinKey1;
                using (var stream = new BinaryReader(File.OpenRead(inFile)))
                {
                    var size = (int) stream.BaseStream.Length;
                    var data = stream.ReadBytes(size);
                    if (type == 3)
                    {
                        // SL.bin
                        for (var i = 0; i < size; i++)
                        {
                            data[i] += (byte) (5 * (i / 5) - i);
                        }
                    }
                    else
                    {
                        // Remove client key from file and re-size
                        var fileName = Path.GetFileName(inFile);
                        cItemList = fileName.Contains("ItemList");
                        var cSkillData = fileName.Contains("SkillData");
                        var cActionText = fileName.Contains("ActionText");
                        if (cItemList || cSkillData || cActionText)
                        {
                            if (data[0] == 0x42 && data[1] == 0x52 || cActionText)
                            {
                                var sizeToDelete = cActionText ? 48 : 12;
                                var newData = new byte[size - sizeToDelete];
                                Buffer.BlockCopy(data, sizeToDelete, newData, 0, size - sizeToDelete);

                                // Set new data
                                size = newData.Length;
                                data = newData;
                            }
                        }

                        var keySize = binKey.Length;
                        for (var i = 0; i < size; i++)
                        {
                            data[i] -= (byte) (i + binKey[i % keySize]);
                        }
                    }

                    File.WriteAllBytes(outFile, data);
                }

                if (cItemList)
                {
                    using (var stream = new BinaryReader(File.OpenRead(outFile)))
                    {
                        var size = stream.BaseStream.Length;
                        var i = 0u;
                        var list = new List<ItemModel>();
                        while (stream.BaseStream.Position < size - 4)
                        {
                            var temp = new ItemModel();
                            temp.LoopId = i;
                            temp.ItemName = Encoding.UTF8.GetString(stream.ReadBytes(64)).Trim('\u0000');
                            temp.ItemName2 = Encoding.UTF8.GetString(stream.ReadBytes(64)).Trim('\u0000');
                            temp.Description = Encoding.UTF8.GetString(stream.ReadBytes(128)).Trim('\u0000');
                            stream.ReadBytes(2);
                            temp.ItemSlot = stream.ReadUInt16();
                            var caelium = stream.ReadInt32();
                            stream.ReadBytes(20);
                            temp.HonorCost = stream.ReadUInt32();
                            temp.MedalCost = stream.ReadUInt32();
                            temp.BuyPrice = stream.ReadUInt32();
                            temp.SellPrice = stream.ReadUInt32();
                            temp.Profession = stream.ReadUInt16();
                            stream.ReadUInt16();
                            var unk = stream.ReadUInt16();
                            stream.ReadInt32();
                            stream.ReadUInt16();
                            var unk2 = stream.ReadUInt16();
                            stream.ReadInt32();
                            stream.ReadUInt16();
                            temp.ImageId = stream.ReadUInt16();
                            var unk3 = stream.ReadUInt16();
                            stream.ReadBytes(6);
                            temp.MinLevel = stream.ReadUInt16();
                            var unk4 = stream.ReadInt32();
                            temp.TimeLimit = stream.ReadInt32();
                            stream.ReadBytes(18);
                            temp.PAtk = stream.ReadUInt16();
                            temp.PDef = stream.ReadUInt16();
                            temp.MAtk = stream.ReadUInt16();
                            temp.MDef = stream.ReadUInt16();
                            stream.ReadBytes(98);
                            i++;
                            list.Add(temp);
                        }

                        var json = JsonConvert.SerializeObject(list);
                        File.WriteAllText(inFile + ".json", json);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.Read();
                throw;
            }
            finally
            {
                Console.WriteLine("Input: " + Path.GetFileName(inFile));
                Console.WriteLine("Output: " + Path.GetFileName(outFile));
                Console.WriteLine("Decrypted with success.");
            }

            Console.Read();
        }

        public class ItemModel
        {
            public uint LoopId { get; set; }
            public string ItemName { get; set; }
            public string ItemName2 { get; set; }
            public string Description { get; set; }
            public ushort ItemSlot { get; set; }
            public uint HonorCost { get; set; }
            public uint MedalCost { get; set; }
            public uint BuyPrice { get; set; }
            public uint SellPrice { get; set; }
            public ushort Profession { get; set; }
            public ushort ImageId { get; set; }
            public ushort MinLevel { get; set; }
            public int TimeLimit { get; set; }
            public ushort PAtk { get; set; }
            public ushort PDef { get; set; }
            public ushort MAtk { get; set; }
            public ushort MDef { get; set; }
        }

        private static readonly byte[] BinKey1 =
        {
            0xBE, 0xC6, 0xC0, 0xCC, 0xC5, 0xDB, 0x20, 0xB8, 0xAE, 0xBD, 0xBA, 0xC6,
            0xAE, 0x20, 0xC0, 0xCE, 0xC4, 0xDA, 0xB5, 0xF9, 0x20, 0xB7, 0xE7, 0xC6,
            0xBE, 0xC0, 0xD4, 0xB4, 0xCF, 0xB4, 0xD9, 0x2E, 0x20, 0xB7, 0xEA, 0xB7,
            0xE7, 0x20, 0xB6, 0xF6, 0xB6, 0xF3, 0x2E, 0x2E, 0x2E, 0x20, 0xC0, 0xB8,
            0x2E, 0x2E, 0x2E, 0x20, 0xC1, 0xA4, 0xB8, 0xBB, 0x20, 0xC1, 0xA4, 0xB8,
            0xBB, 0x20, 0xB1, 0xCD, 0xC2, 0xFA, 0xB4, 0xD9, 0x2E, 0x20, 0xB1, 0xD7,
            0xB7, 0xA1, 0xB5, 0xB5, 0x20, 0xC7, 0xD8, 0xBE, 0xDF, 0xC7, 0xCF, 0xB4,
            0xCF, 0x20, 0xBE, 0xEE, 0xC2, 0xBF, 0x20, 0xBC, 0xF6, 0x20, 0xBE, 0xF8,
            0xC1, 0xD2, 0x2E, 0x2E, 0x2E, 0x20
        };

        private static readonly byte[] BinKey2 =
        {
            0xC0, 0xCC, 0xB0, 0xC5, 0x20, 0xC0, 0xD0, 0xC1, 0xF6, 0x20, 0xB8, 0xB6,
            0xBC, 0xBC, 0xBF, 0xE4, 0x2E, 0x20, 0xC0, 0xD0, 0xC0, 0xB8, 0xB8, 0xE9,
            0x20, 0xB3, 0xAA, 0xBB, 0xDB, 0xBB, 0xE7, 0xB6, 0xF7, 0xB5, 0xCB, 0xB4,
            0xCF, 0xB4, 0xD9, 0x2E, 0x20, 0xC1, 0xA6, 0xB9, 0xDF, 0x20, 0xC0, 0xFD,
            0xB4, 0xEB, 0x20, 0xC0, 0xD0, 0xC1, 0xF6, 0xB8, 0xBB, 0xB0, 0xED, 0x20,
            0xC2, 0xF8, 0xC7, 0xD1, 0x20, 0xBB, 0xE7, 0xB6, 0xF7, 0xB5, 0xEE, 0xB7,
            0xCE, 0x20, 0xBB, 0xE7, 0xBC, 0xBC, 0xBF, 0xE4, 0x2E, 0x20, 0xBE, 0xC6,
            0xBC, 0xCC, 0xC1, 0xD2, 0x3F, 0x20, 0xC1, 0xC1, 0xC0, 0xBA, 0xBC, 0xBC,
            0xBB, 0xF3, 0xB8, 0xB8, 0xB5, 0xEC, 0xBD, 0xC3, 0xB4, 0xD9, 0x2E
        };
    }
}