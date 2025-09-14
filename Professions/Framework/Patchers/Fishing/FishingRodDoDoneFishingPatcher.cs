namespace DaLion.Professions.Framework.Patchers.Fishing;

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
internal sealed class FishingRodDoDoneFishingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDoDoneFishingPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishingRodDoDoneFishingPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FishingRod>("doDoneFishing");
    }

    #region harmony patches

    /// <summary>Patch to consume Angler tackle.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishingRodDoDoneFishingPrefix(FishingRod __instance, bool consumeBaitAndTackle)
    {
        if (__instance.lastUser.HasProfession(Profession.Angler, true))
        {
            if (!__instance.fishCaught)
            {
                State.FishingChain = 0;
            }
        }

        if (!consumeBaitAndTackle)
        {
            return;
        }

        var memorizedTackle = Data.Read(__instance, DataKeys.FirstMemorizedTackle);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            Data.Increment(__instance, DataKeys.FirstMemorizedTackleUses, -1);
            if (Data.ReadAs<int>(__instance, DataKeys.FirstMemorizedTackleUses) <= 0)
            {
                Data.Write(__instance, DataKeys.FirstMemorizedTackle, null);
                Data.Write(__instance, DataKeys.FirstMemorizedTackleUses, null);
            }
        }

        if (__instance.AttachmentSlotsCount < 3)
        {
            return;
        }

        memorizedTackle = Data.Read(__instance, DataKeys.SecondMemorizedTackle);
        if (string.IsNullOrEmpty(memorizedTackle))
        {
            return;
        }

        Data.Increment(__instance, DataKeys.SecondMemorizedTackleUses, -1);
        if (Data.ReadAs<int>(__instance, DataKeys.SecondMemorizedTackleUses) > 0)
        {
            return;
        }

        Data.Write(__instance, DataKeys.SecondMemorizedTackle, null);
        Data.Write(__instance, DataKeys.SecondMemorizedTackleUses, null);
    }

    /// <summary>Patch to record Angler tackle uses.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? FishingRodDoDoneFishingTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch(
                [
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(FishingRod).RequireField(nameof(FishingRod.maxTackleUses))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldarg_0)])
                .Insert(
                [
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[5]),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRodDoDoneFishingPatcher).RequireMethod(nameof(RecordTackleMemory))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Angler tackle memory recording.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static void RecordTackleMemory(FishingRod rod, SObject? tackle)
    {
        if (tackle is null || !rod.lastUser.HasProfession(Profession.Angler))
        {
            return;
        }

        if (tackle.QualifiedItemId == rod.attachments[1]?.QualifiedItemId)
        {
            Data.Write(rod, DataKeys.FirstMemorizedTackle, tackle.QualifiedItemId);
            Data.Write(rod, DataKeys.FirstMemorizedTackleUses, ((FishingRod.maxTackleUses / 2) + 1).ToString());
            return;
        }

        if (rod.AttachmentSlotsCount >= 3 && tackle.QualifiedItemId == rod.attachments[2]?.QualifiedItemId)
        {
            Data.Write(rod, DataKeys.SecondMemorizedTackle, tackle.QualifiedItemId);
            Data.Write(rod, DataKeys.SecondMemorizedTackleUses, ((FishingRod.maxTackleUses / 2) + 1).ToString());
        }
    }

    #endregion injected
}
