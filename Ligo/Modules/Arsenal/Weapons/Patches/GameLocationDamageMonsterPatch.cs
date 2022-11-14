namespace DaLion.Ligo.Modules.Arsenal.Weapons.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDamageMonsterPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDamageMonsterPatch"/> class.</summary>
    internal GameLocationDamageMonsterPatch()
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

    /// <summary>Guaranteed crit on underground Duggy from club smash attack.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

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
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(IsClubSmashHittingDuggy))),
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
                        typeof(GameLocationDamageMonsterPatch).RequireMethod(nameof(IsClubSmashHittingDuggy))),
                    new CodeInstruction(OpCodes.Brfalse_S, notCrit));
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash crit duggy.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsClubSmashHittingDuggy(MeleeWeapon weapon, Monster monster)
    {
        return ModEntry.Config.Arsenal.Weapons.GroundedClubSmash && weapon.type.Value == MeleeWeapon.club &&
               weapon.isOnSpecial && monster is Duggy;
    }

    #endregion injected subroutines
}
