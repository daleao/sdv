namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class EventSkipEventPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="EventSkipEventPatch"/> class.</summary>
    internal EventSkipEventPatch()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.skipEvent));
    }

    #region harmony patches

    /// <summary>Replaces rusty sword with wooden blade in Marlon's intro event.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? EventSkipEventTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        try
        {
            var rusty = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, "Rusty Sword"))
                .Retreat()
                .StripLabels(out var labels)
                .AddLabels(rusty)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.WoodyReplacesRusty))),
                    new CodeInstruction(OpCodes.Brfalse_S, rusty),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(EventSkipEventPatch).RequireMethod(nameof(AddSwordIfNecessary))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .AdvanceUntil(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequireMethod(nameof(Farmer.addItemByMenuIfNecessary))))
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing rusty sword skipped event reward with wooden blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void AddSwordIfNecessary()
    {
        if (Game1.player.Items.All(item => item is not MeleeWeapon weapon || weapon.isScythe()))
        {
            Game1.player.addItemByMenuIfNecessary(new MeleeWeapon(Constants.WoodenBladeIndex));
        }
    }

    #endregion injected subroutines
}
