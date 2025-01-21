namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationCheckGenericFishRequirementsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCheckGenericFishRequirementsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationCheckGenericFishRequirementsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>("CheckGenericFishRequirements");
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Angler boss fish chain.</summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction>? GameLocationCheckGenericFishRequirementsTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        try
        {
            helper
                .PatternMatch([new CodeInstruction(OpCodes.Stloc_S, helper.Locals[22])], ILHelper.SearchOption.Last)
                .Move()
                .StripLabels(out var labels)
                .Insert(
                    [
                        new CodeInstruction(OpCodes.Ldloc_S, helper.Locals[22]),
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_3),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(GameLocationCheckGenericFishRequirementsPatcher).RequireMethod(
                                nameof(GetFishingChainChance))),
                        new CodeInstruction(OpCodes.Add),
                        new CodeInstruction(OpCodes.Stloc_S, helper.Locals[22]),
                    ],
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching Prestiged Angler boss fish chance.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injections

    private static float GetFishingChainChance(Item fish, Farmer player)
    {
    #if DEBUG
        return fish.IsBossFish() ? 1f : 0f;
    #elif RELEASE
        return fish.IsBossFish() && player.IsLocalPlayer && player.HasProfession(Profession.Angler, true) &&
               State.FishingChain > 10
            ? 0.01f * (State.FishingChain - 10)
            : 0f;
    #endif
    }

    #endregion injections
}
