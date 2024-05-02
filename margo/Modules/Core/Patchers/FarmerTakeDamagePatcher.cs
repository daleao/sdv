namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Burn effect + reset seconds-out-of-combat.</summary>
    [HarmonyPostfix]
    private static void FarmerTakeDamagePostfix(Farmer __instance)
    {
        if (__instance.IsLocalPlayer)
        {
            State.SecondsOutOfCombat = 0;
        }
    }

    #endregion harmony patches
}
