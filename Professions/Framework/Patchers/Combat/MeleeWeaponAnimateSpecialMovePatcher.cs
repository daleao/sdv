namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class MeleeWeaponAnimateSpecialMovePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MeleeWeaponAnimateSpecialMovePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MeleeWeaponAnimateSpecialMovePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<MeleeWeapon>(nameof(MeleeWeapon.animateSpecialMove));
    }

    #region harmony patches

    /// <summary>Patch to enable Prestiged Brute rage expenditure.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? MeleeWeaponDoAnimateSpecialMoveTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Skipped: if (lastUser.professions.Contains(<acrobat_id>) cooldown /= 2
        try
        {
            var doSpecialMove = generator.DefineLabel();
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Callvirt, typeof(MeleeWeapon).RequireMethod("specialCooldown")),
                ])
                .Move(2)
                .GetOperand(out var dontDoSpecialMove)
                .ReplaceWith(new CodeInstruction(OpCodes.Ble_S, doSpecialMove))
                .Move()
                .AddLabels(doSpecialMove)
                .Insert([
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(MeleeWeaponAnimateSpecialMovePatcher).RequireMethod(nameof(DoSpendBruteRage))),
                    new CodeInstruction(OpCodes.Brfalse_S, dontDoSpecialMove),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Prestiged Brute rage expenditure.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static bool DoSpendBruteRage()
    {
        if (Game1.player.HasProfession(Profession.Brute, true) && State.BruteRageCounter >= 10)
        {
            State.BruteRageCounter -= 10;
            return true;
        }

        return false;
    }

    #endregion injections
}
