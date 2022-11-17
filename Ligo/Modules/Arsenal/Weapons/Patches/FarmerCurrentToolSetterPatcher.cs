namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolSetterPatcher()
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
