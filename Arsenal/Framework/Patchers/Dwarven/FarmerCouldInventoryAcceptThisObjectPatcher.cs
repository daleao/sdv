namespace DaLion.Arsenal.Framework.Patchers.Dwarven;

using DaLion.Overhaul.Modules.Combat.Integrations;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCouldInventoryAcceptThisObjectPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCouldInventoryAcceptThisObjectPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FarmerCouldInventoryAcceptThisObjectPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.couldInventoryAcceptThisObject));
    }

    #region harmony patches

    /// <summary>Allow picking up blueprints.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerCouldInventoryAcceptThisObjectPrefix(ref bool __result, int index)
    {
        if (!JsonAssetsIntegration.DwarvishBlueprintIndex.HasValue ||
            JsonAssetsIntegration.DwarvishBlueprintIndex.Value != index)
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
