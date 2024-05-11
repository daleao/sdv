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
    internal ObjectCheckForActionOnMachinePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>("CheckForActionOnMachine");
    }

    #region harmony patches

    /// <summary>Prevents remote item pickup when harvested by Hopper.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectCheckForActionTranspiler(
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
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ObjectCheckForActionOnMachinePatcher).RequireMethod(nameof(AttemptPushToHopper))),
                    new CodeInstruction(OpCodes.Dup),
                    new CodeInstruction(
                        OpCodes
                        .Stloc_2), // can't get a reference to the LocalBuilder of this variable in order to load it by ref, so instead we set it by duplicating the result of the injected method
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

    #region injections

    private static bool AttemptPushToHopper(SObject machine, MachineData machineData, SObject objectThatWasHeld, Farmer who)
    {
        Chest hopper;
        var tileBelow = new Vector2(machine.TileLocation.X, machine.TileLocation.Y + 1f);
        if (machine.Location?.Objects.TryGetValue(tileBelow, out var objBelow) != true ||
            objBelow is not Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopperBelow)
        {
            var tileAbove = new Vector2(machine.TileLocation.X, machine.TileLocation.Y - 1f);
            if (machine.Location?.Objects.TryGetValue(tileAbove, out var objAbove) != true ||
                objAbove is not Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopperAbove)
            {
                return false;
            }

            hopper = hopperAbove;
        }
        else
        {
            hopper = hopperBelow;
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

    #endregion injections
}
