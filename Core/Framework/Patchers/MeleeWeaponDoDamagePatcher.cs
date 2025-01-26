namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Classes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponDoDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponDoDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponDoDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.DoDamage));
    }

    #region harmony patches

    /// <summary>Fixes dumb vanilla Scythe AoE which does nothing.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoDamageTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: foreach (Vector2 v in Utility.removeDuplicates(Utility.getListOfTileLocationsForBordersOfNonTileRectangle(areaOfEffect)))
        // To: foreach (Vector2 v in ListInnerTiles(areaOfEffect, this))
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Utility).RequireMethod(
                            nameof(Utility.getListOfTileLocationsForBordersOfNonTileRectangle)))
                ])
                .ReplaceWith(new CodeInstruction(OpCodes.Ldarg_0))
                .Move()
                .ReplaceWith(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponDoDamagePatcher).RequireMethod(nameof(ListInnerTiles))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting inner tile enumerator.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static List<Vector2> ListInnerTiles(Rectangle rectange, MeleeWeapon weapon)
    {
        if (!weapon.isScythe())
        {
            return Utility.getListOfTileLocationsForBordersOfNonTileRectangle(rectange);
        }

        var radius = weapon.addedAreaOfEffect.Value;
        var tiles = radius == 0
            ? Utility.getListOfTileLocationsForBordersOfNonTileRectangle(rectange)
            : new CircleTileGrid(
                new Vector2(
                    rectange.Center.X / Game1.tileSize,
                    rectange.Center.Y / Game1.tileSize),
                (uint)radius).Tiles;
        return tiles.ToList();
    }

    #endregion injected subroutines
}
