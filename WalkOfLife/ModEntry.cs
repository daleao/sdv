using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using TheLion.Stardew.Professions.Framework;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.AssetLoaders;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Patches;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions
{
	/// <summary>The mod entry point.</summary>
	public partial class ModEntry : Mod
	{
		internal static ModData Data { get; set; }
		internal static ModConfig Config { get; set; }
		internal static EventSubscriber Subscriber { get; private set; }
		internal static ProspectorHunt ProspectorHunt { get; set; }
		internal static ScavengerHunt ScavengerHunt { get; set; }
		internal static SoundEffectLoader SoundFX { get; set; }

		internal static IModHelper ModHelper { get; private set; }
		internal static IManifest Manifest { get; private set; }
		internal static Action<string, LogLevel> Log { get; private set; }
		internal static string UniqueID { get; private set; }

		public static int DemolitionistExcitedness { get; set; }
		public static int SpelunkerLadderStreak { get; set; }
		public static int SlimeContactTimer { get; set; }
		public static HashSet<int> MonstersStolenFrom { get; set; } = new();
		public static Dictionary<StardewValley.Monsters.GreenSlime, float> PipedSlimeScales { get; set; } = new();
		public static Dictionary<int, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
		public static int SuperModeCounterMax => 500;
		public static bool ShouldShakeSuperModeBar { get; set; }
		public static float SuperModeBarAlpha { get; set; }
		public static Color SuperModeGlowColor { get; set; }
		public static Color SuperModeOverlayColor { get; set; }
		public static float SuperModeOverlayAlpha { get; set; }
		public static string SuperModeSFX { get; set; }
		public static bool DidBulletPierceEnemy { get; set; }

		public static int SuperModeIndex
		{
			get => _superModeIndex;
			set
			{
				if (_superModeIndex == value) return;
				_superModeIndex = value;
				SuperModeIndexChanged?.Invoke(value);
			}
		}

		public static int SuperModeCounter
		{
			get => _superModeCounter;
			set
			{
				if (value == 0)
				{
					_superModeCounter = 0;
					SuperModeCounterReturnedToZero?.Invoke();
				}
				else
				{
					if (_superModeCounter == value) return;

					if (_superModeCounter == 0) SuperModeCounterRaisedAboveZero?.Invoke();
					if (value >= SuperModeCounterMax) SuperModeCounterFilled?.Invoke();
					_superModeCounter = Math.Min(value, SuperModeCounterMax);
				}
			}
		}

		public static bool IsSuperModeActive
		{
			get => _isSuperModeActive;
			set
			{
				if (_isSuperModeActive == value) return;

				if (!value) SuperModeDisabled?.Invoke();
				else SuperModeEnabled?.Invoke();
				_isSuperModeActive = value;
			}
		}

		public static event SuperModeCounterFilledEventHandler SuperModeCounterFilled;
		public static event SuperModeCounterRaisedAboveZeroEventHandler SuperModeCounterRaisedAboveZero;
		public static event SuperModeCounterReturnedToZeroEventHandler SuperModeCounterReturnedToZero;
		public static event SuperModeDisabledEventHandler SuperModeDisabled;
		public static event SuperModeEnabledEventHandler SuperModeEnabled;
		public static event SuperModeIndexChangedEventHandler SuperModeIndexChanged;
		
		private static int _superModeIndex = -1;
		private static bool _isSuperModeActive;
		private static int _superModeCounter;
		
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			// store references to mod helper and metadata
			ModHelper = helper;
			Manifest = ModManifest;
			Log = Monitor.Log;
			UniqueID = ModManifest.UniqueID;

			// get configs and mod data
			Config = helper.ReadConfig<ModConfig>();
			Data = new(Log, UniqueID);

			// apply harmony patches
			BasePatch.Init(Log, Config.EnableILCodeExport, helper.DirectoryPath);
			new HarmonyPatcher(Log, UniqueID).ApplyAll();

			// start event subscriber
			Subscriber = new(Log);

			// get mod assets
			helper.Content.AssetEditors.Add(new IconEditor()); // sprite assets
			SoundFX = new(helper.DirectoryPath); // sound assets

			// add debug commands
			Helper.ConsoleCommands.Add("player_checkprofessions", "List the player's current professions.",
				PrintLocalPlayerProfessions);
			Helper.ConsoleCommands.Add("player_addprofessions",
				"Add the specified professions to the local player." + GetUsageForAddProfessions(),
				AddProfessionsToLocalPlayer);
			Helper.ConsoleCommands.Add("player_resetprofessions",
				"Reset all skills and professions for the local player.", ResetLocalPlayerProfessions);
			Helper.ConsoleCommands.Add("player_setultmeter", "Set the super mode meter to the desired value.",
				SetSuperModeCounter);
			Helper.ConsoleCommands.Add("player_readyult", "Max-out the super mode meter.", ReadySuperMode);
			Helper.ConsoleCommands.Add("player_register",
				"Change the currently registered Super Mode profession.",
				RegisterSuperMode);
			Helper.ConsoleCommands.Add("player_maxanimalfriendship", "Max-out the friendship of all owned animals.",
				MaxAnimalFriendship);
			Helper.ConsoleCommands.Add("player_maxanimalmood", "Max-out the mood of all owned animals.", MaxAnimalMood);
			Helper.ConsoleCommands.Add("player_checkfishingprogress", "Check your fishing progress as Angler.",
				PrintFishCaughtAudit);
			Helper.ConsoleCommands.Add("wol_checkdata", "Check current value of all mod data fields.", PrintModData);
			Helper.ConsoleCommands.Add("wol_setitemsforaged", "Set a new value for ItemsForaged field.",
				SetItemsForaged);
			Helper.ConsoleCommands.Add("wol_setmineralscollected", "Set a new value for MineralsCollected field.",
				SetMineralsCollected);
			Helper.ConsoleCommands.Add("wol_setprospectorstreak", "Set a new value for ProspectorStreak field.",
				SetProspectorStreak);
			Helper.ConsoleCommands.Add("wol_setscavengerstreak", "Set a new value for ScavengerStreak field.",
				SetScavengerStreak);
			Helper.ConsoleCommands.Add("wol_settrashcollected",
				"Set a new value for WaterTrashCollectedThisSeason field.",
				SetWaterTrashCollectedThisSeason);
			Helper.ConsoleCommands.Add("wol_checkevents", "List currently subscribed mod events.",
				PrintSubscribedEvents);
		}
	}
}