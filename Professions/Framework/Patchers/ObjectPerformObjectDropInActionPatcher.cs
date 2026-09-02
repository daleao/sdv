namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
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
        var treatmentCategory = MachineTreatmentRegistry.FromCatalyst(dropInItem.QualifiedItemId);
        if (treatmentCategory == MachineTreatmentRegistry.None)
        {
            return true; // run original logic
        }

        var appliedTreatments = Data.ReadAppliedMachineTreatments(__instance);
        if (treatmentCategory == MachineTreatmentRegistry.Overclock)
        {
            if (appliedTreatments.OverclockCycles > 0)
            {
                Game1.showRedMessage(I18n.Machines_Coatings_Cant_AlreadyApplied(dropInItem.DisplayName));
                __result = false;
                return false; // don't run original logic
            }

            appliedTreatments.OverclockCycles = 30;
        }
        else
        {
            if (!treatmentRules.Contains(treatmentCategory))
            {
                Game1.showRedMessage(I18n.Machines_Coatings_Cant_NotApplicable());
                __result = false;
                return false; // don't run original logic
            }

            var currentCoatingCategory = MachineTreatmentRegistry.FromCatalyst(appliedTreatments.CoatingCatalyst);
            if (treatmentCategory == currentCoatingCategory && treatmentCategory != MachineTreatmentRegistry.None)
            {
                Game1.showRedMessage(I18n.Machines_Coatings_Cant_AlreadyApplied(treatmentCategory.DisplayName.ToLower()));
                __result = false;
                return false; // don't run original logic
            }

            appliedTreatments.CoatingCycles = 20;
            appliedTreatments.CoatingCatalyst = dropInItem.QualifiedItemId;
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
        if (__state || !__instance.IsArtisanMachine() || __instance is Cask ||
            __instance.heldObject.Value is not { } output || dropInItem is not SObject input)
        {
            return;
        }

        var user = who;
        var owner = __instance.GetOwner();
        var r = Random.Shared;
        var newQuality = ObjectQuality.Regular;

        var calibrationPerItem = Data.Read(__instance, DataKeys.CalibrationPerItem).ParseDictionary<string, int>();
        var inputHasCalibration = calibrationPerItem.TryGetValue(input.QualifiedItemId, out var calibration);
        if (inputHasCalibration)
        {
            // artisan users can preserve the input quality above 50 calibration
            if (__instance is not Cask && user.HasProfession(Profession.Artisan) &&
                calibration >= 50)
            {
                var chance = who.FarmingLevel / 60d;
                if (calibration >= 100)
                {
                    chance *= 2;
                }

                newQuality = (ObjectQuality)input.Quality;
                if (r.NextDouble() > chance)
                {
                    newQuality = newQuality.Decrement();
                    if (r.NextDouble() > chance / 2d)
                    {
                        newQuality = newQuality.Decrement();
                    }
                }
            }

            output.Quality = Math.Max(output.Quality, (int)newQuality);

            // artisan-owned machines calibrate to repeated ingredients
            var calibrationSpeedup = calibration / 400d;
            if (__instance is not Cask && owner.HasProfessionOrLax(Profession.Artisan))
            {
                __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady * calibrationSpeedup);
            }
        }

        if (user.HasProfession(Profession.Artisan))
        {
            // re-calibrate
            if (!inputHasCalibration || calibration < 100)
            {
                if (!Config.ModKey.IsDown())
                {
                    foreach (var key in calibrationPerItem.Keys.ToList())
                    {
                        if (key == input.QualifiedItemId)
                        {
                            continue;
                        }

                        calibrationPerItem[key] -= 1;
                        if (!user.HasProfession(Profession.Artisan, true))
                        {
                            calibrationPerItem[key] -= 1;
                        }

                        if (calibrationPerItem[key] <= 0)
                        {
                            calibrationPerItem.Remove(key);
                        }
                    }

                    calibrationPerItem[input.QualifiedItemId] = Math.Min(calibration + 4, 100);
                    Data.Write(__instance, DataKeys.CalibrationChanged, "true".ToString());
                    Data.Write(__instance, DataKeys.CalibrationLocked, "false".ToString());
                }
                else
                {
                    Data.Write(__instance, DataKeys.CalibrationChanged, "false".ToString());
                    Data.Write(__instance, DataKeys.CalibrationLocked, "true".ToString());
                }
            }
            else
            {
                Data.Write(__instance, DataKeys.CalibrationChanged, "false".ToString());
                Data.Write(__instance, DataKeys.CalibrationLocked, "false".ToString());
            }
        }

        Data.Write(__instance, DataKeys.CalibrationPerItem, calibrationPerItem.Stringify());
        __instance.Set_Calibrations(calibrationPerItem);

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

        if (inputTreatmentCategory == MachineTreatmentRegistry.None)
        {
            return;
        }

        // get applied treatments to this machine
        var appliedTreatments = Data.ReadAppliedMachineTreatments(__instance);
        var currentCoatingCategory = MachineTreatmentRegistry.FromCatalyst(appliedTreatments.CoatingCatalyst);
        if (currentCoatingCategory == inputTreatmentCategory && appliedTreatments.CoatingCycles-- > 0)
        {
            output.MakePremium(appliedTreatments.CoatingCatalyst);
            if (output.Quality < SObject.bestQuality)
            {
                output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
            }

            if (appliedTreatments.CoatingCycles == 0)
            {
                appliedTreatments.CoatingCatalyst = string.Empty;
            }
        }

        if (appliedTreatments.OverclockCycles > 0)
        {
            appliedTreatments.OverclockCycles--;
            __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady / 2d);
        }

        Data.WriteAppliedMachineTreatments(__instance, appliedTreatments);
    }

    #endregion harmony patches
}
