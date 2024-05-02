namespace DaLion.Overhaul.Modules.Combat.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Buff = DaLion.Shared.Enums.Buff;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerMovePositionImplPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerMovePositionImplPatcher"/> class.</summary>
    internal FarmerMovePositionImplPatcher()
    {
        this.Target = this.RequireMethod<Farmer>("MovePosition");
    }

    #region harmony patches

    /// <summary>Confusion effect.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerMovePosition(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                // shuffle up
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(ShuffleDirectionIfNecessary))),
                    })
                // shuffle down
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_2),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(ShuffleDirectionIfNecessary))),
                    })
                // shuffle right
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(ShuffleDirectionIfNecessary))),
                    })
                // shuffle left
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldfld,
                            typeof(Farmer).RequireField(nameof(Farmer.movementDirections))),
                        new CodeInstruction(OpCodes.Ldc_I4_3),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(List<int>).RequireMethod(nameof(List<int>.Contains))),
                    })
                .Move(2)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerMovePositionImplPatcher).RequireMethod(nameof(ShuffleDirectionIfNecessary))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 2).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int ShuffleDirectionIfNecessary(int direction)
    {
        return CombatModule.Config.EnableStatusConditions && Game1.player.hasBuff((int)Buff.Weakness)
            ? CombatModule.State.MovementDirections[direction]
            : direction;
    }

    #endregion injected subroutines
}
