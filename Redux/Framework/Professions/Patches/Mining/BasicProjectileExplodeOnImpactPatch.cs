namespace DaLion.Redux.Framework.Professions.Patches.Mining;

#region using directives

using System.Reflection;
using DaLion.Redux.Framework.Professions.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

// ReSharper disable PossibleLossOfFraction
[UsedImplicitly]
internal sealed class BasicProjectileExplodeOnImpactPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileExplodeOnImpactPatch"/> class.</summary>
    internal BasicProjectileExplodeOnImpactPatch()
    {
        this.Target = this.RequireMethod<BasicProjectile>(nameof(BasicProjectile.explodeOnImpact));
    }

    #region harmony patches

    /// <summary>Patch to increase Demolitionist explosive ammo radius.</summary>
    [HarmonyPrefix]
    private static bool BasicProjectileExplodeOnImpactPrefix(GameLocation location, int x, int y, Character who)
    {
        try
        {
            if (who is not Farmer farmer || !farmer.HasProfession(Profession.Demolitionist))
            {
                return true; // run original logic
            }

            location.explode(
                new Vector2(x / Game1.tileSize, y / Game1.tileSize),
                farmer.HasProfession(Profession.Demolitionist) ? 4 : 3,
                farmer);
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
