using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class GenericObjectMachineSetInputPatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal GenericObjectMachineSetInputPatch()
		{
			//Original = AccessTools.Method(
			//	"Pathoschild.Stardew.Automate.Framework.Machines.Objects.GenericObjectMachine:SetInput");
			Postfix = new(AccessTools.Method(GetType(), nameof(GenericObjectMachineGetOutputPostfix)));
		}

		/// <inheritdoc />
		public override void Apply(Harmony harmony)
		{
			var targetMethods = TargetMethods().ToList();
			ModEntry.Log($"[Patch]: Found {targetMethods.Count} target methods for {GetType().Name}.", LogLevel.Trace);
			foreach (var method in targetMethods)
				try
				{
					Original = method;
					base.Apply(harmony);
				}
				catch
				{
					// ignored
				}
		}

		#region harmony patches

		/// <summary>Patch to apply Artisan effects to automated machines.</summary>
		[HarmonyPostfix]
		private static void GenericObjectMachineGetOutputPostfix(object __instance)
		{
			if (__instance is null) return;

			var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
			if (machine?.heldObject.Value is null || !machine.heldObject.Value.IsArtisanGood()) return;

			var who = Game1.getFarmer(machine.owner.Value);
			if (!who.HasProfession("Artisan")) return;

			if (machine.heldObject.Value.Quality < SObject.bestQuality &&
			    new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
				machine.heldObject.Value.Quality += machine.heldObject.Value.Quality == SObject.medQuality ? 2 : 1;

			machine.MinutesUntilReady -= machine.MinutesUntilReady / 10;
		}

		#endregion harmony patches

		#region private methods

		[HarmonyTargetMethods]
		private static IEnumerable<MethodBase> TargetMethods()
		{
			return from type in AccessTools.AllTypes()
				where type.Name.IsAnyOf(
					"CheesePressMachine",
					"KegMachine",
					"LoomMachine",
					"MayonnaiseMachine",
					"OilMakerMachine",
					"PreservesJarMachine")
				select type.MethodNamed("SetInput");
		}

		#endregion private methods
	}
}