namespace DaLion.Overhaul.Modules.Professions.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodDoDoneFishingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodDoDoneFishingPatcher"/> class.</summary>
    internal FishingRodDoDoneFishingPatcher()
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
                .Match(new[]
                {
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(SObject).RequireField(nameof(SObject.uses))),
                })
                .Match(new[]
                {
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(NetFieldBase<int, NetInt>).RequirePropertySetter(
                            nameof(NetFieldBase<int, NetInt>.Value))),
                })
                .Move()
                .Insert(new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRodDoDoneFishingPatcher).RequireMethod(nameof(ConsumeTackleMemory))),
                });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Angler tackle memory consumption.\nHelper returned {ex}");
            return null;
        }

        try
        {
            helper
                .Match(new[]
                {
                    new CodeInstruction(
                        OpCodes.Ldsfld,
                        typeof(FishingRod).RequireField(nameof(FishingRod.maxTackleUses))),
                })
                .Match(new[] { new CodeInstruction(OpCodes.Ldarg_0) })
                .Insert(new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(FishingRodDoDoneFishingPatcher).RequireMethod(nameof(RecordTackleMemory))),
                });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Angler tackle memory recording.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void ConsumeTackleMemory(FishingRod instance)
    {
        if (string.IsNullOrEmpty(instance.Read(DataKeys.LastTackleUses)))
        {
            return;
        }

        instance.Increment(DataKeys.LastTackleUses, -1);
        if (instance.Read<int>(DataKeys.LastTackleUses) <= 0)
        {
            instance.Write(DataKeys.LastTackleUsed, null);
            instance.Write(DataKeys.LastTackleUses, null);
        }
    }

    private static void RecordTackleMemory(FishingRod instance)
    {
        if (!instance.getLastFarmerToUse().HasProfession(Profession.Angler))
        {
            return;
        }

        instance.Write(DataKeys.LastTackleUsed, instance.attachments[1].ParentSheetIndex.ToString());
        instance.Write(DataKeys.LastTackleUses, FishingRod.maxTackleUses.ToString());
    }

    #endregion injected subroutines
}
