namespace DaLion.Stardew.Taxes.Framework;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal static class Patches
{
    #region harmony patches

    [HarmonyPatch(typeof(Farmer), nameof(Farmer.hasOrWillReceiveMail))]
    internal sealed class FarmerHasOrWillReceiveMailPatch
    {
        /// <summary>Patch to allow receiving multiple letters from the FRS.</summary>
        [HarmonyPrefix]
        private static bool Prefix(ref bool __result, string id)
        {
            try
            {
                if (!id.Contains(ModEntry.Manifest.UniqueID))
                    return true; // run original logic

                __result = id.Contains("TaxIntro");
                return false; // don't run original logic
            }
            catch (Exception ex)
            {
                Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
                return true; // default to original logic
            }
        }
    }

    #endregion harmony patches
}