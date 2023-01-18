namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeTakeDamagePatcher"/> class.</summary>
    internal GreenSlimeTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro when a piped slime is defeated.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(GreenSlime __instance)
    {
        if (__instance.Get_Piped() is null || __instance.Health > 0)
        {
            return;
        }

        foreach (var character in __instance.currentLocation.characters)
        {
            if (character is Monster monster && !monster.IsSlime() && monster.Get_Taunter() == __instance)
            {
                monster.Set_Taunter(null);
            }
        }
    }

    #endregion harmony patches
}
