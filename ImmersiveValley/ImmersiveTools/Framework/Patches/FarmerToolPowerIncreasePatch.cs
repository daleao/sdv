namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerToolPowerIncreasePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerToolPowerIncreasePatch"/> class.</summary>
    internal FarmerToolPowerIncreasePatch()
    {
        this.Target = this.RequireMethod<Farmer>("toolPowerIncrease");
    }

    #region harmony patches

    /// <summary>Allow first two power levels on Pickaxe.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FarmerToolPowerIncreaseTranspiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var l = instructions.ToList();
        for (var i = 0; i < l.Count; ++i)
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
