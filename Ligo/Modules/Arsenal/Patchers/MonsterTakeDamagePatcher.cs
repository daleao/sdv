namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterTakeDamagePatcher"/> class.</summary>
    internal MonsterTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Monster>(
            nameof(Monster.takeDamage),
            new[] { typeof(int), typeof(int), typeof(int), typeof(bool), typeof(double), typeof(string) });
    }

    #region harmony patches

    /// <summary>Record overkill.</summary>
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
        var helper = new ILHelper(original, instructions);

        // From: int actualDamage = Math.Max(1, damage - (int)resilience);
        // To: int actualDamage = this.get_GotCrit() && ModEntry.Confi.Arsenal.OverhauledDefense ? damage : damage * 10 / (10 + (int)resilience);
        try
        {
            var mitigateDamage = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Arsenal))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ArsenalConfig).RequirePropertyGetter(nameof(ArsenalConfig.OverhauledDefense))),
                        new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage), new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.Get_GotCrit))),
                        new CodeInstruction(OpCodes.Brfalse_S, mitigateDamage), new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Stloc_0), new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Remove()
                .AddLabels(mitigateDamage)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Conv_R4), new CodeInstruction(OpCodes.Ldc_R4, 10f),
                        new CodeInstruction(OpCodes.Ldc_R4, 10f),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Sub) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Conv_R4), new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Div),
                    })
                .ReplaceWith(new CodeInstruction(OpCodes.Mul))
                .Move()
                .ReplaceWith(new CodeInstruction(OpCodes.Conv_I4))
                .Move(2)
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding crit. ignore defense option.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
