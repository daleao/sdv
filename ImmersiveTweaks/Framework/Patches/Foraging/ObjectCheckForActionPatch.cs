namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewModdingAPI.Enums;

using Common.Extensions;
using Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectCheckForActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectCheckForActionPatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.checkForAction));
    }

    #region harmony patches

    /// <summary>Detects if a tapper is ready for harvest.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectCheckForActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.name.Contains("Tapper") && __instance.heldObject.Value is not null &&
                  __instance.readyForHarvest.Value && ModEntry.Config.TappersRewardExp;
        return true; // run original logic
    }

    /// <summary>Adds foraging experience if a tapper was harvested.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForActionPostfix(SObject __instance, bool __state)
    {
        if (__state && !__instance.readyForHarvest.Value)
            Game1.player.gainExperience((int) SkillType.Foraging, 5);
    }

    /// <summary>Applies quality to aged bee house.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> ObjectCheckForActionTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: heldObject.Value.Quality = this.GetQualityFromAge();
        /// After: heldObject.Value.preservedParentSheetIndex.Value = honey_type;

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldstr, " Honey")
                )
                .FindNext(
                    new CodeInstruction(OpCodes.Ldfld, typeof(SObject).Field(nameof(SObject.preservedParentSheetIndex)))
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldarg_0)
                )
                .GetInstructionsUntil(out var got, false, true,
                    new CodeInstruction(OpCodes.Callvirt)
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Call, typeof(Game1).PropertyGetter(nameof(Game1.currentLocation)))
                )
                .Insert(got)
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SObjectExtensions).MethodNamed(nameof(SObjectExtensions.GetQualityFromAge))),
                    new CodeInstruction(OpCodes.Callvirt, typeof(SObject).PropertySetter(nameof(SObject.Quality)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while improving honey quality with age.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}