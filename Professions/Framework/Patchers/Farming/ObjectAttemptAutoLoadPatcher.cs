namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Inventories;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectAttemptAutoLoadPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectAttemptAutoLoadPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectAttemptAutoLoadPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target =
            this.RequireMethod<SObject>(nameof(SObject.AttemptAutoLoad), [typeof(IInventory), typeof(Farmer)]);
    }

    #region harmony patches

    /// <summary>Patch for Machinist auto treatment.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ObjectAttemptAutoLoadPrefix(SObject __instance, ref bool __result, IInventory inventory, Farmer who)
    {
        if (!__instance.IsArtisanMachine() || !who.HasProfession(Profession.Artisan, true) ||
            !Lookups.MachineTreatments.TryGetValue(__instance.QualifiedItemId, out var treatmentRules))
        {
            return true; // run original logic
        }

        if (__instance.heldObject.Value is not null)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (!inventory.Any(item => (item as SObject)?.IsPossibleMachineTreatment() ?? false))
        {
            return true; // run original logic
        }

        SObject.autoLoadFrom = inventory;
        Item? itemThatWillBeLoaded = null;
        foreach (var item in inventory)
        {
            if (__instance.performObjectDropInAction(item, probe: true, who))
            {
                itemThatWillBeLoaded = item;
                break;
            }
        }

        if (itemThatWillBeLoaded is null)
        {
            SObject.autoLoadFrom = null;
            __result = false;
            return false; // run original logic
        }

        var inputTreatmentCategory = treatmentRules.Default;
        var contextTags = itemThatWillBeLoaded.GetContextTags();
        var matchingTag = treatmentRules
            .Overrides
            .Keys
            .LastOrDefault(contextTags.Contains);
        if (!string.IsNullOrEmpty(matchingTag))
        {
            inputTreatmentCategory = treatmentRules.Overrides[matchingTag];
        }

        if (treatmentRules.Overrides.TryGetValue(itemThatWillBeLoaded.QualifiedItemId, out var overrideCategory))
        {
            inputTreatmentCategory = overrideCategory;
        }

        if (inputTreatmentCategory == MachineTreatmentCategory.None)
        {
            return true; // don't run original logic
        }

        // get applied treatments to this machine
        var appliedTreatments = Data.ReadAppliedMachineTreatments(__instance);
        if ((appliedTreatments.CoatingCategory != inputTreatmentCategory || appliedTreatments.CoatingCycles == 0) &&
            inventory.FirstOrDefault(item => item.QualifiedItemId.IsIn(Lookups.TreatmentsByCategory[inputTreatmentCategory])) is { } treatmentItem)
        {
            treatmentItem.ConsumeStack(1);
            appliedTreatments.CoatingCategory = inputTreatmentCategory;
            appliedTreatments.CoatingCycles = 20;
        }

        if (appliedTreatments.OverclockCycles == 0 && inventory.FirstOrDefault(item => item.QualifiedItemId.IsIn(Lookups.TreatmentsByCategory[MachineTreatmentCategory.Overclock])) is { } battery)
        {
            battery.ConsumeStack(1);
            appliedTreatments.OverclockCycles = 30;
        }

        Data.WriteAppliedMachineTreatments(__instance, appliedTreatments);
        return true; // run original logic
    }

    #endregion harmony patches
}
