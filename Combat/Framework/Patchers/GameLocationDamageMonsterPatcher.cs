namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Combat.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationDamageMonsterPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            [
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer), typeof(bool),
            ]);
    }

    #region harmony patches

    /// <summary>Record knockback for damage and crit. for defense ignore + back attacks.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: Monster.set_WasKnockedBack(true);
        // After: trajectory *= knockBackModifier;
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[7]), // local 7 = Vector2 trajectory
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)5), // arg 5 = float knockBackModifier
                        new CodeInstruction(OpCodes.Call),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[7]),
                    ],
                    ILHelper.SearchOption.First)
                .Move(4)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_KnockedBack).RequireMethod(nameof(Monster_KnockedBack.Set_KnockedBack))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording knocked back flag.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (BackAttack(Farmer farmer, Monster monster) critChance *= 2f;
        // After: if (who.professions.Contains(25)) critChance += critChance * 0.5f;
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Starg_S, (byte)7)]) // arg 7 = float critChance
                .Move()
                .AddLabels(resumeExecution)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(IsBackAttack))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)7),
                    new CodeInstruction(OpCodes.Ldc_R4, 2f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Starg_S, (byte)7),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting back attack.\nHelper returned {ex}");
            return null;
        }

        // Injected: Monster.set_GotCrit(true);
        // After: playSound("crit");
        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Ldstr, "crit")])
                .Move(3)
                .Insert(
                [
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.Set_GotCrit))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording crit flag.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsBackAttack(Farmer? farmer, Monster monster)
    {
        return Config.CriticalBackAttacks && farmer?.FacingDirection == monster.FacingDirection;
    }

    #endregion injected subroutines
}
