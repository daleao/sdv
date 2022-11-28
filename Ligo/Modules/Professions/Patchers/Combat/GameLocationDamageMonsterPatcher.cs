namespace DaLion.Ligo.Modules.Professions.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DaLion.Ligo.Modules.Professions.Events.GameLoop.DayEnding;
using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Ligo.Modules.Professions.Sounds;
using DaLion.Ligo.Modules.Professions.Ultimates;
using DaLion.Ligo.Modules.Professions.VirtualProperties;
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
                .FindProfessionCheck(Farmer.scout) // find index of scout check
                .Advance()
                .SetOperand(Profession.Poacher.Value); // replace with Poacher check
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving modded bonus crit chance from Scout to Poacher.\nHelper returned {ex}");
            return null;
        }

        // From: if (who is not null && who.professions.Contains(<fighter_id>) ... *= 1.1f;
        // To: if (who is not null && who.professions.Contains(<fighter_id>) ... *= who.professions.Contains(100 + <fighter_id>) ? 1.15f : 1.1f;
        try
        {
            var isNotPrestiged = generator.DefineLabel();
            var resumeExecution = generator.DefineLabel();
            helper
                .FindProfessionCheck(
                    Profession.Fighter.Value,
                    true) // find index of brute check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.1f)) // brute damage multiplier
                .AddLabels(isNotPrestiged)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10)) // arg 10 = Farmer who
                .InsertProfessionCheck(Profession.Fighter.Value + 100, forLocalPlayer: false)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, isNotPrestiged),
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution))
                .Advance()
                .AddLabels(resumeExecution);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching prestiged Fighter bonus damage.\nHelper returned {ex}");

            return null;
        }

        // From: if (who is not null && who.professions.Contains(<brute_id>)) ... *= 1.15f;
        // From: if (who is not null && who.IsLocalPlayer && who.professions.Contains(<brute_id>)) ... *= 1f + who.Get_BruteRageCounter() * 0.01f;
        try
        {
            helper
                .FindProfessionCheck(Profession.Brute.Value, true) // find index of brute check
                .Retreat(2)
                .GetOperand(out var dontBuffDamage)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffDamage),
                    // check for local player
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))))
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_R4, 1.15f)) // brute damage multiplier
                .SetOperand(1f)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Farmer_BruteCounters).RequireMethod(nameof(Farmer_BruteCounters.Get_BruteRageCounter))),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.01f),
                    new CodeInstruction(OpCodes.Mul),
                    new CodeInstruction(OpCodes.Add));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching modded Brute bonus damage.\nHelper returned {ex}");

            return null;
        }

        // From: if (who is not null && crit && who.professions.Contains(<desperado_id>) ... *= 2f;
        // To: if (who is not null && who.IsLocalPlayer && crit && who.get_Ultimate() is Ambush ambush && ambush.ShouldBuffCritPow()) ... *= 2f;
        try
        {
            var ambush = generator.DeclareLocal(typeof(Ambush));
            helper
                .FindProfessionCheck(Farmer.desperado, true) // find index of desperado check
                .RetreatUntil(new CodeInstruction(OpCodes.Brfalse_S))
                .GetOperand(out var dontBuffCritPow)
                .RetreatUntil(new CodeInstruction(OpCodes.Ldnull))
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow))
                .Advance()
                .ReplaceInstructionWith(new CodeInstruction(OpCodes.Ldarg_S, (byte)10)) // was cgt ; arg 10 = Farmer who
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow))
                .Advance()
                .RemoveInstructions() // was and
                .Advance()
                .InsertInstructions(
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
                    new CodeInstruction(OpCodes.Brfalse_S, dontBuffCritPow))
                .RemoveInstructionsUntil(
                    new CodeInstruction(OpCodes.Brfalse_S));
        }
        catch (Exception ex)
        {
            Log.E($"Failed while moving Desperado bonus crit damage to Poacher after-ult.\nHelper returned {ex}");

            return null;
        }

        // Injected: DamageMonsterSubroutine(damageAmount, isBomb, crit, critMultiplier, monster, who);
        // Before: if (monster.Health <= 0)
        try
        {
            var didCrit = helper.Locals[7];
            var damageAmount = helper.Locals[8];
            helper
                .FindFirst(
                    // monster.Health <= 0
                    new CodeInstruction(OpCodes.Ldloc_2),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Monster).RequirePropertyGetter(nameof(Monster.Health))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Bgt))
                .StripLabels(out var labels) // backup and remove branch labels
                .InsertWithLabels(
                    labels, // restore backed-up labels
                    // prepare arguments
                    new CodeInstruction(OpCodes.Ldloc_S, damageAmount),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)4), // arg 4 = bool isBomb
                    new CodeInstruction(OpCodes.Ldloc_S, didCrit),
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)8), // arg 8 = float critMultiplier
                    new CodeInstruction(OpCodes.Ldloc_2), // local 2 = Monster monster
                    new CodeInstruction(OpCodes.Ldarg_S, (byte)10), // arg 10 = Farmer who
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GameLocationDamageMonsterPatcher).RequireMethod(nameof(DamageMonsterSubroutine))));
        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed while injecting modded Poacher snatch attempt plus Brute Fury and Poacher Cold Blood gauges.\nHelper returned {ex}");

            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DamageMonsterSubroutine(
        int damageAmount, bool isBomb, bool didCrit, float critMultiplier, Monster monster, Farmer who)
    {
        if (damageAmount <= 0 || isBomb || !who.IsLocalPlayer)
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
        if (who.CurrentTool is not MeleeWeapon weapon || ultimate != Ultimate.BruteFrenzy || monster.Health > 0 ||
            !ModEntry.Config.Professions.EnableSpecials)
        {
            return;
        }

        if (ultimate.IsActive)
        {
            // increment kill count
            who.Increment_BruteKillCounter();
        }
        else
        {
            // increment ultimate
            ultimate.ChargeValue += weapon.IsClub() ? 3 : 2; // more if wielding a club
        }
    }

    private static void HandlePoacher(bool didCrit, float critMultiplier, Monster monster, Farmer who, Ultimate? ultimate, Random r)
    {
        // try to steal
        if (who.CurrentTool is MeleeWeapon && didCrit)
        {
            TrySteal(monster, who, r);

            // increment Poacher ultimate meter
            if (ultimate == Ultimate.PoacherAmbush && ultimate.IsActive)
            {
                ultimate.ChargeValue += critMultiplier;
            }
        }

        if (ultimate is not Ambush ambush || !ModEntry.Config.Professions.EnableSpecials)
        {
            return;
        }

        var wasActive = false;
        if (ultimate.IsActive)
        {
            wasActive = true;
            ultimate.ChargeValue = 0;
        }

        if (monster.Health > 0 || (!wasActive && !(ambush.SecondsOutOfAmbush <= 1.5d)))
        {
            return;
        }

        ultimate.ChargeValue += ultimate.MaxValue / 5d;
        ambush.SecondsOutOfAmbush = double.MaxValue;
    }

    private static void HandlePiper(Monster monster, Farmer who, Ultimate? ultimate, Random r)
    {
        // add Piper buffs
        if (r.NextDouble() < 0.16667 + (who.DailyLuck / 2.0))
        {
            var applied = who.Get_PiperBuffs();
            var whatToBuff = r.Next(applied.Length);
            if (whatToBuff is not (3 or 6))
            {
                switch (whatToBuff)
                {
                    case 8:
                        if (applied[8] < ModEntry.Config.Professions.PiperBuffCap * 8)
                        {
                            applied[8] += 8;
                        }

                        break;
                    case 7:
                        if (applied[7] < ModEntry.Config.Professions.PiperBuffCap * 10)
                        {
                            applied[7] += 10;
                        }

                        break;
                    default:
                        if (applied[8] < ModEntry.Config.Professions.PiperBuffCap)
                        {
                            applied[whatToBuff]++;
                        }

                        break;
                }

                var buffId = (ModEntry.Manifest.UniqueID + Profession.Piper).GetHashCode();
                Game1.buffsDisplay.removeOtherBuff(buffId);
                Game1.buffsDisplay.addOtherBuff(new Buff(
                    applied[0],
                    applied[1],
                    applied[2],
                    applied[3],
                    applied[4],
                    applied[5],
                    applied[6],
                    applied[7],
                    applied[8],
                    applied[9],
                    applied[10],
                    applied[11],
                    3,
                    "Piper",
                    ModEntry.i18n.Get("piper.title" + (who.IsMale ? ".male" : ".female")))
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
                        applied[7],
                        applied[8]),
                });

                ModEntry.Events.Enable<PiperDayEndingEvent>();
            }
        }

        // heal if prestiged
        if (who.HasProfession(Profession.Piper, true) && r.NextDouble() < 0.333 + (who.DailyLuck / 2.0))
        {
            var healed = (int)(monster.MaxHealth * 0.025f);
            who.health = Math.Min(who.health + healed, who.maxHealth);
            who.currentLocation.debris.Add(new Debris(
                healed,
                new Vector2(who.getStandingX() + 8, who.getStandingY()),
                Color.Lime,
                1f,
                who));
            Game1.playSound("healSound");

            who.Stamina = Math.Min(who.Stamina + (who.Stamina * 0.01f), who.MaxStamina);
        }

        // increment ultimate meter
        if (ultimate is Concerto { IsActive: false } concerto && ModEntry.Config.Professions.EnableSpecials)
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

    private static void TrySteal(Monster monster, Farmer who, Random r)
    {
        if (monster.Get_Stolen() || !(Game1.random.NextDouble() < 0.2))
        {
            return;
        }

        var drops = monster.objectsToDrop.Select(o => new SObject(o, 1) as Item)
            .Concat(monster.getExtraDropItems()).ToList();
        var itemToSteal = drops.ElementAtOrDefault(r.Next(drops.Count))?.getOne();
        if (itemToSteal is null || itemToSteal.Name.Contains("Error") || !who.addItemToInventoryBool(itemToSteal))
        {
            return;
        }

        monster.Set_Stolen(true);

        // play sound effect
        Sfx.PoacherSteal.Play();

        if (!who.HasProfession(Profession.Poacher, true))
        {
            return;
        }

        // if prestiged, reset cooldown
        MeleeWeapon.attackSwordCooldown = 0;
        MeleeWeapon.daggerCooldown = 0;
        MeleeWeapon.clubCooldown = 0;
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
        int energy,
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

        if (energy > 0)
        {
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es)
            {
                builder.AppendLine(Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.495") + "+" + energy);
            }
            else
            {
                builder.AppendLine("+" + energy + Game1.content.LoadString("Strings\\StringsFromCSFiles:Buff.cs.495"));
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

    #endregion helper methods
}
