using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches
{
	[UsedImplicitly]
	internal class ObjectLoadDisplayNamePatch : BasePatch
	{
		/// <summary>Construct an instance.</summary>
		internal ObjectLoadDisplayNamePatch()
		{
			Original = RequireMethod<SObject>("loadDisplayName");
			Postfix = new(GetType().MethodNamed(nameof(ObjectLoadDisplayNamePostfix)));
		}

		/// <summary>Patch to add honey-specific mead names.</summary>
		[HarmonyPostfix]
		private static void ObjectLoadDisplayNamePostfix(SObject __instance, ref string __result)
		{
			if (!__instance.name.Contains("Mead") || __instance.preservedParentSheetIndex.Value <= 0) return;

			var prefix = Game1.objectInformation[__instance.preservedParentSheetIndex.Value].Split('/')[4];
			__result = prefix + " " + __result;
		}
	}
}