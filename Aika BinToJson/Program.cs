using System;
using System.IO;
using Aika_BinToJson.Convertion;

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

            try
            {
                var fileName = Path.GetFileName(inFile);
                if (fileName == null) return;

                Base convert = null;
                if (fileName.Contains("ItemList"))
                {
                    convert = new ItemList(inFile, outFile);
                }
                else if (fileName.Contains("npcpos"))
                {
                    //
                }

                if (convert != null)
                {
                    convert.Convert();
                    convert.Save();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Console.WriteLine("Input: " + Path.GetFileName(inFile));
                Console.WriteLine("Output: " + Path.GetFileName(outFile));
                Console.WriteLine("Converted with success.");
            }

            Console.Read();
        }
    }
}