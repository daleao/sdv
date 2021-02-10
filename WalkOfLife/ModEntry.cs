using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheLion.AwesomeProfessions
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModConfig Config { get; set; }
		public Dictionary<string, int> ProfessionIds { get; set; } = new();

		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			InitializeProfessionIds();

			// get configs.json
			Config = Helper.ReadConfig<ModConfig>();

			// create and patch Harmony instance
			var harmony = HarmonyInstance.Create("thelion.AwesomeProfessions");
			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}

		public void InitializeProfessionIds()
		{
			// farming
			ProfessionIds["harvester"] = Farmer.tiller;
			ProfessionIds["agriculturist"] = Farmer.agriculturist;
			ProfessionIds["oenologist"] = Farmer.artisan;
			
			ProfessionIds["rancher"] = Farmer.rancher;
			ProfessionIds["breeder"] = Farmer.shepherd;
			ProfessionIds["apiologist"] = Farmer.butcher;

			// foraging
			ProfessionIds["forager"] = Farmer.gatherer;
			ProfessionIds["botanist"] = Farmer.botanist;
			ProfessionIds["mycologist"] = Farmer.tracker;

			ProfessionIds["lumberjack"] = Farmer.forester;
			ProfessionIds["forester"] = Farmer.lumberjack;
			ProfessionIds["tapper"] = Farmer.tapper;

			// mining
			ProfessionIds["miner"] = Farmer.miner;
			ProfessionIds["spelunker"] = Farmer.burrower;
			ProfessionIds["metallurgist"] = Farmer.blacksmith;

			ProfessionIds["blaster"] = Farmer.geologist;
			ProfessionIds["demolitionist"] = Farmer.excavator;
			ProfessionIds["gemologist"] = Farmer.gemologist;

			// fishing
			ProfessionIds["fisher"] = Farmer.fisher;
			ProfessionIds["angler"] = Farmer.angler;
			ProfessionIds["aquarist"] = Farmer.pirate;

			ProfessionIds["trapper"] = Farmer.trapper;
			ProfessionIds["mariner"] = Farmer.mariner;
			ProfessionIds["conservationist"] = Farmer.baitmaster;

			// combat
			ProfessionIds["fighter"] = Farmer.fighter;
			ProfessionIds["brute"] = Farmer.brute;
			ProfessionIds["gambit"] = Farmer.defender;

			ProfessionIds["scout"] = Farmer.scout;
			ProfessionIds["marksman"] = Farmer.acrobat;
			ProfessionIds["venturer"] = Farmer.desperado;
		}
	}
}
