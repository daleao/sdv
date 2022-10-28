namespace DaLion.Redux.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using StardewValley;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolSetterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolSetterPatch"/> class.</summary>
    internal FarmerCurrentToolSetterPatch()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentTool));
    }

    #region harmony patches

    /// <summary>Reset combo hit counter.</summary>
    [HarmonyPostfix]
    private static void FarmerCurrentToolSetterPostfix()
    {
        ModEntry.State.Arsenal.ComboHitStep = ComboHitStep.Idle;
    }

    #endregion harmony patches
}
