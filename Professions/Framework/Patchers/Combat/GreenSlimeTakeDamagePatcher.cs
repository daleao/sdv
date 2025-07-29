namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GreenSlimeTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GreenSlimeTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GreenSlimeTakeDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GreenSlime>(
            nameof(GreenSlime.takeDamage),
            [typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer)]);
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro when a piped slime is defeated.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool GreenSlimeTakeDamagePrefix(GreenSlime __instance, bool isBomb)
    {
        return __instance.Get_Piped() is null || !isBomb;
    }

    /// <summary>Patch to reset monster aggro when a piped slime is defeated.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GreenSlimeTakeDamagePostfix(GreenSlime __instance, Farmer who)
    {
        if (__instance.Health > 0 || __instance.Get_Piped() is not { } piped)
        {
            return;
        }

        foreach (var character in __instance.currentLocation.characters)
        {
            if (character is Monster { IsMonster: true } monster && !monster.IsSlime() &&
                monster.Get_Taunter() == __instance)
            {
                monster.Set_Taunter(null);
            }
        }

        piped.Burst();
    }

    #endregion harmony patches
}
