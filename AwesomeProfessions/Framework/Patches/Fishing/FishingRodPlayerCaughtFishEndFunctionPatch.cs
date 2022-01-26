namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class FishingRodPlayerCaughtFishEndFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodPlayerCaughtFishEndFunctionPatch()
    {
        Original = RequireMethod<FishingRod>(nameof(FishingRod.playerCaughtFishEndFunction));
    }

    #region harmony patches

    /// <summary>Patch for remove annoying repeated message for recatching legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> FishingRodPlayerCaughtFishEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if (isFishBossFish(whichFish))
        /// To: if (isFishBossFish(whichFish) && !this.getLastFarmerToUse().fishCount.ContainsKey(whichFish)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call, typeof(FishingRod).MethodNamed(nameof(FishingRod.isFishBossFish)))
                )
                .Advance()
                .GetOperand(out var dontShowMessage)
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(FishingRod).MethodNamed(nameof(FishingRod.getLastFarmerToUse))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).Field(nameof(Farmer.fishCaught))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishingRod).Field("whichFish")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(NetIntIntArrayDictionary).MethodNamed(nameof(NetIntIntArrayDictionary.ContainsKey))),
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