namespace DaLion.Core.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
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
    internal FarmerTakeDamagePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Implement blind status.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance, ref int damage, Monster? damager)
    {
        if (damager?.IsBlinded() != true || !Game1.random.NextBool())
        {
            return true; // run original logic
        }

        damage = -1;
        var missText = Game1.content.LoadString("Strings\\StringsFromCSFiles:Attack_Miss");
        __instance.currentLocation.debris.Add(new Debris(missText, 1, new Vector2(__instance.StandingPixel.X, __instance.StandingPixel.Y), Color.LightGray, 1f, 0f));
        return false; // don't run original logic
    }

    /// <summary>Reset seconds-out-of-combat.</summary>
    [HarmonyPostfix]
    private static void FarmerTakeDamagePostfix(Farmer __instance)
    {
        if (__instance.IsLocalPlayer)
        {
            State.SecondsOutOfCombat = 0;
        }
    }

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
                        new CodeInstruction(OpCodes.Ldloc_3), // loc 4 = int defense
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

        // Injected: if (CombatModule.Config.OverhauledDefense)
        //     skip
        //     {
        //         defense >= damage * 0.5f)
        //         defense -= (int) (defense * Game1.random.Next(3) / 10f);
        //     }
        var skipSoftCap = generator.DefineLabel();
        try
        {
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[4]),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.5f),
                    ],
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(CoreMod).RequirePropertyGetter(nameof(Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(CoreConfig).RequirePropertyGetter(nameof(CoreConfig.UseRationalMitigationFormula))),
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

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CalculateDamage(Farmer who, int rawDamage, int defense)
    {
        if (!Config.UseRationalMitigationFormula)
        {
            return Math.Max(1, rawDamage - defense);
        }

        var playerDefense = who.buffs.FloatingDefense();
        var weaponDefense = (who.CurrentTool as MeleeWeapon)?.addedDefense.Value ?? 0;
        var damage = (int)Math.Max(1, rawDamage * (10f / (10f + playerDefense + weaponDefense)));
        return damage;
    }

    #endregion injected subroutines
}
