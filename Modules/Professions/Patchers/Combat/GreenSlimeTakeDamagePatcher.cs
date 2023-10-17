namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

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
        this.Target = this.RequireMethod<GreenSlime>(
            nameof(GreenSlime.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(Farmer) });
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro when a piped slime is defeated.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeTakeDamagePostfix(GreenSlime __instance)
    {
        if (__instance.Get_Piped() is not { } piped || __instance.Health > 0)
        {
            return;
        }

        for (var i = 0; i < __instance.currentLocation.characters.Count; i++)
        {
            if (__instance.currentLocation.characters[i] is Monster monster && !monster.IsSlime() &&
                monster.Get_Taunter() == __instance)
            {
                monster.Set_Taunter(null);
            }
        }
    }

    #endregion harmony patches
}
