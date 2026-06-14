namespace DaLion.Professions.Framework.Patchers.Integration.ItemExtensions;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("mistyspring.ItemExtensions", minimumVersion: "1.16.0")]
internal sealed class GeneralResourceCheckDropsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GeneralResourceCheckDropsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GeneralResourceCheckDropsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "ItemExtensions.Additions.GeneralResource"
            .ToType()
            .RequireMethod("CheckDrops");
    }

    #region harmony patches

    /// <summary>Patch for Gemologist mineral quality and increment counter for Item Extensions minerals.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? CheckDropsTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            var quality = generator.DeclareLocal(typeof(int));
            helper
                .PatternMatch([
                    new CodeInstruction(
                        OpCodes.Call, "ItemExtensions.Additions.GeneralResource".ToType().RequireMethod("IsGem"))
                ])
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Stloc_S, quality),
                ])
                .Move(2)
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        "ItemExtensions.Models.ResourceData".ToType()
                            .RequirePropertyGetter("ItemDropped")), // the item id
                    new CodeInstruction(OpCodes.Ldloc_0), // the farmer
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(GeneralResourceCheckDropsPatcher).RequireMethod(nameof(DoGemologistStuff))),
                    new CodeInstruction(OpCodes.Stloc_S, quality),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Ldc_I4_0)])
                .ReplaceWith(new CodeInstruction(OpCodes.Ldloc_S, quality));
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting Gemologist stuff.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    private static int DoGemologistStuff(string itemId, Farmer who)
    {
        try
        {
            if (!who.HasProfession(Profession.Gemologist))
            {
                return 0;
            }

            var quality = who.GetGemologistMineralQuality();
            Data.AppendToGemologistMineralsCollected(itemId, who);
            return quality;
        }
        catch (Exception ex)
        {
            Log.E($"Failed doing Gemologist stuff for Item Extensions resource with id {itemId}.\n{ex}");
            return 0;
        }
    }
}
