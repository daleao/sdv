namespace DaLion.Ligo.Modules.Professions.Patchers.Fishing;

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
internal sealed class FishingRodPlayerCaughtFishEndFunctionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodPlayerCaughtFishEndFunctionPatcher"/> class.</summary>
    internal FishingRodPlayerCaughtFishEndFunctionPatcher()
    {
        this.Target = this.RequireMethod<FishingRod>(nameof(FishingRod.playerCaughtFishEndFunction));
    }

    #region harmony patches

    /// <summary>Patch for remove annoying repeated message for recatching legendary fish.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodPlayerCaughtFishEndFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // From: if (isFishBossFish(whichFish))
        // To: if (isFishBossFish(whichFish) && !this.getLastFarmerToUse().fishCount.ContainsKey(whichFish)
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRod).RequireMethod(nameof(FishingRod.isFishBossFish))))
                .Advance()
                .GetOperand(out var dontShowMessage)
                .Advance()
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRod).RequireMethod(nameof(FishingRod.getLastFarmerToUse))),
                    new CodeInstruction(OpCodes.Ldfld, typeof(Farmer).RequireField(nameof(Farmer.fishCaught))),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(FishingRod).RequireField("whichFish")),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(NetIntIntArrayDictionary).RequireMethod(nameof(NetIntIntArrayDictionary.ContainsKey))),
                    new CodeInstruction(OpCodes.Brtrue_S, dontShowMessage));
        }
        catch (Exception ex)
        {
            Log.E($"Failed removing annoying legendary fish caught notification.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
