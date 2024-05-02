namespace DaLion.Chargeable.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

#endregion using directives

[HarmonyPatch(typeof(Farmer), nameof(Farmer.toolPowerIncrease))]
internal sealed class FarmerToolPowerIncreasePatcher
{
    #region harmony patches

    /// <summary>Allow first two power levels on Pick.</summary>
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        for (var i = 0; i < l.Count; i++)
        {
            if (l[i].opcode != OpCodes.Isinst ||
                l[i].operand?.ToString() != "StardewValley.Tools.Pickaxe")
            {
                continue;
            }

            // inject branch over toolPower += 2
            l.Insert(i - 2, new CodeInstruction(OpCodes.Br_S, l[i + 1].operand));
            break;
        }

        return l.AsEnumerable();
    }

    #endregion harmony patches
}
