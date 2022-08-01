namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationDamageMonsterPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.damageMonster), new[]
        {
            typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
            typeof(float), typeof(float), typeof(bool), typeof(Farmer)
        });
    }

    #region harmony patches

    /// <summary>Guaranteed crit on underground Duggy from club smash attack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (!monster.IsInvisible && ...
        /// To: if ((!monster.IsInvisible || who?.CurrentTool is MeleeWeapon && IsClubSmashHittingDuggy(who.CurrentTool as MeleeWeapon, monster)) && ...

        var resumeExecution1 = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Callvirt, typeof(NPC).RequirePropertyGetter(nameof(NPC.IsInvisible)))
                )
                .Advance()
                .GetOperand(out var skip)
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution1)
                )
                .Advance()
                .AddLabels(resumeExecution1)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Brfalse_S, skip),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Brfalse_S, skip),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(IsClubSmashHittingDuggy))),
                    new CodeInstruction(OpCodes.Brfalse, skip)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash hit duggy.\nHelper returned {ex}");
            return null;
        }

        /// From: if (who != null && Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)))
        /// To: if (who != null && (Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)) ||
        ///         who.CurrentTool is MeleeWeapon && isClubSmashHittingDuggy(who.CurrentTool as MeleeWeapon, monster))

        var doCrit = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldstr, "crit")
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Bge_Un_S)
                )
                .GetOperand(out var notCrit)
                .ReplaceWith(
                    new CodeInstruction(OpCodes.Blt_Un_S, doCrit)
                )
                .Advance()
                .AddLabels(doCrit)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Brfalse_S, notCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Callvirt, typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(IsClubSmashHittingDuggy))),
                    new CodeInstruction(OpCodes.Brfalse_S, notCrit)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash crit duggy.\nHelper returned {ex}");
            return null;
        }

        /// Injected: Monster.set_GotCrit(true);
        /// After: playSound("crit");
        
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldstr, "crit")
                )
                .Advance(3)
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.set_GotCrit)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording crit flag.\nHelper returned {ex}");
            return null;
        }

        /// From: else if (damageAmount > 0) { ... }
        /// To: else { DoSlingshotSpecial(monster, who); if (damageAmount > 0) { ... } }

        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(DoSlingshotSpecial)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding slingshot special stun.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsClubSmashHittingDuggy(MeleeWeapon weapon, Monster monster) =>
        ModEntry.Config.ImmersiveClubSmash && weapon.type.Value == MeleeWeapon.club && weapon.isOnSpecial &&
        monster is Duggy;

    private static void DoSlingshotSpecial(Monster monster, Farmer who)
    {
        if (who.CurrentTool is Slingshot slingshot && slingshot.get_IsOnSpecial())
            monster.stunTime = 2000;
    }

    #endregion injected subroutines
}