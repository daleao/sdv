namespace DaLion.Professions.Framework.Patchers.Integration.ItemExtensions;

#region using directives

using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData;
using StardewValley.Internal;

#endregion using directives

[UsedImplicitly]
[ModRequirement("mistyspring.ItemExtensions", minimumVersion: "1.16.0")]
internal sealed class GeneralResourceTryExtraDropsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GeneralResourceTryExtraDropsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GeneralResourceTryExtraDropsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "ItemExtensions.Additions.GeneralResource"
            .ToType()
            .RequireMethod("TryExtraDrops");
    }

    #region harmony patches

    /// <summary>Patch for Gemologist mineral quality and increment counter for Item Extensions minerals.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GeneralResourceTryExtraDropsTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call, "ItemExtensions.Additions.GeneralResource".ToType().RequireMethod("IsGem"))
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[11]), // the item
                    new CodeInstruction(OpCodes.Ldarg_2), // the farmer
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GeneralResourceTryExtraDropsPatcher).RequireMethod(nameof(DoGemologistStuff))),
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Gemologist stuff.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    private static void DoGemologistStuff(Item parsedItem, Farmer who)
    {
        try
        {
            if (!who.HasProfession(Profession.Gemologist))
            {
                return;
            }

            var quality = who.GetGemologistMineralQuality();
            Data.AppendToGemologistMineralsCollected(parsedItem.ItemId, who);
            parsedItem.Quality = quality;
        }
        catch (Exception ex)
        {
            Log.E($"Failed doing Gemologist stuff for Item Extensions resource with id {parsedItem.ItemId}.\n{ex}");
        }
    }
}
