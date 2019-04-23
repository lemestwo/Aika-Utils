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
						convert = new ItemList(inFile, outFile);
						break;
					case "npcpos":
						convert = new NpcPos(inFile, outFile);
						break;
					case "MobPos":
						convert = new MobPos(inFile, outFile);
						break;
					case "ExpList":
						convert = new ExpList(inFile, outFile);
						break;
					case "PranExpList":
						convert = new PranExpList(inFile, outFile);
						break;
					case "MN":
						convert = new Mn(inFile, outFile);
						break;
					case "GearCore":
						convert = new GearCore(inFile, outFile);
						break;
					case "Title":
						convert = new Title(inFile, outFile);
						break;
					case "Recipe":
						convert = new Recipe(inFile, outFile);
						break;
					case "RecipeRate":
						convert = new RecipeRate(inFile, outFile);
						break;
					case "ItemEffect":
						convert = new ItemEffect(inFile, outFile);
						break;
					case "ReinforceA":
					case "ReinforceW":
						convert = new ReinforceA(inFile, outFile);
						break;
					case "Reinforce2":
					case "Reinforce3":
						convert = new Reinforce2(inFile, outFile);
						break;
					case "MakeItems":
						convert = new MakeItems(inFile, outFile);
						break;
					case "SetItem":
						convert = new SetItem(inFile, outFile);
						break;
					case "SkillData":
						convert = new SkillData(inFile, outFile);
						break;
					case "Quest":
						convert = new Quest(inFile, outFile);
						break;
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