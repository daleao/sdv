namespace DaLion.Redux.Tweex.Patches;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("Digus.ProducerFrameworkMod")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class ProducerRuleControllerProduceOutputPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProducerRuleControllerProduceOutputPatch"/> class.</summary>
    internal ProducerRuleControllerProduceOutputPatch()
    {
        this.Target = "ProducerFrameworkMod.Controllers.ProducerRuleController"
            .ToType()
            .RequireMethod("ProduceOutput");
        this.Postfix!.before = new[] { ReduxModule.Professions.Name };
    }

    #region harmony patches

    /// <summary>Replaces large egg and milk output quality with quantity for PFM machines.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.Redux.Professions")]
    private static void ProducerRuleControllerProduceOutputPostfix(
        SObject producer, SObject? input, bool probe)
    {
        if (probe || input?.Category is not (SObject.EggCategory or SObject.MilkCategory) ||
            !input.Name.ContainsAnyOf("Large", "L.") || !ModEntry.Config.Tweex.LargeProducsYieldQuantityOverQuality ||
            !ModEntry.Config.Tweex.DairyArtisanMachines.Contains(producer.Name))
        {
            return;
        }

        var output = producer.heldObject.Value;
        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion harmony patches
}
