namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using System.Linq;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    internal MonsterTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Monster __instance)
    {
        if (__instance is not GreenSlime slime || slime.Get_Piper() is null ||
            slime.Health > 0)
        {
            return;
        }

        foreach (var monster in slime.currentLocation.characters.OfType<Monster>()
                     .Where(m => !m.IsSlime() && m.Get_Taunter().Get(m.currentLocation) == slime))
        {
            monster.Set_Taunter(null);
        }
    }

    #endregion harmony patches
}
