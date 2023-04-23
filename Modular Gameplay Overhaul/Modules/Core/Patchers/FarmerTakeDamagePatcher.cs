namespace DaLion.Overhaul.Modules.Core.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
        this.Prefix!.priority = Priority.First;
    }

    #region harmony patches

    /// <summary>Burn effect + reset seconds-out-of-combat.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static void FarmerTakeDamagePrefix(Farmer __instance, ref int damage, Monster? damager)
    {
        if (damager is not null && damager.IsBurning())
        {
            damage /= 2;
        }

        if (__instance.IsLocalPlayer)
        {
            Globals.SecondsOutOfCombat = 0;
        }
    }

    #endregion harmony patches
}
