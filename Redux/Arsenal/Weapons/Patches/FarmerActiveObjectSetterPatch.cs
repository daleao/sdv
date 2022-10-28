namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerActiveObjectSetterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerActiveObjectSetterPatch"/> class.</summary>
    internal FarmerActiveObjectSetterPatch()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.ActiveObject));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void FarmerActiveObjectSetterPostfix()
    {
        ModEntry.State.Arsenal.ComboHitStep = ComboHitStep.Idle;
    }

    #endregion harmony patches
}
