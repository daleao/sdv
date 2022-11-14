namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using HarmonyLib;
using Shared.Harmony;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerActiveObjectSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerActiveObjectSetterPatcher"/> class.</summary>
    internal FarmerActiveObjectSetterPatcher()
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
