namespace DaLion.Overhaul.Modules.Professions.Patchers.Mining;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;

#endregion using directives

// ReSharper disable PossibleLossOfFraction
[UsedImplicitly]
internal sealed class BasicProjectileExplodeOnImpactPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BasicProjectileExplodeOnImpactPatcher"/> class.</summary>
    internal BasicProjectileExplodeOnImpactPatcher()
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
