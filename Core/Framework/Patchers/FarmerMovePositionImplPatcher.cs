namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerMovePositionImplPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerMovePositionImplPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerMovePositionImplPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>("MovePosition");
    }

    #region harmony patches

    /// <summary>Confusion effect.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmerMovePosition(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                // shuffle up
                .PatternMatch([
                    new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(InvertDirectionIfNecessary))),
                ])
                // shuffle down
                .PatternMatch([
                    new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(InvertDirectionIfNecessary))),
                ])
                // shuffle right
                .PatternMatch([
                    new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(InvertDirectionIfNecessary))),
                ])
                // shuffle left
                .PatternMatch([
                    new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_3),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(InvertDirectionIfNecessary))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding shuffled movement.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static int InvertDirectionIfNecessary(int direction)
    {
        return Config.ConsistentFarmerDebuffs && Game1.player.hasBuff(BuffIDs.Weakness)
            ? (direction + 2) % 4
            : direction;
    }

    #endregion injected
}
