namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationExplodePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Patch to immunize treasure hunt target tile.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocation).RequireMethod(nameof(GameLocation.makeHoeDirt)))
                    ],
                    _ =>
                    {
                        helper
                            .Move()
                            .GetOperand(out var skipMakeHoeDirt)
                            .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)], ILHelper.SearchOption.Previous)
                            .Insert([
                                new CodeInstruction(OpCodes.Ldarg_0),
                                new CodeInstruction(OpCodes.Ldloc_1),
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(GameLocationExplodePatcher).RequireMethod(nameof(IsAnyoneHuntingThisTile))),
                                new CodeInstruction(OpCodes.Brtrue_S, skipMakeHoeDirt),
                            ])
                            .PatternMatch([new CodeInstruction(
                                OpCodes.Call,
                                typeof(GameLocation).RequireMethod(nameof(GameLocation.makeHoeDirt)))
                            ]);
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding treasure tile bomb immunity.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool IsAnyoneHuntingThisTile(GameLocation location, Vector2 tile)
    {
        var theLocalHuntIsOn = (State.ScavengerHunt?.IsActive ?? false) &&
                               ReferenceEquals(State.ScavengerHunt.Location, location) &&
                               State.ScavengerHunt.TargetTile == tile;
        if (theLocalHuntIsOn)
        {
            return true;
        }

        var anyGlobalHuntIsOn = Farmer_TreasureHunt.Values.Any(pair =>
            pair.Value.IsHuntingTreasure.Value &&
            pair.Value.LocationNameOrUniqueName == location.NameOrUniqueName &&
            pair.Value.TreasureTile == tile);
        return anyGlobalHuntIsOn;
    }

    #endregion injected
}
