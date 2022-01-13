using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

[UsedImplicitly]
internal class ProducerRuleControllerProduceOutputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ProducerRuleControllerProduceOutputPatch()
    {
        try
        {
            Original = "ProducerFrameworkMod.Controllers.ProducerRuleController".ToType()
                .MethodNamed("ProduceOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply modded Artisan perks to PFM artisan machines.</summary>
    [HarmonyPostfix]
    private static void ProducerRuleControllerProduceOutputPostfix(SObject producer, Farmer who, SObject input,
        bool probe)
    {
        if (input is null || probe || !producer.IsArtisanMachine()) return;

        var output = producer.heldObject.Value;
        if (!output.IsArtisanGood()) return;

        if (Context.IsMultiplayer && producer.owner.Value != who.UniqueMultiplayerID ||
            !who.HasProfession("Artisan"))
        {
            output.Quality = SObject.lowQuality;
            return;
        }

        output.Quality = input.Quality;
        if (output.Quality < SObject.bestQuality &&
            new Random(Guid.NewGuid().GetHashCode()).NextDouble() < 0.05)
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;

        if (who.HasPrestigedProfession("Artisan"))
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 4;
        else
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 10;
    }

    #endregion harmony patches
}