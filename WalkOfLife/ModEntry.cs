using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using TheLion.Stardew.Professions.Framework;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Patches;
using TheLion.Stardew.Professions.Framework.TreasureHunt;

namespace TheLion.Stardew.Professions
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static ModData Data { get; private set; }
		public static ModConfig Config { get; private set; }

		public static int DemolitionistExcitedness { get; set; }
		public static int SpelunkerLadderStreak { get; set; }
		public static int SlimeContactTimer { get; set; }
		public static HashSet<int> MonstersStolenFrom { get; set; } = new();
		public static Dictionary<GreenSlime, float> PipedSlimesScales { get; set; } = new();
		public static Dictionary<int, HashSet<long>> ActivePeerSuperModes { get; set; } = new();
		public static int SuperModeCounterMax => 500;
		public static bool ShouldShakeSuperModeBar { get; set; }
		public static float SuperModeBarAlpha { get; set; }
		public static Color SuperModeGlowColor { get; set; }
		public static Color SuperModeOverlayColor { get; set; }
		public static float SuperModeOverlayAlpha { get; set; }
		public static string SuperModeSfx { get; set; }

		public static int SuperModeIndex
		{
			get => _superModeIndex;
			set
			{
				if (_superModeIndex != value)
				{
					_superModeIndex = value;
					SuperModeIndexChanged?.Invoke(value);
				}
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

		internal static GameFramework GameFramework { get; private set; }
		internal static IContentHelper Content { get; private set; }
		internal static IModEvents Events { get; private set; }
		internal static IModRegistry Registry { get; private set; }
		internal static IMultiplayerHelper Multiplayer { get; private set; }
		internal static IReflectionHelper Reflection { get; private set; }
		internal static ITranslationHelper I18n { get; private set; }
		internal static Action<string, LogLevel> Log { get; private set; }
		internal static string UniqueID { get; private set; }

		internal static EventSubscriber Subscriber { get; private set; }
		internal static ProspectorHunt ProspectorHunt { get; set; }
		internal static ScavengerHunt ScavengerHunt { get; set; }
		internal static SoundEffectLoader SfxLoader { get; set; }

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
			// get target framework
			GameFramework = Constants.GameFramework;

			// store references to mod helpers
			Content = helper.Content;
			Events = helper.Events;
			Registry = helper.ModRegistry;
			Multiplayer = helper.Multiplayer;
			Reflection = helper.Reflection;
			I18n = helper.Translation;
			Log = Monitor.Log;

			// get configs.json
			Config = helper.ReadConfig<ModConfig>();

			// get unique id and instantiate mod data
			UniqueID = ModManifest.UniqueID;
			Data = new ModData();

			// get mod assets
			helper.Content.AssetEditors.Add(new IconEditor());

			// get sound assets
			SfxLoader = new SoundEffectLoader(helper.DirectoryPath);

			// apply harmony patches
			BasePatch.Init();
			new HarmonyPatcher().ApplyAll();

			// start event manager
			Subscriber = new EventSubscriber();

			// add debug commands
			Helper.ConsoleCommands.Add("player_addprofessions", "Add the specified professions to the local player." + ConsoleCommands.GetUsageForAddProfessions(), ConsoleCommands.AddProfessionsToLocalPlayer);
			Helper.ConsoleCommands.Add("player_resetprofessions", "Reset all skills and professions for the local player.", ConsoleCommands.ResetLocalPlayerProfessions);
			Helper.ConsoleCommands.Add("player_checkprofessions", "List the player's current professions.", ConsoleCommands.CheckLocalPlayerProfessions);
			Helper.ConsoleCommands.Add("player_setultmeter", "Set the super mode meter to the desired value.", ConsoleCommands.SetSuperModeCounter);
			Helper.ConsoleCommands.Add("player_readyult", "Max-out the super mode meter.", ConsoleCommands.ReadySuperMode);
			Helper.ConsoleCommands.Add("player_maxanimalfriendship", "Max-out the friendship of all owned animals.", ConsoleCommands.MaxAnimalFriendship);
			Helper.ConsoleCommands.Add("player_maxanimalmood", "Max-out the mood of all owned animals.", ConsoleCommands.MaxAnimalMood);
			Helper.ConsoleCommands.Add("wol_getdatafield", "Check current value for a profession data field." + ConsoleCommands.GetAvailableDataFields(), ConsoleCommands.PrintModDataField);
			Helper.ConsoleCommands.Add("wol_setitemsforaged", "Set a new value for ItemsForaged field.", ConsoleCommands.SetItemsForaged);
			Helper.ConsoleCommands.Add("wol_setmineralscollected", "Set a new value for MineralsCollected field.", ConsoleCommands.SetMineralsCollected);
			Helper.ConsoleCommands.Add("wol_setprospectorstreak", "Set a new value for ProspectorStreak field.", ConsoleCommands.SetProspectorStreak);
			Helper.ConsoleCommands.Add("wol_setscavengerstreak", "Set a new value for ScavengerStreak field.", ConsoleCommands.SetScavengerStreak);
			Helper.ConsoleCommands.Add("wol_settrashcollected", "Set a new value for WaterTrashCollectedThisSeason field.", ConsoleCommands.SetWaterTrashCollectedThisSeason);
			Helper.ConsoleCommands.Add("wol_checkevents", "List currently subscribed mod events.", ConsoleCommands.PrintSubscribedEvents);
		}
	}
}