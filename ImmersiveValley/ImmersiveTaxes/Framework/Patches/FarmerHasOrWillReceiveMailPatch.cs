namespace DaLion.Stardew.Taxes.Framework.Patches;

#region using directives

using System;
using System.Reflection;
using DaLion.Common;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerHasOrWillReceiveMailPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerHasOrWillReceiveMailPatch"/> class.</summary>
    internal FarmerHasOrWillReceiveMailPatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.hasOrWillReceiveMail));
    }

    #region harmony patches

    /// <summary>Patch to allow receiving multiple letters from the FRS.</summary>
    [HarmonyPrefix]
    private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
    {
        try
        {
            if (!id.Contains(ModEntry.Manifest.UniqueID))
            {
                return true; // run original logic
            }

            __result = id.Contains("TaxIntro");
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
