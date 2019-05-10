using System;
using System.IO;
using Aika_BinToJson.Convertion;
using Aika_BinToJson.Models;

namespace Aika_BinToJson
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
            //var inFile = "C:\\Projetos\\Aika Utils\\Aika BinToJson\\bin\\Debug\\SkillData.bin";
            var outFile = inFile + ".json";

            if (!File.Exists(inFile))
            {
                Console.WriteLine("File not found.");
                Console.Read();
                return;
            }

            var isDone = false;
            try
            {
                // OBS:
                // It's better to convert using same class model
                // as the one used in AikaEmu.GameServer.

                var fileName = Path.GetFileNameWithoutExtension(inFile);
                if (fileName == null) return;

                BaseConvert convert = null;
                switch (fileName)
                {
                    case "ItemList":
                        convert = new ItemList();
                        break;
                    case "npcpos":
                        convert = new NpcPos();
                        break;
                    case "MobPos":
                        convert = new MobPos();
                        break;
                    case "ExpList":
                        convert = new ExpList();
                        break;
                    case "PranExpList":
                        convert = new PranExpList();
                        break;
                    case "MN":
                        convert = new Mn();
                        break;
                    case "GearCore":
                        convert = new GearCore();
                        break;
                    case "Title":
                        convert = new Title();
                        break;
                    case "Recipe":
                        convert = new Recipe();
                        break;
                    case "RecipeRate":
                        convert = new RecipeRate();
                        break;
                    case "ItemEffect":
                        convert = new ItemEffect();
                        break;
                    case "ReinforceA":
                    case "ReinforceW":
                        convert = new ReinforceA();
                        break;
                    case "Reinforce2":
                    case "Reinforce3":
                        convert = new Reinforce2();
                        break;
                    case "MakeItems":
                        convert = new MakeItems();
                        break;
                    case "SetItem":
                        convert = new SetItem();
                        break;
                    case "SkillData":
                        convert = new SkillData();
                        break;
                    case "Quest":
                        convert = new Quest();
                        break;
                    case "Map":
                        convert = new Map();
                        break;
                    case "ObjPos":
                        convert = new ObjPos();
                        break;
                    case "SPosition":
                        convert = new SPosition();
                        break;
                    case "Dialog":
                        convert = new Dialog();
                        break;
                    case "StatusPoint":
                        convert = new StatusPoint();
                        break;
                }

                if (convert != null)
                {
                    convert.SetupFile(inFile, outFile);
                    convert.Convert();
                    convert.Save();
                    isDone = true;
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
                if (isDone)
                {
                    Console.WriteLine("Input: " + Path.GetFileName(inFile));
                    Console.WriteLine("Output: " + Path.GetFileName(outFile));
                    Console.WriteLine("Converted with success.");
                }
                else
                {
                    Console.WriteLine("Error in conversion.");
                }
            }

            Console.Read();
        }
    }
}