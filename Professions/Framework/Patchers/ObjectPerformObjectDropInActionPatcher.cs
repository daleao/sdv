namespace DaLion.Professions.Framework.Patchers;

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
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, out bool __state, bool probe)
    {
        __state = __instance.heldObject.Value !=
                  null && !probe; // remember whether this machine was already holding an object
        return true; // run original logic
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

        // artisan-owned machines calibrate to repeated ingredients
        if (!owner.HasProfessionOrLax(Profession.Artisan))
        {
            return;
        }

        var lastProcessedData = Data.Read(__instance, DataKeys.LastIngredientProcessed);
        var lastProcessed = lastProcessedData.Split(',');
        if (lastProcessed.Length != 2)
        {
            Log.W($"Badly formatted ingredient data in machine {__instance.Name}: {lastProcessedData}");
            Data.Write(__instance, DataKeys.LastIngredientProcessed, null);
            return;
        }

        var lastProcessedId = lastProcessed[0];
        var lastProcessedTimes = int.Parse(lastProcessed[1]);
        if (lastProcessedId != input.ItemId)
        {
            Data.Write(__instance, DataKeys.LastIngredientProcessed, $"{input.ItemId},1");
            return;
        }

        var bonus = Math.Min(0.025 * lastProcessedTimes, 0.25);
        if (__instance is Cask cask)
        {
            cask.daysToMature.Value -= (int)Math.Floor(cask.daysToMature.Value * bonus);
        }
        else
        {
            __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady * bonus);
        }

        Data.Write(__instance, DataKeys.LastIngredientProcessed, $"{input.ItemId},{lastProcessedTimes + 1}");

        if (!owner.HasProfession(Profession.Artisan, true))
        {
            return;
        }

        // machinist machine treatments
        var before = Data.Read(__instance, DataKeys.MachinePowerups);
        if (string.IsNullOrEmpty(before))
        {
            return;
        }

        var powerups = before.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var after = string.Empty;
        foreach (var powerup in powerups)
        {
            var split = powerup.Split(',');
            var itemId = split[0];
            if (!int.TryParse(split[1], out var cycles))
            {
                Log.W($"Badly formatted data for cycles of {itemId}.");
                continue;
            }

            MachineTreatmentCategory? category = MachineTreatmentCategory.None;
            if (Lookups.MachineTreatments.TryGetValue(__instance.ItemId, out var treatmentRules))
            {
                category = treatmentRules.Default;
                var contextTags = input.GetContextTags();
                var matchingKey = treatmentRules
                    .Overrides
                    .Keys
                    .FirstOrDefault(contextTags.Contains);
                if (!string.IsNullOrEmpty(matchingKey))
                {
                    category = treatmentRules.Overrides[matchingKey];
                    break;
                }
            }

            switch (itemId)
            {
                case QIDs.OakResin when category == MachineTreatmentCategory.Fermentation:
                case QIDs.MapleSyrup when category == MachineTreatmentCategory.Glazing:
                case QIDs.PineTar when category == MachineTreatmentCategory.Sealing:
                case "(BC)FlashShifter.StardewValleyExpandedCP_Birch_Water" when category == MachineTreatmentCategory.Glazing:
                case "(BC)FlashShifter.StardewValleyExpandedCP_Fir_Wax" when category == MachineTreatmentCategory.Sealing:
                    if (output.Quality < SObject.bestQuality)
                    {
                        output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
                    }

                    break;
                case QIDs.BatteryPack:
                    __instance.MinutesUntilReady -= (int)Math.Floor(__instance.MinutesUntilReady / 2d);
                    break;
            }

            if (--cycles > 0)
            {
                after += $"/{itemId},{cycles}";
            }
        }

        Data.Write(__instance, DataKeys.MachinePowerups, after);
    }

    #endregion harmony patches
}
