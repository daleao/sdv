using System;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Monsters;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class GreenSlimeCollisionWithFarmerBehaviorPatch : BasePatch
{
    private const int FARMER_INVINCIBILITY_FRAMES_I = 72;

    /// <summary>Construct an instance.</summary>
    internal GreenSlimeCollisionWithFarmerBehaviorPatch()
    {
        Original = RequireMethod<GreenSlime>(nameof(GreenSlime.collisionWithFarmerBehavior));
    }

    #region harmony patches

    /// <summary>Patch to increment Piper Eubstance gauge and heal on contact with slime.</summary>
    [HarmonyPostfix]
    private static void GreenSlimeCollisionWithFarmerBehaviorPostfix(GreenSlime __instance)
    {
        var who = __instance.Player;
        if (!who.IsLocalPlayer || ModState.SuperModeIndex != Utility.Professions.IndexOf("Piper") ||
            ModState.SlimeContactTimer > 0) return;

        if (who.HasPrestigedProfession("Piper"))
        {
            var healed = __instance.DamageToFarmer / 3;
            healed += Game1.random.Next(Math.Min(-1, -healed / 8), Math.Max(1, healed / 8));
            healed = Math.Max(healed, 1);

            who.health = Math.Min(who.health + healed, who.maxHealth);
            __instance.currentLocation.debris.Add(new(healed,
                new(who.getStandingX() + 8, who.getStandingY()), Color.Lime, 1f, who));
        }

        if (!ModState.IsSuperModeActive)
            ModState.SuperModeGaugeValue +=
                (int) (Game1.random.Next(1, 10) * ((float) ModState.SuperModeGaugeMaxValue / 500));

        ModState.SlimeContactTimer = FARMER_INVINCIBILITY_FRAMES_I;
    }

    #endregion harmony patches
}