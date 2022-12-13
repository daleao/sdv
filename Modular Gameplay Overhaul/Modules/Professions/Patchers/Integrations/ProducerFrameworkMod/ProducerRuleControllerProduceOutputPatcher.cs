namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Digus.ProducerFrameworkMod")]
internal sealed class ProducerRuleControllerProduceOutputPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ProducerRuleControllerProduceOutputPatcher"/> class.</summary>
    internal ProducerRuleControllerProduceOutputPatcher()
    {
        this.Target = "ProducerFrameworkMod.Controllers.ProducerRuleController"
            .ToType()
            .RequireMethod("ProduceOutput");
        this.Postfix!.after = new[] { OverhaulModule.Tweex.Namespace };
    }

    #region harmony patches

    /// <summary>Patch to apply modded Artisan perks to PFM artisan machines.</summary>
    [HarmonyPostfix]
    [HarmonyAfter("Overhaul.Modules.Tweex")]
    private static void ProducerRuleControllerProduceOutputPostfix(
        SObject producer, Farmer who, SObject? input, bool probe)
    {
        if (input is null || probe || !producer.IsArtisanMachine())
        {
            return;
        }

        var output = producer.heldObject.Value;
        if (!output.IsArtisanGood())
        {
            return;
        }

        var user = who;
        if (user.HasProfession(Profession.Artisan))
        {
            output.Quality = input.Quality;
        }

        var owner = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : producer.GetOwner();
        if (!owner.HasProfession(Profession.Artisan))
        {
            return;
        }

        if (owner.HasProfession(Profession.Artisan, true))
        {
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 4;
        }
        else
        {
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 10;
        }

        if (output.Quality < SObject.bestQuality && Game1.random.NextDouble() < 0.05)
        {
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
        }
    }

    #endregion harmony patches
}
