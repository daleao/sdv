using Harmony;
using StardewModdingAPI;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions.Framework.Patches
{
	internal abstract class BasePatch
	{
		private protected static ProfessionsConfig _config;
		private protected static IMonitor _monitor;

		/// <summary>Construct an instance.</summary>
		/// <param name="config">The mod settings.</param>
		/// <param name="monitor">Interface for writing to the SMAPI console.</param>
		internal BasePatch(ProfessionsConfig config, IMonitor monitor)
		{
			_config = config;
			_monitor = monitor;
		}

		/// <summary>Apply internally-defined Harmony patches.</summary>
		/// <param name="harmony">The Harmony instance for this mod.</param>
		protected internal abstract void Apply(HarmonyInstance harmony);

		/// <summary>Whether a given object is a fish trapped by a crab pot.</summary>
		/// <param name="obj">The given object.</param>
		protected static bool IsShellfish(SObject obj)
		{
			return obj.ParentSheetIndex > 714 && obj.ParentSheetIndex < 724;
		}

		/// <summary>Whether a given object is a fish caught with a fishing rod.</summary>
		/// <param name="obj">The given object.</param>
		protected static bool IsReeledFish(SObject obj)
		{
			return obj.Category == SObject.FishCategory && !IsShellfish(obj);
		}
	}
}
