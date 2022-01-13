using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using DaLion.Stardew.Professions.Framework.Extensions;

// ReSharper disable PossibleLossOfFraction

namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

[UsedImplicitly]
internal class BasicProjectileExplodeOnImpact : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BasicProjectileExplodeOnImpact()
    {
        Original = RequireMethod<BasicProjectile>(nameof(BasicProjectile.explodeOnImpact));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist explosive ammo radius.</summary>
    [HarmonyPrefix]
    private static bool BasicProjectileExplodeOnImpactPrefix(GameLocation location, int x, int y, Character who)
    {
        try
        {
            if (who is not Farmer farmer || !farmer.HasProfession("Demolitionist"))
                return true; // run original logic

            location.explode(new(x / Game1.tileSize, y / Game1.tileSize),
                farmer.HasPrestigedProfession("Demolitionist") ? 4 : 3, farmer);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}