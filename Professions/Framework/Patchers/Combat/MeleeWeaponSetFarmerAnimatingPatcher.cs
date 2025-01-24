namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponSetFarmerAnimatingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponSetFarmerAnimatingPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponSetFarmerAnimatingPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.setFarmerAnimating));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void MeleeWeaponSetFarmerAnimatingPrefix(Farmer who)
    {
        if (who.IsLocalPlayer && State.LimitBreak is PoacherAmbush { IsActive: true } ambush)
        {
            ambush.Deactivate();
        }
    }

    /// <summary>Patch to increase prestiged Brute attack speed with rage.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MeleeWeaponSetFarmerAnimatingTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Injected: if (who.professions.Contains(<brute_id>) swipeSpeed *= 1f - State.BruteRageCounter * 0.005f;
        // After: if (who.IsLocalPlayer)
        try
        {
            var skipRageBonus = generator.DefineLabel();
            helper
                .PatternMatch(
                    [
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(Farmer).RequirePropertyGetter(nameof(Farmer.IsLocalPlayer))),
                    ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)])
                .AddLabels(skipRageBonus)
                .Insert([new CodeInstruction(OpCodes.Ldarg_1)]) // arg 1 = Farmer who
                .InsertProfessionCheck(Farmer.brute, forLocalPlayer: false)
                .Insert([
                        new CodeInstruction(OpCodes.Brfalse_S, skipRageBonus),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                        new CodeInstruction(OpCodes.Ldc_R4, 1f),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ProfessionsMod).RequirePropertyGetter(nameof(State))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionsState).RequirePropertyGetter(nameof(ProfessionsState.BruteRageCounter))),
                        new CodeInstruction(OpCodes.Conv_R4),
                        new CodeInstruction(OpCodes.Ldc_R4, 0.005f),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Mul),
                        new CodeInstruction(OpCodes.Stfld, typeof(MeleeWeapon).RequireField("swipeSpeed")),
                    ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding attack speed to prestiged Brute.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
