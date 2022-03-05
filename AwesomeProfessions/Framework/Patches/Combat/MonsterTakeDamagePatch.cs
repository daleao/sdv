namespace DaLion.Stardew.Professions.Framework.Patches.Combat;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Monsters;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class MonsterTakeDamagePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterTakeDamagePatch()
    {
        Original = RequireMethod<Monster>(nameof(Monster.takeDamage),
            new[] {typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string)});
    }

    #region harmony patches

    /// <summary>Patch to reset monster aggro.</summary>
    [HarmonyPostfix]
    private static void MonsterTakeDamagePostfix(Monster __instance)
    {
        if (__instance is not GreenSlime slime || !slime.ReadDataAs<bool>("Piped") || slime.Health > 0) return;

        foreach (var monster in slime.currentLocation.characters.OfType<Monster>()
                     .Where(m => !m.IsSlime() && m.ReadDataAs<bool>("Aggroed") && m.ReadDataAs<int>("Aggroer") == slime.GetHashCode()))
        {
            monster.WriteData("Aggroed", false.ToString());
            monster.WriteData("Aggroer", null);
        };
    }

    #endregion harmony patches
}