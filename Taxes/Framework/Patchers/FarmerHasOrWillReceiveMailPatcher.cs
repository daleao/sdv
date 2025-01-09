namespace DaLion.Taxes.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
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
    [UsedImplicitly]
    private static bool FarmerHasOrWillReceiveMailPrefix(ref bool __result, string id)
    {
        if (!id.Contains(UniqueId))
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
