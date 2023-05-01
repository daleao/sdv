namespace DaLion.Overhaul.Modules.Weapons.Patchers.Dwarven;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanUnderstandDwarvesSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanUnderstandDwarvesSetterPatcher"/> class.</summary>
    internal FarmerCanUnderstandDwarvesSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.canUnderstandDwarves));
    }

    #region harmony patches

    /// <summary>Try to patch in Clint's event.</summary>
    [HarmonyPostfix]
    private static void FarmerCanUnderstandDwarvesSetterPostfix()
    {
        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/Events/Blacksmith");
    }

    #endregion harmony patches
}
