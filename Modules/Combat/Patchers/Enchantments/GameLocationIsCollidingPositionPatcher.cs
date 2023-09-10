namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationIsCollidingPositionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationIsCollidingPositionPatcher"/> class.</summary>
    internal GameLocationIsCollidingPositionPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.isCollidingPosition),
            new[]
            {
                typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character),
                typeof(bool), typeof(bool), typeof(bool),
            });
    }

    #region harmony patches

    /// <summary>Collide with Wabbajack animals.</summary>
    [HarmonyPostfix]
    private static void GameLocationIsCollidingPositionPostfix(
        GameLocation __instance, ref bool __result, Rectangle position, bool isFarmer, Character? character)
    {
        if (__result || character is null || character is FarmAnimal)
        {
            return;
        }

        var playerBox = Game1.player.GetBoundingBox();
        var farmer = isFarmer ? character as Farmer : null;
        foreach (var animal in __instance.Get_Animals())
        {
            if (!position.Intersects(animal.GetBoundingBox()) ||
                (isFarmer && playerBox.Intersects(animal.GetBoundingBox())))
            {
                continue;
            }

            if (farmer is not null && farmer.TemporaryPassableTiles.Intersects(position))
            {
                break;
            }

            animal.farmerPushing();
            __result = true;
        }
    }

    #endregion harmony patches
}
