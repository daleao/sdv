namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Core.Framework.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerTakeDamagePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(Monster).RequireMethod(nameof(Monster.onDealContactDamage)))
                ])
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Brfalse)
                    ],
                    ILHelper.SearchOption.Previous)
                .GetOperand(out var endExecution)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(TryDodge))),
                    new CodeInstruction(OpCodes.Brtrue, endExecution)
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting dodge chance.\nHelper returned {ex}");
            return null;
        }

        // Injected: if (Config.OverhauledDefense)
        //     skip
        //     {
        //         defense >= damage * 0.5f)
        //         defense -= (int) (defense * Game1.random.Next(3) / 10f);
        //     }
        var skipSoftCap = generator.DefineLabel();
        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Conv_R4),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                ])
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(CombatMod).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.HyperbolicMitigationFormula))),
                        new CodeInstruction(OpCodes.Brtrue_S, skipSoftCap),
                    ],
                    labels)
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4])])
                .Move()
                .AddLabels(skipSoftCap);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting skip over vanilla defense cap.\nHelper returned {ex}");
            return null;
        }

        // From: damage = Math.Max(1, damage - defense);
        // To: damage = CalculateDamage(who, damage, defense);
        //     x2
        try
        {
            helper
                .ForEach(
                    [
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]), // loc 4 = int defense
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), [typeof(int), typeof(int)])),
                    ],
                    _ =>
                    {
                        helper
                            .SetOpCode(OpCodes.Ldarg_0) // replace const int 1 with Farmer who
                            .PatternMatch([new CodeInstruction(OpCodes.Sub)])
                            .Remove()
                            .SetOperand(typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(CalculateDamage)));
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static int CalculateDamage(Farmer who, int rawDamage, int defense)
    {
        var linearMitigatedDamage = Math.Max(0, rawDamage - defense);
        if (!Config.HyperbolicMitigationFormula)
        {
            return linearMitigatedDamage;
        }

        var weaponDefense = (who.CurrentTool as MeleeWeapon)?.addedDefense.Value ?? 0;
        var playerDefense = who.buffs.FloatingDefense() - weaponDefense;
        var bookDefense = who.stats.Get("Book_Defense") != 0 ? 1 : 0;
        var hyperbolicMitigatedDamage = (int)(rawDamage * (10f / (10f + playerDefense) * (10f / (10f + weaponDefense)) * (10f / (10f + bookDefense))));
        return Math.Min(hyperbolicMitigatedDamage, linearMitigatedDamage);
    }

    private static bool TryDodge(Farmer who)
    {
        if (!Config.LuckImprovesCritAndDodge || !Game1.random.NextBool(0.01f * who.LuckLevel))
        {
            return false;
        }

        who.temporarilyInvincible = true;
        who.flashDuringThisTemporaryInvincibility = false;
        who.temporaryInvincibilityTimer = 0;
        who.currentTemporaryInvincibilityDuration = 1200 + (who.GetEffectsOfRingMultiplier("861") * 400);
        var missText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Attack_Miss");
        var standingPixel = who.StandingPixel;
        who.currentLocation.debris.Add(
            new Debris(missText, 1, new Vector2(standingPixel.X + 8, standingPixel.Y), Color.LightGray, 1f, 0f)
            {
                toHover = who,
            });
        who.playNearbySoundAll("miss");
        return true;
    }

    #endregion injected
}
