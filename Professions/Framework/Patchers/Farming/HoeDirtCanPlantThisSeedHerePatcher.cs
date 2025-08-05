namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData.Crops;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtCanPlantThisSeedHerePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtCanPlantThisSeedHerePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal HoeDirtCanPlantThisSeedHerePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.canPlantThisSeedHere));
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Agriculturist crop endurance.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? HoeDirtCanPlantThisSeedHereTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Ldfld, typeof(CropData).RequireField(nameof(CropData.IsRaised))),
                ])
                .PatternMatch([new CodeInstruction(OpCodes.Brfalse_S)], ILHelper.SearchOption.Previous)
                .GetOperand(out var falseBranch)
                .PatternMatch([new CodeInstruction(OpCodes.Brtrue_S)], ILHelper.SearchOption.Previous)
                .GetOperand(out var trueBranch)
                .Return()
                .SetOpCode(OpCodes.Brtrue_S)
                .SetOperand(trueBranch)
                .Move()
                .Insert([
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(HoeDirtCanPlantThisSeedHerePatcher).RequireMethod(nameof(CanGrowWithRetainingSoil))),
                    new CodeInstruction(OpCodes.Brfalse_S, falseBranch)
                ]);
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting crop retention (UI).\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected

    private static bool CanGrowWithRetainingSoil(HoeDirt dirt)
    {
        return Game1.player.HasProfession(Profession.Agriculturist, true) &&
               dirt.GetFertilizerWaterRetentionChance() > 0f && dirt.Location.GetSeason() != Season.Winter;
    }

    #endregion injected
}
