namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using DaLion.Common.Data;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterTakeDamagePatch()
    {
        Target = RequireMethod<Monster>(nameof(Monster.takeDamage),
            new[] {typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string)});
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Monster __instance)
    {
        if (__instance is not GreenSlime slime || !ModDataIO.ReadDataAs<bool>(slime, "Piped") ||
            slime.Health > 0) return;

        foreach (var monster in slime.currentLocation.characters.OfType<Monster>()
                     .Where(m => !m.IsSlime() && ModDataIO.ReadDataAs<bool>(m, "Aggroed") &&
                                 ModDataIO.ReadDataAs<int>(m, "Aggroer") == slime.GetHashCode()))
        {
            ModDataIO.WriteData(monster, "Aggroed", false.ToString());
            ModDataIO.WriteData(monster, "Aggroer", null);
        }
    }

    #endregion harmony patches
}