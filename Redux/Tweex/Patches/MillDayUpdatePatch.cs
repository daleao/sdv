namespace DaLion.Redux.Tweex.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MillDayUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MillDayUpdatePatch"/> class.</summary>
    internal MillDayUpdatePatch()
    {
        this.Target = this.RequireMethod<Mill>(nameof(Mill.dayUpdate));
    }

    #region harmony patches

    /// <summary>Mills preserve quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MillDayUpdateTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        try
        {
            var input = generator.DeclareLocal(typeof(SObject));
            var @break = generator.DefineLabel();
            helper
                .ForEach(
                    new[] { new CodeInstruction(OpCodes.Newobj), new CodeInstruction(OpCodes.Stloc_1) },
                    () =>
                    {
                        var popThenBreak = generator.DefineLabel();
                        helper
                            .Advance(2)
                            .InsertInstructions(
                                new CodeInstruction(
                                    OpCodes.Call,
                                    typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Tweex))),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(Config).RequirePropertyGetter(nameof(Config.MillsPreserveQuality))),
                                new CodeInstruction(OpCodes.Brfalse_S, @break),
                                new CodeInstruction(OpCodes.Ldloc_1),
                                new CodeInstruction(OpCodes.Castclass, typeof(SObject)),
                                new CodeInstruction(OpCodes.Ldarg_0),
                                new CodeInstruction(OpCodes.Ldfld, typeof(Mill).RequireField(nameof(Mill.input))),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(NetFieldBase<Chest, NetRef<Chest>>).RequirePropertyGetter("Value")),
                                new CodeInstruction(OpCodes.Ldfld, typeof(Chest).RequireField(nameof(Chest.items))),
                                new CodeInstruction(OpCodes.Ldloc_0),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(NetList<Item, NetRef<Item>>).RequirePropertyGetter("Item")),
                                new CodeInstruction(OpCodes.Isinst, typeof(SObject)),
                                new CodeInstruction(OpCodes.Stloc_S, input),
                                new CodeInstruction(OpCodes.Ldloc_S, input),
                                new CodeInstruction(OpCodes.Brfalse_S, popThenBreak),
                                new CodeInstruction(OpCodes.Ldloc_S, input),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(SObject).RequirePropertyGetter(nameof(SObject.Quality))),
                                new CodeInstruction(
                                    OpCodes.Callvirt,
                                    typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))),
                                new CodeInstruction(OpCodes.Br_S, @break))
                            .InsertWithLabels(
                                new[] { popThenBreak },
                                new CodeInstruction(OpCodes.Pop));
                    })
                .AddLabels(@break);
        }
        catch (Exception ex)
        {
            Log.E($"Failed to add Mill quality preservation.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DayUpdateSubroutine(Mill mill, int index, Item toAdd)
    {
        ((SObject)toAdd).Quality =
            mill.input.Value.items[index] is SObject obj ? obj.Quality : SObject.lowQuality;
    }

    #endregion injected subroutines
}
