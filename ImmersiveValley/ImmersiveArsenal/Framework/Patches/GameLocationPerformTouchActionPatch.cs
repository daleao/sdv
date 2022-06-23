namespace DaLion.Stardew.Arsenal.Framework.Patches;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common;
using Common.Extensions.Reflection;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationPerformTouchActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationPerformTouchActionPatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.performTouchAction));
    }

    #region harmony patches

    /// <summary>Apply new galaxy sword conditions.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> GameLocationPerformTouchActionTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, 74)
        /// To: Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject, ModEntry.Config.TrulyLegendaryGalaxySword ? Constants.GALAXY_SOUL_INDEX_I : 74)
        ///     -- and also
        /// Injected: this.playSound("thunder");

        var trulyLegendaryGalaxySword = generator.DefineLabel();
        var resumeExecution = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldc_I4_S, 74)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.TrulyLegendaryGalaxySword))),
                    new CodeInstruction(OpCodes.Brtrue_S, trulyLegendaryGalaxySword)
                )
                .Advance()
                .AddLabels(resumeExecution)
                .Insert(
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] { trulyLegendaryGalaxySword },
                    new CodeInstruction(OpCodes.Ldc_I4, Constants.GALAXY_SOUL_INDEX_I)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Brtrue)
                )
                .Advance()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldstr, "thunder"),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(GameLocation).RequireMethod(nameof(GameLocation.playSound)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting custom legendary sword conditions.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static bool CheckLegendarySwordConditions()
    {
        if (ModEntry.Config.TrulyLegendaryGalaxySword)
        {
            return Game1.player.Items.Any(item =>
                       item.ParentSheetIndex == Constants.IRIDIUM_BAR_INDEX_I && item.Stack >= 20) &&
                   Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject,
                       Constants.PRISMATIC_SHARD_INDEX_I);
        }

        return Utility.IsNormalObjectAtParentSheetIndex(Game1.player.ActiveObject,
            Constants.PRISMATIC_SHARD_INDEX_I);
    }

    #endregion injected subroutines
}