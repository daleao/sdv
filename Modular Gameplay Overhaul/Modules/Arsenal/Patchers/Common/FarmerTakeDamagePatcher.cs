namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Common;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Arsenal.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerTakeDamagePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerTakeDamagePatcher"/> class.</summary>
    internal FarmerTakeDamagePatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.takeDamage));
    }

    #region harmony patches

    /// <summary>Grant i-frames during Stabbing sword lunge.</summary>
    [HarmonyPrefix]
    private static bool FarmerTakeDamagePrefix(Farmer __instance)
    {
        return __instance.CurrentTool is not MeleeWeapon { type.Value: MeleeWeapon.stabbingSword, isOnSpecial: true };
    }

    /// <summary>Overhaul for farmer defense.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FarmerTakeDamageTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: damage = Math.Max(1, damage - effectiveResilience);
        // To: damage = CalculateDamage(who, damage, effectiveResilience);
        //     x2
        try
        {
            helper
                .ForEach(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldc_I4_1),
                        new CodeInstruction(OpCodes.Ldarg_1), // arg 1 = int damage
                        new CodeInstruction(OpCodes.Ldloc_3), // loc 3 = int effectiveResilience
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Math).RequireMethod(nameof(Math.Max), new[] { typeof(int), typeof(int) })),
                    },
                    () =>
                    {
                        helper
                            .SetOpCode(OpCodes.Ldarg_0) // replace const int 1 with Farmer who
                            .Match(new[] { new CodeInstruction(OpCodes.Sub) })
                            .Remove()
                            .SetOperand(typeof(FarmerTakeDamagePatcher).RequireMethod(nameof(CalculateDamage)));
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding overhauled farmer defense (part 2).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static int CalculateDamage(Farmer who, int rawDamage, int vanillaResistance)
    {
        return ArsenalModule.Config.OverhauledDefense
            ? (int)(rawDamage * who.GetOverhauledResilience())
            : Math.Max(1, rawDamage - vanillaResistance);
    }

    #endregion injected subroutines
}
