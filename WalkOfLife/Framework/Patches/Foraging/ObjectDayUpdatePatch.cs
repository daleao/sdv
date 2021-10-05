using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Reflection;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	internal class ObjectDayUpdatePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectDayUpdatePatch()
		{
			Original = typeof(SObject).MethodNamed(nameof(SObject.DayUpdate));
			Postfix = new HarmonyMethod(GetType(), nameof(ObjectDayUpdatePostfix));
		}

		#region harmony patches

		/// <summary>Patch to add quality to Ecologist Mushroom Boxes.</summary>
		[HarmonyPostfix]
		private static void ObjectDayUpdatePostfix(SObject __instance)
		{
			try
			{
				if (!__instance.bigCraftable.Value || __instance.ParentSheetIndex != 128 ||
				    __instance.heldObject.Value == null || !Game1.MasterPlayer.HasProfession("Ecologist"))
					return;

				__instance.heldObject.Value.Quality = Util.Professions.GetEcologistForageQuality();
			}
			catch (Exception ex)
			{
				ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod().Name}:\n{ex}", LogLevel.Error);
			}
		}

		#endregion harmony patches
	}
}