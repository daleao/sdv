namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using DaLion.Common.ModData;
using Extensions;
using HarmonyLib;
using StardewValley.Monsters;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterTakeDamagePatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Monster __instance)
    {
        if (__instance is not GreenSlime slime || !ModDataIO.Read<bool>(slime, "Piped") ||
            slime.Health > 0) return;

        foreach (var monster in slime.currentLocation.characters.OfType<Monster>()
                     .Where(m => !m.IsSlime() && ModDataIO.Read<bool>(m, "Aggroed") &&
                                 ModDataIO.Read<int>(m, "Aggroer") == slime.GetHashCode()))
        {
            ModDataIO.Write(monster, "Aggroed", false.ToString());
            ModDataIO.Write(monster, "Aggroer", null);
        }
    }

    #endregion harmony patches
}