namespace DaLion.Stardew.Professions.Framework.Patches.Mining;

#region using directives

using DaLion.Common;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Projectiles;
using System;
using System.Reflection;

#endregion using directives

// ReSharper disable PossibleLossOfFraction
[UsedImplicitly]
internal sealed class BasicProjectileExplodeOnImpact : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal BasicProjectileExplodeOnImpact()
    {
        Target = RequireMethod<BasicProjectile>(nameof(BasicProjectile.explodeOnImpact));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist explosive ammo radius.</summary>
    [HarmonyPrefix]
    private static bool BasicProjectileExplodeOnImpactPrefix(GameLocation location, int x, int y, Character who)
    {
        try
        {
            if (who is not Farmer farmer || !farmer.HasProfession(Profession.Demolitionist))
                return true; // run original logic

            location.explode(new(x / Game1.tileSize, y / Game1.tileSize),
                farmer.HasProfession(Profession.Demolitionist) ? 4 : 3, farmer);
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}