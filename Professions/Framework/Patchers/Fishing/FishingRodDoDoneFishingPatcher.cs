namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDoDoneFishingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDoDoneFishingPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal FishingRodDoDoneFishingPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<FishingRod>("doDoneFishing");
    }

    #region harmony patches

    /// <summary>Patch to record Angler tackle uses.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? FishingRodDoDoneFishingTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(SObject).RequireField(nameof(SObject.uses))),
                ])
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequirePropertySetter(
                            nameof(NetFieldBase<int, NetInt>.Value))),
                ])
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRodDoDoneFishingPatcher).RequireMethod(nameof(ConsumeTackleMemory))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Angler tackle memory consumption.\nHelper returned {ex}");
            return null;
        }

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
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[6]),
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

    #region injections

    private static void ConsumeTackleMemory(FishingRod rod)
    {
        var memorizedTackle = Data.Read(rod, DataKeys.FirstMemorizedTackle);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            Data.Increment(rod, DataKeys.FirstMemorizedTackleUses, -1);
            if (Data.ReadAs<int>(rod, DataKeys.FirstMemorizedTackleUses) <= 0)
            {
                Data.Write(rod, DataKeys.FirstMemorizedTackle, null);
                Data.Write(rod, DataKeys.FirstMemorizedTackleUses, null);
            }
        }

        if (rod.AttachmentSlotsCount < 3)
        {
            return;
        }

        memorizedTackle = Data.Read(rod, DataKeys.SecondMemorizedTackle);
        if (!string.IsNullOrEmpty(memorizedTackle))
        {
            Data.Increment(rod, DataKeys.SecondMemorizedTackleUses, -1);
            if (Data.ReadAs<int>(rod, DataKeys.SecondMemorizedTackleUses) <= 0)
            {
                Data.Write(rod, DataKeys.SecondMemorizedTackle, null);
                Data.Write(rod, DataKeys.SecondMemorizedTackleUses, null);
            }
        }
    }

    private static void RecordTackleMemory(FishingRod rod, SObject tackle)
    {
        if (!rod.lastUser.HasProfession(Profession.Angler))
        {
            return;
        }

        if (rod.lastUser.HasProfession(Profession.Angler, true))
        {
            if (tackle.QualifiedItemId == rod.attachments[1].QualifiedItemId)
            {
                Data.Write(rod, DataKeys.FirstMemorizedTackle, rod.attachments[1].QualifiedItemId);
                Data.Write(rod, DataKeys.FirstMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
            }
            else if (rod.AttachmentSlotsCount >= 3 && tackle.QualifiedItemId == rod.attachments[2].QualifiedItemId)
            {
                Data.Write(rod, DataKeys.SecondMemorizedTackle, FishingRod.maxTackleUses.ToString());
                Data.Write(rod, DataKeys.SecondMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
            }

            return;
        }

        Data.Write(rod, DataKeys.FirstMemorizedTackle, tackle.QualifiedItemId);
        Data.Write(rod, DataKeys.FirstMemorizedTackleUses, (FishingRod.maxTackleUses / 2).ToString());
    }

    #endregion injections
}
