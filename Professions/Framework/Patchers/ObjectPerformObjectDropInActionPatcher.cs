namespace DaLion.Professions.Framework.Patchers;

using System.Reflection.Emit;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPerformObjectDropInActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Patch to remember initial machine state.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    [UsedImplicitly]
    private static bool ObjectPerformObjectDropInActionPrefix(
        SObject __instance, ref bool __result, out bool __state, Item dropInItem, bool probe, Farmer who)
    {
        __state = __instance.heldObject.Value !=
                  null && !probe; // remember whether this machine was already holding an object

        if (probe || !__instance.IsArtisanMachine() || !((dropInItem as SObject)?.IsPossibleMachineTreatment() ?? false) ||
            !who.HasProfession(Profession.Artisan, true) ||
            !Lookups.MachineTreatments.TryGetValue(__instance.QualifiedItemId, out var treatmentRules))
        {
            return true; // run original logic
        }

        if (__instance.isTemporarilyInvisible)
        {
            __result = false;
            return false; // don't run original logic
        }

        // apply machine treatment
        var treatmentCategory = Lookups.CategoryByTreatment.TryGetValue(dropInItem.QualifiedItemId, out var category)
            ? category
            : MachineTreatmentCategory.None;
        if (treatmentCategory == MachineTreatmentCategory.None)
        {
            return true; // run original logic
        }

        var appliedTreatments = Data.ReadAppliedMachineTreatments(__instance);
        if (treatmentCategory == MachineTreatmentCategory.Overclock)
        {
            if (appliedTreatments.OverclockCycles > 0)
            {
                Game1.showRedMessage(I18n.Objects_Machinetreatments_Cant_AlreadyApplied(dropInItem.DisplayName));
                __result = false;
                return false; // don't run original logic
            }

            appliedTreatments.OverclockCycles = 30;
        }
        else
        {
            if (!treatmentRules.Contains(treatmentCategory))
            {
                Game1.showRedMessage(I18n.Objects_Machinetreatments_Cant_NotApplicable());
                __result = false;
                return false; // don't run original logic
            }

            if (appliedTreatments.CoatingCategory == treatmentCategory)
            {
                var which = treatmentCategory switch
                {
                    MachineTreatmentCategory.Fermentation => I18n.Objects_Machinetreatments_Fermentation(),
                    MachineTreatmentCategory.Glazing => I18n.Objects_Machinetreatments_Glazing(),
                    MachineTreatmentCategory.Sealing => I18n.Objects_Machinetreatments_Sealing(),
                };

                Game1.showRedMessage(I18n.Objects_Machinetreatments_Cant_AlreadyApplied(which));
                __result = false;
                return false; // don't run original logic
            }

            appliedTreatments.CoatingCycles = 20;
            appliedTreatments.CoatingCategory = treatmentCategory;
        }

        dropInItem.ConsumeStack(1);
        Data.WriteAppliedMachineTreatments(__instance, appliedTreatments);
        __instance.Location.playSound("Ship");
        __result = true;
        return false; // don't run original logic
    }

    /// <summary>Patch to increase Artisan production + integrate Quality Artisan Products + Immersive Diary Yield tweak.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectPerformObjectDropInActionPostfix(
        SObject __instance, bool __state, Item dropInItem, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is not an artisan machine, or if the machine is still empty after running the original method, then do nothing
        if (__state || !__instance.IsArtisanMachine() || __instance.heldObject.Value is not { } output ||
            dropInItem is not SObject input)
        {
            return;
        }

        var user = who;
        var owner = __instance.GetOwner();
        var r = Random.Shared;
        var newQuality = ObjectQuality.Regular;

        // artisan users can preserve the input quality
        if (__instance.QualifiedItemId != QIDs.Cask && user.HasProfession(Profession.Artisan))
        {
            newQuality = (ObjectQuality)input.Quality;
            if (r.NextDouble() > who.FarmingLevel / 60d)
            {
                newQuality = newQuality.Decrement();
                if (r.NextDouble() > who.FarmingLevel / 30d)
                {
                    newQuality = newQuality.Decrement();
                }
            }
        }

        output.Quality = Math.Max(output.Quality, (int)newQuality);

        if (!owner.HasProfessionOrLax(Profession.Artisan))
        {
            return;
        }

        // artisan-owned machines calibrate to repeated ingredients
        if (input.QualifiedItemId == __instance.lastInputItem.Value?.QualifiedItemId)
        {
            var repeatedCycles = Data.ReadAs<int>(__instance, DataKeys.RepeatedInputCycles);
            var calibrationLevel = Math.Min(repeatedCycles, 10);
            var calibrationBonus = calibrationLevel * 0.025;
            if (__instance is Cask cask)
            {
                cask.daysToMature.Value -= (int)Math.Floor(cask.daysToMature.Value * calibrationBonus);
            }
            else
            {
                __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady * calibrationBonus);
            }

            Data.Increment(__instance, DataKeys.RepeatedInputCycles);
        }
        else
        {
            Data.Write(__instance, DataKeys.RepeatedInputCycles, null);
        }

        // apply machinist calibration bonus
        if (!owner.HasProfession(Profession.Artisan, true))
        {
            return;
        }

        // apply machinist machine treatment bonus
        if (!Lookups.MachineTreatments.TryGetValue(__instance.QualifiedItemId, out var treatmentRules))
        {
            return;
        }

        // determine treatment category for this input
        var inputTreatmentCategory = treatmentRules.Default;
        var contextTags = input.GetContextTags();
        var matchingTag = treatmentRules
            .Overrides
            .Keys
            .LastOrDefault(contextTags.Contains);
        if (!string.IsNullOrEmpty(matchingTag))
        {
            inputTreatmentCategory = treatmentRules.Overrides[matchingTag];
        }

        if (treatmentRules.Overrides.TryGetValue(input.QualifiedItemId, out var overrideCategory))
        {
            inputTreatmentCategory = overrideCategory;
        }

        if (inputTreatmentCategory == MachineTreatmentCategory.None)
        {
            return;
        }

        // get applied treatments to this machine
        var appliedTreatments = Data.ReadAppliedMachineTreatments(__instance);
        if (appliedTreatments.CoatingCategory == inputTreatmentCategory && appliedTreatments.CoatingCycles-- > 0)
        {
            if (output.Quality < SObject.bestQuality)
            {
                output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
            }

            if (appliedTreatments.CoatingCycles == 0)
            {
                appliedTreatments.CoatingCategory = MachineTreatmentCategory.None;
            }
        }

        if (appliedTreatments.OverclockCycles-- > 0)
        {
            __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady / 2d);
        }

        Data.WriteAppliedMachineTreatments(__instance, appliedTreatments);
    }

    #endregion harmony patches
}
