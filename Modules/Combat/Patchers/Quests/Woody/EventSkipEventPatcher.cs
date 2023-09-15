namespace DaLion.Overhaul.Modules.Combat.Patchers.Quests.Woody;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Constants;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EventSkipEventPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventSkipEventPatcher"/> class.</summary>
    internal EventSkipEventPatcher()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.skipEvent));
    }

    #region harmony patches

    /// <summary>Replaces rusty sword with wooden blade in Marlon's intro event.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? EventSkipEventTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var rusty = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ldstr, "Rusty Sword") })
                .Move(-1)
                .StripLabels(out var labels)
                .AddLabels(rusty)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Combat))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Config).RequirePropertyGetter(nameof(Config.WoodyReplacesRusty))),
                        new CodeInstruction(OpCodes.Brfalse_S, rusty),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(EventSkipEventPatcher).RequireMethod(nameof(AddSwordIfNecessary))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    labels)
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequireMethod(nameof(Farmer.addItemByMenuIfNecessary))),
                    })
                .Move()
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
            Game1.player.addItemByMenuIfNecessary(new MeleeWeapon(WeaponIds.WoodenBlade));
        }
    }

    #endregion injected subroutines
}
