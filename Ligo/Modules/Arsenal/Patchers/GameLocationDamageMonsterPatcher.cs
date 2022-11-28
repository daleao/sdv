namespace DaLion.Ligo.Modules.Arsenal.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using DaLion.Ligo.Modules.Core.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatcher"/> class.</summary>
    internal GameLocationDamageMonsterPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(
            nameof(GameLocation.damageMonster),
            new[]
            {
                typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float), typeof(int),
                typeof(float), typeof(float), typeof(bool), typeof(Farmer),
            });
    }

    #region harmony patches

    /// <summary>Guaranteed crit on underground Duggy from club smash attack + record knockback.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (!monster.IsInvisible && ...
        // To: if ((!monster.IsInvisible || who?.CurrentTool is MeleeWeapon && IsClubSmashHittingDuggy(who.CurrentTool as MeleeWeapon, monster)) && ...
        try
        {
            var resumeExecution = generator.DefineLabel();
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Callvirt, typeof(NPC).RequirePropertyGetter(nameof(NPC.IsInvisible))))
                .Advance()
                .GetOperand(out var skip)
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Brfalse_S, resumeExecution))
                .Advance()
                .AddLabels(resumeExecution)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(OpCodes.Brfalse_S, skip),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Brfalse_S, skip),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(IsClubSmashHittingDuggy))),
                    new CodeInstruction(OpCodes.Brfalse, skip));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash hit duggy.\nHelper returned {ex}");
            return null;
        }

        // From: if (who != null && Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)))
        // To: if (who != null && (Game1.random.NextDouble() < (double)(critChance + (float)who.LuckLevel * (critChance / 40f)) ||
        //         who.CurrentTool is MeleeWeapon && isClubSmashHittingDuggy(who.CurrentTool as MeleeWeapon, monster))
        try
        {
            var doCrit = generator.DefineLabel();
            helper
                .FindNext(new CodeInstruction(OpCodes.Ldstr, "crit"))
                .RetreatUntil(new CodeInstruction(OpCodes.Bge_Un_S))
                .GetOperand(out var notCrit)
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Blt_Un_S, doCrit))
                .Advance()
                .AddLabels(doCrit)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Brfalse_S, notCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.CurrentTool))),
                    new CodeInstruction(OpCodes.Isinst, typeof(MeleeWeapon)),
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(IsClubSmashHittingDuggy))),
                    new CodeInstruction(OpCodes.Brfalse_S, notCrit));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash crit duggy.\nHelper returned {ex}");
            return null;
        }

        // From: else if (damageAmount > 0) { ... }
        // To: else { DoSlingshotSpecial(monster, who); if (damageAmount > 0) { ... } }
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[8]),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Ble))
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(DoSlingshotSpecial))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding slingshot special stun.\nHelper returned {ex}");
            return null;
        }

        // Injected: Monster.set_WasKnockedBack(true);
        // After: trajectory *= knockBackModifier;
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[6]),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)5),
                    new CodeInstruction(OpCodes.Call),
                    new CodeInstruction(OpCodes.Stloc_S, helper.Locals[6]))
                .Advance(4)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_WasKnockedBack).RequireMethod(nameof(Monster_WasKnockedBack.Set_WasKnockedBack))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed recording knocked back flag.\nHelper returned {ex}");
            return null;
        }

        // Injected: Monster.set_GotCrit(true);
        // After: playSound("crit");
        try
        {
            helper
                .FindNext(new CodeInstruction(OpCodes.Ldstr, "crit"))
                .Advance(3)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Monster_GotCrit).RequireMethod(nameof(Monster_GotCrit.Set_GotCrit))));
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

    private static bool IsClubSmashHittingDuggy(MeleeWeapon weapon, Monster monster)
    {
        return ModEntry.Config.Arsenal.Weapons.GroundedClubSmash && weapon.IsClub() &&
               weapon.isOnSpecial && monster is Duggy;
    }

    private static void DoSlingshotSpecial(Monster monster, Farmer who)
    {
        if (who.CurrentTool is Slingshot slingshot && slingshot.Get_IsOnSpecial())
        {
            monster.Stun(slingshot.hasEnchantmentOfType<ReduxArtfulEnchantment>() ? 3000 : 2000);
        }
    }

    #endregion injected subroutines
}
