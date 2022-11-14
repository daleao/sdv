namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForActionPatcher"/> class.</summary>
    internal ObjectCheckForActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.checkForAction));
    }

    #region harmony patches

    /// <summary>Detects if an object is ready for harvest.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectCheckForActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value is not null &&
                  __instance.readyForHarvest.Value;
        return true; // run original logic
    }

    /// <summary>Adds foraging experience if a tapper or mushroom box was harvested.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForActionPostfix(SObject __instance, bool __state)
    {
        if (!__state || __instance.readyForHarvest.Value)
        {
            return;
        }

        if (__instance.name.Contains("Tapper") && ModEntry.Config.Tweex.TappersRewardExp)
        {
            Game1.player.gainExperience(Farmer.foragingSkill, 5);
        }
        else if (__instance.name.Contains("Mushroom Box") && ModEntry.Config.Tweex.MushroomBoxesRewardExp)
        {
            Game1.player.gainExperience(Farmer.foragingSkill, 1);
        }
    }

    /// <summary>Applies quality to aged bee house.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ObjectCheckForActionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new IlHelper(original, instructions);

        // Injected: heldObject.Value.Quality = this.GetQualityFromAge();
        // After: heldObject.Value.preservedParentSheetIndex.Value = honey_type;
        try
        {
            helper
                .FindFirst(new CodeInstruction(OpCodes.Ldstr, " Honey"))
                .FindNext(
                    new CodeInstruction(
                        OpCodes.Ldfld,
                        typeof(SObject).RequireField(nameof(SObject.preservedParentSheetIndex))))
                .RetreatUntil(new CodeInstruction(OpCodes.Ldarg_0))
                .GetInstructionsUntil(
                    out var got,
                    false,
                    true,
                    new CodeInstruction(OpCodes.Callvirt))
                .AdvanceUntil(
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Game1).RequirePropertyGetter(nameof(Game1.currentLocation))))
                .InsertInstructions(got)
                .InsertInstructions(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(
                        OpCodes.Call,
                        typeof(Extensions.SObjectExtensions).RequireMethod(nameof(Extensions.SObjectExtensions.GetQualityFromAge))),
                    new CodeInstruction(
                        OpCodes.Callvirt,
                        typeof(SObject).RequirePropertySetter(nameof(SObject.Quality))));
        }
        catch (Exception ex)
        {
            Log.E($"Failed improving honey quality with age.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}
