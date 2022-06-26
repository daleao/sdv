namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerToolPowerIncreasePatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerToolPowerIncreasePatch()
    {
        Target = RequireMethod<Farmer>("toolPowerIncrease");
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
                l[i].operand?.ToString() != "StardewValley.Tools.Pickaxe") continue;

            // inject branch over toolPower += 2
            l.Insert(i - 2, new(OpCodes.Br_S, l[i + 1].operand));
            break;
        }

        return l.AsEnumerable();
    }

    #endregion harmony patches
}