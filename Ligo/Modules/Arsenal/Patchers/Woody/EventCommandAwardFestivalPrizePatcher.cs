namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class EventCommandAwardFestivalPrizePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="EventCommandAwardFestivalPrizePatcher"/> class.</summary>
    internal EventCommandAwardFestivalPrizePatcher()
    {
        this.Target = this.RequireMethod<Event>(nameof(Event.command_awardFestivalPrize));
    }

    #region harmony patches

    /// <summary>Replaces rusty sword with wooden blade in Marlon's intro event.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? EventCommandAwardFestivalPrizeTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var rusty = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Newobj, typeof(MeleeWeapon).RequireConstructor(typeof(int))),
                    })
                .AddLabels(rusty)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.WoodyReplacesRusty))),
                        new CodeInstruction(OpCodes.Brfalse_S, rusty),
                        new CodeInstruction(OpCodes.Ldc_I4_S, Constants.WoodenBladeIndex),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed replacing rusty sword festival reward with wooden blade.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
