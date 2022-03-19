namespace DaLion.Stardew.Arsenal.Framework.Patches.Weapons;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal class MonsterParriedPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MonsterParriedPatch()
    {
        Original = RequireMethod<Monster>(nameof(Monster.parried));
    }

    #region harmony patches

    /// <summary>Adds stamina cost to sword parry.</summary>
    [HarmonyPostfix]
    private static void MonsterParriedPostfix(Farmer who)
    {
        if (ModEntry.Config.WeaponsCostStamina)
            who.Stamina -= 2 - who.CombatLevel * 0.1f;
    }

    #endregion harmony patches
}