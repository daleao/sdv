namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Core.Framework.Extensions;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Collections;
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

    /// <summary>
    ///     Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage
    ///     bonus from Desperado to Poacher Ambush + perform Poacher steal and Piper buff actions + increment Piper LimitBreak
    ///     meter.
    /// </summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? GameLocationDamageMonsterTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (who.professions.Contains(<scout_id>) critChance += critChance * 0.5f;
        // To: if (who.professions.Contains(<poacher_id>) critChance += critChance * 0.5f;
        try
        {
            helper
                .MatchProfessionCheck(Farmer.scout) // find index of scout check
                .Move()
                .SetOperand(Profession.Poacher.Value); // replace with Poacher check
        }
        catch (Exception ex)
        {
            Log.E($"Failed moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
            return null;
        }

        // From: if (who is not null && who.professions.Contains(<fighter_id>) ... *= 1.1f;
        // To: if (who is not null && who.professions.Contains(<fighter_id>) ... *= who.professions.Contains(100 + <fighter_id>) ? 1.15f : 1.1f;
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .MatchProfessionCheck(Farmer.fighter) // find index of brute check
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R4, 1.1f)]) // brute damage multiplier
                .AddLabels(isNotPrestiged)
                .Insert([new CodeInstruction(OpCodes.Ldarg_S, (byte)10)]) // arg 10 = Farmer who
                .InsertProfessionCheck(Farmer.fighter + 100, forLocalPlayer: false)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution),
                ])
                .Move()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching prestiged Fighter bonus damage.\nHelper returned {ex}");

            return null;
        }

        // From: if (who is not null && who.professions.Contains(<brute_id>)) ... *= 1.15f;
        // From: if (who is not null && who.IsLocalPlayer && who.professions.Contains(<brute_id>)) ... *= 1f + who.Get_BruteRageCounter() * 0.01f;
        try
        {
            helper
                .MatchProfessionCheck(Farmer.brute) // find index of brute check
                .Move(-2)
                .GetOperand(out var dontBuffDamage)
                .Insert([
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffDamage),
                    // check for local player
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_R4, 1.15f)]) // brute damage multiplier
                .SetOperand(1f)
                .Move()
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(ProfessionsMod).RequirePropertyGetter(nameof(State))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(ProfessionsState).RequirePropertyGetter(nameof(ProfessionsState.BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.01f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Brute bonus damage.\nHelper returned {ex}");

            return null;
        }

        // Removed: if (who != null && crit && who.professions.Contains(29)) damageAmount = (int)((float)damageAmount * 2f);
        try
        {
            helper
                .MatchProfessionCheck(Farmer.desperado) // find index of desperado check
                .Move(-1)
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_S)], ILHelper.SearchOption.Previous)
                .StripLabels(out var labels)
                .RemoveUntil([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[9])])
                .AddLabels(labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing vanilla Desperado crit. bonus.\nHelper returned {ex}");

            return null;
        }

        // Injected: DamageMonsterSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who);
        // Before: if (monster.Health <= 0)
        try
        {
            var didCrit = helper.Locals[8];
            var damageAmount = helper.Locals[9];
            helper
                .PatternMatch(
                    [
                        // monster.Health <= 0
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Health))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt_S),
                    ],
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    [
                        // prepare arguments
                        new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = bool isBomb
                        new CodeInstruction(OpCodes.Ldloc_S, didCrit),
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)8), // arg 8 = float critMultiplier
                        new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(DamageMonsterSubroutine))),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed injecting modded Poacher snatch attempt plus Brute and Poacher Limit gauges.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static void DamageMonsterSubroutine(
        int damageAmount, bool isBomb, bool didCrit, float critMultiplier, Monster monster, Farmer who)
    {
        if (damageAmount <= 0 || isBomb)
        {
            return;
        }

        if (monster.Get_Taunter() is not null)
        {
            monster.Set_Taunter(null);
        }

        if (!who.IsLocalPlayer)
        {
            return;
        }

        var r = new Random(Guid.NewGuid().GetHashCode());
        if (who.HasProfession(Profession.Brute))
        {
            HandleBrute(monster, who);
        }

        if (who.HasProfession(Profession.Poacher))
        {
            HandlePoacher(didCrit, critMultiplier, monster, who, r);
        }

        if (monster.IsSlime() && monster.Health <= 0 && who.HasProfession(Profession.Piper))
        {
            HandlePiper(monster, who, r);
        }
    }

    #endregion injections

    #region helper methods

    private static void HandleBrute(Monster monster, Farmer who)
    {
        if (!Config.Masteries.EnableLimitBreaks || who.CurrentTool is not MeleeWeapon weapon ||
            State.LimitBreak is not BruteFrenzy frenzy || monster.Health > 0)
        {
            return;
        }

        if (frenzy.IsActive)
        {
            // increment kill count
            frenzy.KillCount++;
        }
        else
        {
            // increment Brute Limit Break meter
            frenzy.ChargeValue += weapon.IsClub() ? 3 : 2; // more if wielding a club
        }
    }

    private static void HandlePoacher(bool didCrit, float critMultiplier, Monster monster, Farmer who, Random r)
    {
        // try to steal
        var poached = TryPoach(monster, who, r);

        // increment Poacher Limit Break meter
        if (!Config.Masteries.EnableLimitBreaks || State.LimitBreak is not PoacherAmbush { IsActive: false } ambush)
        {
            return;
        }

        if (monster.Health <= 0 && ambush.SecondsOutOfAmbush < 0.5d)
        {
            ambush.ChargeValue += LimitBreak.MaxCharge / 2d;
            ambush.SecondsOutOfAmbush = double.MaxValue;
        }

        if (poached)
        {
            ambush.ChargeValue += critMultiplier;
        }

        if (who.CurrentTool is MeleeWeapon && didCrit)
        {
            ambush.ChargeValue += critMultiplier;
        }
    }

    private static void HandlePiper(Monster monster, Farmer who, Random r)
    {
        if (!Config.Masteries.EnableLimitBreaks ||
            State.LimitBreak is not PiperConcerto { IsActive: false } concerto)
        {
            return;
        }

        // increment Piper Limit Break meter
        var increment = monster switch
        {
            GreenSlime slime => 4f * slime.Scale,
            BigSlime => 8f,
            _ => 0f,
        };

        concerto.ChargeValue += increment + r.Next(-2, 3);
    }

    private static bool TryPoach(Monster monster, Farmer who, Random r)
    {
        if (who.CurrentTool is not MeleeWeapon weapon)
        {
            return false;
        }

        var effectiveCritChance = weapon.critChance.Value;
        if (weapon.type.Value == (int)WeaponType.Dagger)
        {
            effectiveCritChance += 0.005f;
            effectiveCritChance *= 1.12f;
        }

        effectiveCritChance *= 1f + who.buffs.CriticalChanceMultiplier;
        var poachChance = effectiveCritChance -
                          ((monster.resilience.Value - who.LuckLevel) * monster.jitteriness.Value);
        if (r.NextDouble() > poachChance)
        {
            return false;
        }

        var maxPoachCount = 1;
        if (who.HasProfession(Profession.Poacher, true))
        {
            maxPoachCount++;
        }

        if (monster.Get_Poached().Value >= maxPoachCount)
        {
            return false;
        }

        var itemToSteal = monster.objectsToDrop
            .Select(id => ItemRegistry.Create($"(O){id}"))
            .Concat(monster.getExtraDropItems())
            .Choose(r)?.getOne();
        if (itemToSteal is null || itemToSteal.Name.Contains("Error") || !who.addItemToInventoryBool(itemToSteal))
        {
            return false;
        }

        monster.IncrementPoached();
        SoundBox.PoacherSteal.PlayLocal();
        if (who.HasProfession(Profession.Poacher, true))
        {
            monster.Poison(who, stacks: 2, maxStacks: 4);
        }

        return true;
    }

    #endregion helper methods
}
