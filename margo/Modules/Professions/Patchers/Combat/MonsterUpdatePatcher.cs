namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterUpdatePatcher"/> class.</summary>
    internal MonsterUpdatePatcher()
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.update), new[] { typeof(GameTime), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Patch to become aggroed by musked monsters.</summary>
    [HarmonyPostfix]
    private static void MonsterUpdatePostfix(Monster __instance, GameTime time)
    {
        if (__instance.Get_Musk() is not { } musk)
        {
            return;
        }

        for (var i = 0; i < __instance.currentLocation.characters.Count; i++)
        {
            var character = __instance.currentLocation.characters[i];
            if (character is not Monster { IsMonster: true } monster ||
                !ReferenceEquals(monster.Get_Target(), musk.FakeFarmer) || !monster.CanBeDamaged())
            {
                continue;
            }

            var monsterBox = monster.GetBoundingBox();
            if (!monsterBox.Intersects(__instance.GetBoundingBox()))
            {
                continue;
            }

            // damage monster
            var randomizedDamage = __instance.DamageToFarmer +
                                   Game1.random.Next(-__instance.DamageToFarmer / 4, __instance.DamageToFarmer / 4);
            var damageToMonster = (int)Math.Max(1, randomizedDamage * __instance.Scale) - monster.resilience.Value;
            var (xTrajectory, yTrajectory) = monster.Slipperiness < 0
                ? Vector2.Zero
                : Utility.getAwayFromPositionTrajectory(monsterBox, __instance.getStandingPosition()) / 2f;
            monster.takeDamage(damageToMonster, (int)xTrajectory, (int)yTrajectory, false, 1d, "hitEnemy");
            monster.currentLocation.debris.Add(new Debris(
                damageToMonster,
                new Vector2(monsterBox.Center.X + 16, monsterBox.Center.Y),
                new Color(255, 130, 0),
                1f,
                monster));
            monster.setInvincibleCountdown(450);

            // get damaged by monster
            randomizedDamage = monster.DamageToFarmer +
                               Game1.random.Next(-monster.DamageToFarmer / 4, monster.DamageToFarmer / 4);
            var damageToSlime = Math.Max(1, randomizedDamage) - __instance.resilience.Value;
            __instance.takeDamage(damageToSlime, (int)-xTrajectory, (int)-yTrajectory, false, 1d, "slime");
            if (__instance.Health <= 0)
            {
                break;
            }

            __instance.Set_Taunter(monster);
        }
    }

    #endregion harmony patches
}
