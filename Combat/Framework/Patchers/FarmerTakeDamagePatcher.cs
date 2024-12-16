namespace DaLion.Combat.Framework.Patchers;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Core.Framework.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
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

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
                            typeof(CombatConfig).RequirePropertyGetter(nameof(CombatConfig.GeometricMitigationFormula))),
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

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CalculateDamage(Farmer who, int rawDamage, int defense)
    {
        if (!Config.GeometricMitigationFormula)
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
