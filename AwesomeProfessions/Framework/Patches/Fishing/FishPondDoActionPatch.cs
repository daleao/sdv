namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Netcode;
using StardewValley.Buildings;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class FishPondDoActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDoActionPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.doAction));
    }

    #region harmony patches

    /// <summary>Patch to allow legendary fish share ponds with their extended families.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishPondDoActionTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (who.ActiveObject.ParentSheetIndex != (int) fishType)
        /// To: if (who.ActiveObject.ParentSheetIndex != (int) fishType && !IsExtendedFamily(who.ActiveObject.ParentSheetIndex, (int) fishType)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishPond).Field(nameof(FishPond.fishType))),
                    new CodeInstruction(OpCodes.Call, typeof(NetFieldBase<int, NetInt>).MethodNamed("op_Implicit")),
                    new CodeInstruction(OpCodes.Beq)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldloc_0)
                )
                .GetInstructionsUntil(out var got, true, true,
                    new CodeInstruction(OpCodes.Beq)
                )
                .Insert(got)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishPondDoActionPatch).MethodNamed(nameof(IsExtendedFamilyMember)))
                )
                .SetOpCode(OpCodes.Brtrue_S);
        }
        catch (Exception ex)
        {
            Log.E($"Failed while adding family ties to legendary fish in ponds.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static bool IsExtendedFamilyMember(int heldFish, int pondFish)
    {
        return ModEntry.Config.EnableFishPondRebalance &&
               ObjectLookups.ExtendedFamilyPairs.TryGetValue(pondFish, out var pair) && pair == heldFish;
    }

    #endregion private methods
}