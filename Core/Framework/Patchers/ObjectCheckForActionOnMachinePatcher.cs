namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.GameData.Machines;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForActionOnMachinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForActionOnMachinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectCheckForActionOnMachinePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>("CheckForActionOnMachine");
    }

    #region harmony patches

    /// <summary>Prevents remote item pickup when harvested by Hopper.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? ObjectCheckForActionOnMachineTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var skipHumanHarvest = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                ])
                .Move(-1)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ObjectCheckForActionOnMachinePatcher).RequireMethod(nameof(AttemptPushToHopper))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(
                        OpCodes
                        .Stloc_3), // can't get a reference to the LocalBuilder of this variable in order to load it by ref, so instead we set it by duplicating the result of the injected method
                    new CodeInstruction(OpCodes.Brtrue_S, skipHumanHarvest),
                ])
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SObject).RequireField(nameof(SObject.heldObject))),
                        new CodeInstruction(OpCodes.Ldnull),
                    ],
                    nth: 2)
                .AddLabels(skipHumanHarvest);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting anti-remote harvest with Prestiged Hopper.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool AttemptPushToHopper(SObject machine, MachineData machineData, SObject objectThatWasHeld, Farmer who)
    {
        var tileBelow = new Vector2(machine.TileLocation.X, machine.TileLocation.Y + 1f);
        if (machine.Location?.Objects.TryGetValue(tileBelow, out var objBelow) != true ||
            !objBelow.TryGetHopper(out var hopper))
        {
            var tileAbove = new Vector2(machine.TileLocation.X, machine.TileLocation.Y - 1f);
            if (machine.Location?.Objects.TryGetValue(tileAbove, out var objAbove) != true ||
                !objAbove.TryGetHopper(out hopper))
            {
                return false;
            }
        }

        // this should always be false
        if (hopper.GetOwner() != who)
        {
            return false;
        }

        machine.heldObject.Value = null;
        if (hopper.addItem(objectThatWasHeld) != null)
        {
            machine.heldObject.Value = objectThatWasHeld;
            return false;
        }

        machine.Location.playSound("coin");
        MachineDataUtility.UpdateStats(machineData?.StatsToIncrementWhenHarvested, objectThatWasHeld, objectThatWasHeld.Stack);
        return true;
    }

    #endregion injected
}
