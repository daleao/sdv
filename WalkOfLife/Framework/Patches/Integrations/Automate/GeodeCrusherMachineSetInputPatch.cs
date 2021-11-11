using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GeodeCrusherMachineSetInputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GeodeCrusherMachineSetInputPatch()
		{
			try
			{
				Original = "GeodeCrusherMachine".ToType().MethodNamed("SetInput");
			}
			catch
			{
				// ignored
			}

			Postfix = new(GetType().MethodNamed(nameof(GeodeCrusherMachineSetInputPostfix)));
		}

		#region harmony patches

		/// <summary>Patch to apply Gemologist effects to automated Geode Crusher.</summary>
		[HarmonyPostfix]
		private static void GeodeCrusherMachineSetInputPostfix(object __instance)
		{
			if (__instance is null) return;

			var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
			if (machine?.heldObject.Value is null) return;

			var who = Game1.getFarmer(machine.owner.Value);
			if (!who.HasProfession("Gemologist") || !machine.heldObject.Value.IsForagedMineral() &&
				!machine.heldObject.Value.IsGemOrMineral()) return;

			machine.heldObject.Value.Quality = Utility.Professions.GetGemologistMineralQuality();
			if (who.IsLocalPlayer) ModEntry.Data.Increment<uint>("MineralsCollected");
		}

		#endregion harmony patches
	}
}