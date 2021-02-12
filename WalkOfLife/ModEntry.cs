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
		public static Dictionary<string, int> ProfessionIds { get; set; } = new();

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
			ProfessionIds["harvester"] = Farmer.tiller;				// 1
			ProfessionIds["agriculturist"] = Farmer.agriculturist;	// 5
			ProfessionIds["oenologist"] = Farmer.artisan;			// 4
			
			ProfessionIds["rancher"] = Farmer.rancher;				// 0
			ProfessionIds["breeder"] = Farmer.shepherd;				// 3
			ProfessionIds["apiologist"] = Farmer.butcher;			// 2 (coopmaster)

			// foraging
			ProfessionIds["forager"] = Farmer.gatherer;				// 13
			ProfessionIds["botanist"] = Farmer.botanist;			// 16
			ProfessionIds["mycologist"] = Farmer.tracker;			// 17

			ProfessionIds["lumberjack"] = Farmer.forester;			// 12
			ProfessionIds["forester"] = Farmer.lumberjack;			// 14
			ProfessionIds["tapper"] = Farmer.tapper;				// 15

			// mining
			ProfessionIds["miner"] = Farmer.miner;					// 18
			ProfessionIds["spelunker"] = Farmer.burrower;			// 21 (prospector)
			ProfessionIds["metallurgist"] = Farmer.blacksmith;		// 20

			ProfessionIds["blaster"] = Farmer.geologist;			// 19
			ProfessionIds["demolitionist"] = Farmer.excavator;		// 22
			ProfessionIds["gemologist"] = Farmer.gemologist;		// 23

			// fishing
			ProfessionIds["fisher"] = Farmer.fisher;				// 6
			ProfessionIds["angler"] = Farmer.angler;				// 8
			ProfessionIds["aquarist"] = Farmer.pirate;				// 9

			ProfessionIds["trapper"] = Farmer.trapper;				// 7
			ProfessionIds["mariner"] = Farmer.mariner;				// 11
			ProfessionIds["conservationist"] = Farmer.baitmaster;	// 10

			// combat
			ProfessionIds["fighter"] = Farmer.fighter;				// 24
			ProfessionIds["brute"] = Farmer.brute;					// 26
			ProfessionIds["gambit"] = Farmer.defender;				// 27

			ProfessionIds["scout"] = Farmer.scout;					// 25
			ProfessionIds["marksman"] = Farmer.acrobat;				// 28
			ProfessionIds["venturer"] = Farmer.desperado;			// 29
		}
	}
}
