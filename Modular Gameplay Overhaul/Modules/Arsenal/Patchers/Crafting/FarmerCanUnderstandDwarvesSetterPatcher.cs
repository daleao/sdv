namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Crafting;

#region using directives

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

    /// <summary>Double stamina consumption when cursed.</summary>
    [HarmonyPostfix]
    private static void FarmerStaminaSetterPostfix()
    {
        ModHelper.GameContent.InvalidateCache("Data/Events/Blacksmith");
    }

    #endregion harmony patches
}
