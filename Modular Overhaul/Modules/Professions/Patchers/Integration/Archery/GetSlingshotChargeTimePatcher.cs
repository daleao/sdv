namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
internal sealed class GetSlingshotChargeTimePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GetSlingshotChargeTimePatcher"/> class.</summary>
    internal GetSlingshotChargeTimePatcher()
    {
        this.Target = "Archery.Framework.Objects.Weapons.Bow"
            .ToType()
            .RequireMethod("GetSlingshotChargeTime");
        this.Transpiler!.after = new[] { OverhaulModule.Slingshots.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to reduce Bow charge time for Desperado.</summary>
    [HarmonyTranspiler]
    [HarmonyAfter("DaLion.Overhaul.Modules.Slingshots")]
    private static IEnumerable<CodeInstruction>? GetSlingshotChargeTimeTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_S, helper.Locals[4]) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_1), // local 1 = Farmer farmer
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GetSlingshotChargeTimePatcher).RequireMethod(nameof(GetBonusChargeTime))),
                        new CodeInstruction(OpCodes.Mul),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Desperado charge bonus to Bows.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static float GetBonusChargeTime(Farmer farmer)
    {
        return farmer.HasProfession(Profession.Desperado)
            ? 1f - MathHelper.Lerp(0f, 0.5f, Math.Clamp(1f - ((float)farmer.health / farmer.maxHealth), 0f, 1f))
            : 1f;
    }

    #endregion injected subroutines
}
