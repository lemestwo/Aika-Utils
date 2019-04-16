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

                var fileName = Path.GetFileName(inFile);
                if (fileName == null) return;

                Base convert = null;
                if (fileName.Contains("ItemList"))
                {
                    convert = new ItemList(inFile, outFile);
                }
                else if (fileName.Contains("npcpos"))
                {
                    convert = new NpcPos(inFile, outFile);
                }
                else if (fileName.Contains("MobPos"))
                {
                    convert = new MobPos(inFile, outFile);
                }
                else if (fileName.Contains("ExpList"))
                {
                    if (fileName.Contains("Pran"))
                    {
                        convert = new PranExpList(inFile, outFile);
                    }
                    else
                    {
                        convert = new ExpList(inFile, outFile);
                    }
                }
                else if (fileName.Contains("MN"))
                {
                    convert = new Mn(inFile, outFile);
                }

                if (convert != null)
                {
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