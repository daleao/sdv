namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class StatsMonsterKilledPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="StatsMonsterKilledPatcher"/> class.</summary>
    internal StatsMonsterKilledPatcher()
    {
        this.Target = this.RequireMethod<Stats>(nameof(Stats.monsterKilled));
    }

    #region harmony patches

    /// <summary>Update virtue progress.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? StatsMonsterKilledTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var virtuesQuest = generator.DeclareLocal(typeof(VirtueQuest));
            var resumeExecution = generator.DefineLabel();
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Ret) })
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModState).RequirePropertyGetter(nameof(ModState.Weapons))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(State).RequirePropertyGetter(nameof(State.VirtuesQuest))),
                        new CodeInstruction(OpCodes.Stloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Ldloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Ldloc_S, virtuesQuest),
                        new CodeInstruction(OpCodes.Ldsfld, typeof(Virtue).RequireField(nameof(Virtue.Valor))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Call, typeof(VirtueQuest).RequireMethod(nameof(VirtueQuest.UpdateVirtueProgress))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Valor progress check.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
