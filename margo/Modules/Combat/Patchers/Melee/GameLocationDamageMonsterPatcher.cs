namespace DaLion.Overhaul.Modules.Combat.Patchers.Melee;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Enchantments;
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

    /// <summary>KillerBug, Needle and Neptune enchantment effects.</summary>
    [HarmonyPrefix]
    private static void GameLocationDamageMonsterPrefix(ref float knockBackModifier, ref float critChance, Farmer who)
    {
        if (who.CurrentTool is not MeleeWeapon weapon)
        {
            return;
        }

        if (weapon.HasAnyEnchantmentOf(typeof(KillerBugEnchantment), typeof(SwordFishEnchantment)))
        {
            critChance = 0f;
        }

        if (weapon.hasEnchantmentOfType<NeedleEnchantment>())
        {
            critChance = 1f;
        }

        if (weapon.hasEnchantmentOfType<NeptuneEnchantment>())
        {
            knockBackModifier *= 4;
        }
    }

    /// <summary>Guaranteed crit on underground Duggy from club smash attack + record knockback and crit.</summary>
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
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(NPC).RequirePropertyGetter(nameof(NPC.IsInvisible))),
                    })
                .Move()
                .GetOperand(out var skip)
                .ReplaceWith(new CodeInstruction(OpCodes.Brfalse_S, resumeExecution))
                .Move()
                .AddLabels(resumeExecution)
                .Insert(
                    new[]
                    {
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
                        new CodeInstruction(OpCodes.Brfalse, skip),
                    });
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
                .Match(new[] { new CodeInstruction(OpCodes.Ldstr, "crit") })
                .Match(new[] { new CodeInstruction(OpCodes.Bge_Un_S) }, ILHelper.SearchOption.Previous)
                .GetOperand(out var notCrit)
                .ReplaceWith(new CodeInstruction(OpCodes.Blt_Un_S, doCrit))
                .Move()
                .AddLabels(doCrit)
                .Insert(
                    new[]
                    {
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
                        new CodeInstruction(OpCodes.Brfalse_S, notCrit),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding club smash crit duggy.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                        new CodeInstruction(OpCodes.Callvirt),
                        new CodeInstruction(OpCodes.Callvirt),
                        new CodeInstruction(OpCodes.Ldstr, "Galaxy Sword"),
                    })
                .CountUntil(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Multiplayer).RequireMethod(
                                nameof(Multiplayer.broadcastSprites),
                                new[] { typeof(GameLocation), typeof(TemporaryAnimatedSprite[]) })),
                    },
                    out var count)
                .Remove(count);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing arbitrary Galaxy Sword hit animation.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool IsClubSmashHittingDuggy(MeleeWeapon weapon, Monster monster)
    {
        return CombatModule.Config.WeaponsSlingshots.GroundedClubSmash && weapon.IsClub() &&
               weapon.isOnSpecial && monster is Duggy;
    }

    #endregion injected subroutines
}
