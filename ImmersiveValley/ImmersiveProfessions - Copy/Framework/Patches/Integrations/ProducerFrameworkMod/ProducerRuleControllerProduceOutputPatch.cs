namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.ProducerFrameworkMod;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly, RequiresMod("Digus.ProducerFrameworkMod")]
internal sealed class ProducerRuleControllerProduceOutputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ProducerRuleControllerProduceOutputPatch()
    {
        Target = "ProducerFrameworkMod.Controllers.ProducerRuleController".ToType().RequireMethod("ProduceOutput");
    }

    #region harmony patches

    /// <summary>Patch to apply modded Artisan perks to PFM artisan machines.</summary>
    [HarmonyPostfix]
    private static void ProducerRuleControllerProduceOutputPostfix(SObject producer, Farmer who, SObject? input,
        bool probe)
    {
        if (input is null || probe || !producer.IsArtisanMachine()) return;

        var output = producer.heldObject.Value;
        if (!output.IsArtisanGood()) return;

        var user = who;
        if (user.HasProfession(Profession.Artisan)) output.Quality = input.Quality;

        var owner = ModEntry.Config.LaxOwnershipRequirements ? Game1.player : producer.GetOwner();
        if (!owner.HasProfession(Profession.Artisan)) return;

        if (owner.HasProfession(Profession.Artisan, true))
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 4;
        else
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 10;

        if (output.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
    }

    #endregion harmony patches
}