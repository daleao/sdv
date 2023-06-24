namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Slingshots.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

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
        this.Transpiler!.before = new[] { OverhaulModule.Professions.Namespace };
    }

    #region harmony patches

    /// <summary>Add Emerald Ring and Emerald Enchantment bonuses to Bows.</summary>
    [HarmonyTranspiler]
    [HarmonyBefore("DaLion.Overhaul.Modules.Professions")]
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
                        new CodeInstruction(OpCodes.Ldarg_0), // arg 0 = Slingshot this
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetTotalFiringSpeedModifier))),
                        new CodeInstruction(OpCodes.Mul),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adding Emerald bonuses to Bows.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
