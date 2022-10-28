namespace DaLion.Stardew.Arsenal.Framework.Patches.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using DaLion.Stardew.Arsenal.Framework.VirtualProperties;
using HarmonyLib;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatch"/> class.</summary>
    internal MonsterTakeDamagePatch()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    [HarmonyPrefix]
    private static void MonsterTakeDamagePrefix(Monster __instance, int damage)
    {
        __instance.Set_Overkill(Math.Max(damage - __instance.Health, 0));
    }

    /// <summary>Crits ignore defense, which, btw, actually does something.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? MonsterTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // From: int actualDamage = Math.Max(1, damage - (int)resilience);
        // To: int actualDamage = this.get_GotCrit() && ModEntry.Config.CritsIgnoreDefense ? damage : damage * 10 / (10 + (int)resilience) ;
        try
        {
            var mitigateDamage = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.EnemyDefenseOverhaul))),
                    new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.Get_GotCrit))),
                    new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Stloc_0),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .RemoveInstructions()
                .AddLabels(mitigateDamage)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 10f),
                    new CodeInstruction(OpCodes.Ldc_R4, 10f))
                .AdvanceUntil(new CodeInstruction(OpCodes.Sub))
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Add),
                    new CodeInstruction(OpCodes.Div))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Mul))
                .Advance()
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Conv_I4))
                .Advance(2)
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding defense options.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
