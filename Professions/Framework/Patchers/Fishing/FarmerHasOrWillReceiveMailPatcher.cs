namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModConflict("DaLion.Taxes")]
internal sealed class FarmerHasOrWillReceiveMailPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerHasOrWillReceiveMailPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerHasOrWillReceiveMailPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.hasOrWillReceiveMail));
    }

    #region harmony patches

    /// <summary>Patch to allow receiving multiple letters from the FRS.</summary>
    [HarmonyPrefix]
    private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
    {
        if (id != $"{UniqueId}/ConservationistTaxNotice")
        {
            return true; // run original logic
        }

        __result = false;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
