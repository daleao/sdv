namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.DayEnding;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;
using Buff = StardewValley.Buff;

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

    /// <summary>
    ///     Patch to move critical chance bonus from Scout to Poacher + patch Brute damage bonus + move critical damage
    ///     bonus from Desperado to Poacher Ambush + perform Poacher steal and Piper buff actions + increment Piper Ultimate
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
                .MatchProfessionCheck(Profession.Fighter.Value) // find index of brute check
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R4, 1.1f) }) // brute damage multiplier
                .AddLabels(isNotPrestiged)
                .Insert(new[] { new CodeInstruction(OpCodes.Ldarg_S, (byte)10) }) // arg 10 = Farmer who
                .InsertProfessionCheck(Profession.Fighter.Value + 100, forLocalPlayer: false)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                        new CodeInstruction(OpCodes.Ldc_R4, 1.15f),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
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
                .MatchProfessionCheck(Profession.Brute.Value) // find index of brute check
                .Move(-2)
                .GetOperand(out var dontBuffDamage)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Brfalse_S, dontBuffDamage),
                        // check for local player
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Ldc_R4, 1.15f) }) // brute damage multiplier
                .SetOperand(1f)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModState).RequirePropertyGetter(nameof(ModState.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionState).RequirePropertyGetter(nameof(ProfessionState.BruteRageCounter))),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.01f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Add),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching modded Brute bonus damage.\nHelper returned {ex}");

            return null;
        }

        // From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f;
        // To: if (who is not null && who.IsLocalPlayer && crit && who.get_Ultimate() is Ambush ambush && ambush.ShouldBuffCritPow()) ... *= 2f;
        try
        {
            var ambush = generator.DeclareLocal(typeof(Ambush));
            helper
                .MatchProfessionCheck(Farmer.desperado) // find index of desperado check
                .Match(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, ILHelper.SearchOption.Previous)
                .GetOperand(out var dontBuffCritPow)
                .Match(new[] { new CodeInstruction(OpCodes.Ldnull) }, ILHelper.SearchOption.Previous)
                .ReplaceWith(new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow))
                .Move()
                .ReplaceWith(new CodeInstruction(OpCodes.Ldarg_S, (byte)10)) // was cgt ; arg 10 = Farmer who
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow),
                    })
                .Move()
                .Remove() // was and
                .Move()
                .Insert(
                    new[]
                    {
                        // check for ambush
                        new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Farmer_Ultimate).RequireMethod(nameof(Farmer_Ultimate.Get_Ultimate))),
                        new CodeInstruction(OpCodes.Isinst, typeof(Ambush)),
                        new CodeInstruction(OpCodes.Stloc_S, ambush),
                        new CodeInstruction(OpCodes.Ldloc_S, ambush),
                        new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow),
                        // check for crit. pow. buff
                        new CodeInstruction(OpCodes.Ldloc_S, ambush),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Ambush).RequirePropertyGetter(nameof(Ambush.IsGrantingCritBuff))),
                        new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow),
                    })
                .CountUntil(new[] { new CodeInstruction(OpCodes.Brfalse_S) }, out var count)
                .Remove(count);
        }
        catch (Exception ex)
        {
            Log.E($"Failed moving Desperado bonus crit damage to Poacher after-ult.\nHelper returned {ex}");

            return null;
        }

        // Injected: DamageMonsterSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who);
        // Before: if (monster.Health <= 0)
        try
        {
            var didCrit = helper.Locals[7];
            var damageAmount = helper.Locals[8];
            helper
                .Match(
                    new[]
                    {
                        // monster.Health <= 0
                        new CodeInstruction(OpCodes.Ldloc_2),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Monster).RequirePropertyGetter(nameof(Monster.Health))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Bgt),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    new[]
                    {
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
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}");

            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

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
        var ultimate = who.Get_Ultimate();
        if (who.HasProfession(Profession.Brute))
        {
            HandleBrute(monster, who, ultimate);
        }

        if (who.HasProfession(Profession.Poacher))
        {
            HandlePoacher(didCrit, critMultiplier, monster, who, ultimate, r);
        }

        if (monster.IsSlime() && monster.Health <= 0 && who.HasProfession(Profession.Piper))
        {
            HandlePiper(monster, who, ultimate, r);
        }
    }

    #endregion injected subroutines

    #region helper methods

    private static void HandleBrute(Monster monster, Farmer who, Ultimate? ultimate)
    {
        if (!ProfessionsModule.Config.Limit.EnableLimitBreaks || who.CurrentTool is not MeleeWeapon weapon ||
            ultimate is not Frenzy frenzy || monster.Health > 0)
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
            // increment Brute ultimate meter
            frenzy.ChargeValue += weapon.IsClub() ? 3 : 2; // more if wielding a club
        }
    }

    private static void HandlePoacher(bool didCrit, float critMultiplier, Monster monster, Farmer who, Ultimate? ultimate, Random r)
    {
        // try to steal
        var poached = TryPoach(monster, who, r);

        // increment Poacher ultimate meter
        if (!ProfessionsModule.Config.Limit.EnableLimitBreaks || ultimate is not Ambush { IsActive: false } ambush)
        {
            return;
        }

        if (monster.Health <= 0 && ambush.SecondsOutOfAmbush < 0.5d)
        {
            ultimate.ChargeValue += ultimate.MaxValue / 4d;
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

    private static void HandlePiper(Monster monster, Farmer who, Ultimate? ultimate, Random r)
    {
        // add Piper buffs
        var applied = ProfessionsModule.State.PiperBuffs;
        if (r.NextDouble() < (1d / 6d) + (who.DailyLuck / 2.0))
        {
            switch (r.Next(4))
            {
                case 0:
                    var healed = (int)(monster.MaxHealth * (who.HasProfession(Profession.Piper, true) ? 0.04f : 0.025f));
                    who.health = Math.Min(who.health + healed, who.maxHealth);
                    who.currentLocation.debris.Add(new Debris(
                        healed,
                        new Vector2(who.getStandingX() + 8, who.getStandingY()),
                        Color.Lime,
                        1f,
                        who));
                    Game1.playSound("healSound");
                    break;

                case 1:
                    var recovered = (int)(who.Stamina * (who.HasProfession(Profession.Piper, true) ? 0.01f : 0.02f));
                    who.Stamina = Math.Min(who.Stamina + recovered, who.MaxStamina);
                    who.currentLocation.debris.Add(new Debris(
                        recovered,
                        new Vector2(who.getStandingX() + 8, who.getStandingY()),
                        Color.Yellow,
                        1f,
                        who));
                    Game1.playSound("healSound");
                    break;

                case 2 when applied[10] < (who.HasProfession(Profession.Piper, true) ? 10 : 5):
                    applied[10]++;
                    RefreshPiperBuff(applied);
                    break;

                case 3 when applied[11] < (who.HasProfession(Profession.Piper, true) ? 10 : 5):
                    applied[10]++;
                    RefreshPiperBuff(applied);
                    break;
            }
        }
        else if (who.HasProfession(Profession.Piper, true) && r.NextDouble() < (1d / 6d) + (who.DailyLuck / 2.0))
        {
            var whatToBuff = r.Next(10);
            if (whatToBuff is not (3 or 6 or 7))
            {
                switch (whatToBuff)
                {
                    case 8:
                        if (applied[8] < 64)
                        {
                            applied[8] += 8;
                        }

                        break;

                    default:
                        if (applied[8] < 5)
                        {
                            applied[whatToBuff]++;
                        }

                        break;
                }

                RefreshPiperBuff(applied);
            }
        }

        // increment Piper ultimate meter
        if (ProfessionsModule.Config.Limit.EnableLimitBreaks &&
            ultimate is Concerto { IsActive: false } concerto)
        {
            var increment = monster switch
            {
                GreenSlime slime => 4 * slime.Scale,
                BigSlime => 8,
                _ => 0,
            };

            concerto.ChargeValue += increment + r.Next(-2, 3);
        }
    }

    private static bool TryPoach(Monster monster, Farmer who, Random r)
    {
        if (who.CurrentTool is not MeleeWeapon weapon)
        {
            return false;
        }

        float effectiveCritChance;
        if (CombatModule.ShouldEnable)
        {
            effectiveCritChance = weapon.Get_EffectiveCritChance();
        }
        else
        {
            effectiveCritChance = weapon.critChance.Value;
            if (weapon.type.Value == (int)WeaponType.Dagger)
            {
                effectiveCritChance += 0.005f;
                effectiveCritChance *= 1.12f;
            }

            effectiveCritChance *= 1f + who.critChanceModifier;
        }

        var actualResistance = (monster.resilience.Value - CombatModule.Config.Enemies.MonsterDefenseSummand) /
                               CombatModule.Config.Enemies.MonsterDefenseMultiplier;
        var poachChance = effectiveCritChance - ((actualResistance - who.LuckLevel) * monster.jitteriness.Value);
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
            .Select(o => new SObject(o, 1) as Item)
            .Concat(monster.getExtraDropItems())
            .Choose(r)?.getOne();
        if (itemToSteal is null || itemToSteal.Name.Contains("Error") || !who.addItemToInventoryBool(itemToSteal))
        {
            return false;
        }

        monster.IncrementPoached();
        SoundEffectPlayer.PoacherSteal.Play(who.currentLocation);
        if (who.HasProfession(Profession.Poacher, true))
        {
            monster.Poison(who, intensity: 2);
        }

        // if prestiged, reset cooldown
        //MeleeWeapon.attackSwordCooldown = 0;
        //MeleeWeapon.daggerCooldown = 0;
        //MeleeWeapon.clubCooldown = 0;
        //CombatModule.State.SlingshotCooldown = 0;
        return true;
    }

    private static string GetPiperBuffDescription(
        int farming,
        int fishing,
        int foraging,
        int mining,
        int attack,
        int defense,
        int luck,
        int speed,
        int magnetic)
    {
        var builder = new StringBuilder();
        if (farming > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.480") + "+" + farming);
            }
            else
            {
                builder.AppendLine("+" + farming + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.480"));
            }
        }

        if (fishing > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.483") + "+" + fishing);
            }
            else
            {
                builder.AppendLine("+" + fishing + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.483"));
            }
        }

        if (foraging > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.492") + "+" +
                                   foraging);
            }
            else
            {
                builder.AppendLine("+" + foraging +
                                   Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.492"));
            }
        }

        if (mining > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486") + "+" + mining);
            }
            else
            {
                builder.AppendLine("+" + mining + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486"));
            }
        }

        if (attack > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.504") + "+" + attack);
            }
            else
            {
                builder.AppendLine("+" + attack + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.504"));
            }
        }

        if (defense > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.501") + "+" + defense);
            }
            else
            {
                builder.AppendLine("+" + defense + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.501"));
            }
        }

        if (mining > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486") + "+" + mining);
            }
            else
            {
                builder.AppendLine("+" + mining + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.486"));
            }
        }

        if (luck > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.489") + "+" + luck);
            }
            else
            {
                builder.AppendLine("+" + luck + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.489"));
            }
        }

        if (speed > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.507") + "+" + speed);
            }
            else
            {
                builder.AppendLine("+" + speed + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.507"));
            }
        }

        if (magnetic > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.498") + "+" +
                                   magnetic);
            }
            else
            {
                builder.AppendLine("+" + magnetic +
                                   Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.498"));
            }
        }

        return builder.ToString();
    }

    private static void RefreshPiperBuff(int[] applied)
    {
        var buffId = (Manifest.UniqueID + Profession.Piper).GetHashCode();
        Game1.buffsDisplay.removeOtherBuff(buffId);
        Game1.buffsDisplay.addOtherBuff(new Buff(
            applied[0],
            applied[1],
            applied[2],
            0,
            applied[4],
            applied[5],
            0,
            0,
            applied[8],
            applied[9],
            applied[10],
            applied[11],
            3,
            "Piper",
            _I18n.Get("piper.title" + (Game1.player.IsMale ? ".male" : ".female")))
        {
            which = buffId,
            sheetIndex = 38,
            millisecondsDuration = 180000,
            description = GetPiperBuffDescription(
                applied[0],
                applied[1],
                applied[5],
                applied[2],
                applied[11],
                applied[10],
                applied[4],
                applied[9],
                applied[8]),
        });

        EventManager.Enable<PiperDayEndingEvent>();
    }

    #endregion helper methods
}
