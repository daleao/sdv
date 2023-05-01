namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerHasOrWillReceiveMailPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerHasOrWillReceiveMailPatcher"/> class.</summary>
    internal FarmerHasOrWillReceiveMailPatcher()
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
            if (id != $"{Manifest.UniqueID}/ConservationistTaxNotice")
            {
                return true; // run original logic
            }

            __result = false;
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
