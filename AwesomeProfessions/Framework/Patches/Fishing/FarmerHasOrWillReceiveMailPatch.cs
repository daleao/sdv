using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class FarmerHasOrWillReceiveMailPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerHasOrWillReceiveMailPatch()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.hasOrWillReceiveMail));
    }

    #region harmony patches

    /// <summary>Patch to allow receiving multiple letters from the FRS and the SWA.</summary>
    [HarmonyPrefix]
    private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
    {
        try
        {
            if (id != $"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice")
                return true; // run original logic

            __result = false;
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}