namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodPlayerCaughtFishEndFunctionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodPlayerCaughtFishEndFunctionPatch()
    {
        Target = RequireMethod<FishingRod>(nameof(FishingRod.playerCaughtFishEndFunction));
    }

    #region harmony patches

    /// <summary>Patch for remove annoying repeated message for recatching legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodPlayerCaughtFishEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (isFishBossFish(whichFish))
        /// To: if (isFishBossFish(whichFish) && !this.getLastFarmerToUse().fishCount.ContainsKey(whichFish)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call, typeof(FishingRod).RequireMethod(nameof(FishingRod.isFishBossFish)))
                )
                .Advance()
                .GetOperand(out var dontShowMessage)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishingRod).RequireMethod(nameof(FishingRod.getLastFarmerToUse))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.fishCaught))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishingRod).RequireField("whichFish")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(NetIntIntArrayDictionary).RequireMethod(nameof(NetIntIntArrayDictionary.ContainsKey))),
                    new CodeInstruction(OpCodes.Brtrue_S, dontShowMessage)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while removing annoying legendary fish caught notification.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}