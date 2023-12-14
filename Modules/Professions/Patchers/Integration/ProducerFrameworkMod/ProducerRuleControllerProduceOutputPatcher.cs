namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.ProducerFrameworkMod;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Digus.ProducerFrameworkMod", "Producer Framework Mod")]
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
    [HarmonyAfter("DaLion.Overhaul.Modules.Tweex")]
    private static void ProducerRuleControllerProduceOutputPostfix(
        SObject producer, Farmer who, GameLocation location, SObject? input, bool probe)
    {
        if (input is null || probe || !producer.IsArtisanMachine())
        {
            return;
        }

        var chest = AutomateIntegration.Instance?.GetClosestContainerTo(producer, location);
        if (chest is null)
        {
            return;
        }

        var output = producer.heldObject.Value;
        var user = ProfessionsModule.Config.LaxOwnershipRequirements ? who : chest.GetOwner() ?? Game1.MasterPlayer;
        var r = new Random(Guid.NewGuid().GetHashCode());
        if (user.HasProfession(VanillaProfession.Artisan))
        {
            output.Quality = input.Quality;
            if (!ProfessionsModule.Config.ArtisanGoodsAlwaysInputQuality)
            {
                if (r.NextDouble() > user.FarmingLevel / 30d)
                {
                    output.Quality = (int)((ObjectQuality)output.Quality).Decrement();
                    if (r.NextDouble() > user.FarmingLevel / 15d)
                    {
                        output.Quality = (int)((ObjectQuality)output.Quality).Decrement();
                    }
                }
            }
        }

        var owner = ProfessionsModule.Config.LaxOwnershipRequirements ? Game1.player : producer.GetOwner();
        if (!owner.HasProfession(VanillaProfession.Artisan))
        {
            return;
        }

        if (output.Quality < SObject.bestQuality && r.NextDouble() < 0.05)
        {
            output.Quality += output.Quality == SObject.highQuality ? 2 : 1;
        }

        if (owner.HasProfession(VanillaProfession.Artisan, true))
        {
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 4;
        }
        else
        {
            producer.MinutesUntilReady -= producer.MinutesUntilReady / 10;
        }
    }

    #endregion harmony patches
}
