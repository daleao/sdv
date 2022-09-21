namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Digus.ProducerFrameworkMod")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class ProducerRuleControllerProduceOutputPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ProducerRuleControllerProduceOutputPatch"/> class.</summary>
    internal ProducerRuleControllerProduceOutputPatch()
    {
        this.Target = "ProducerFrameworkMod.Controllers.ProducerRuleController"
            .ToType()
            .RequireMethod("ProduceOutput");
        this.Postfix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Replaces large egg and milk output quality with quantity for PFM machines.</summary>
    [HarmonyPostfix]
    [HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static void ProducerRuleControllerProduceOutputPostfix(
        SObject producer, SObject? input, bool probe)
    {
        if (probe || input?.Category is not (SObject.EggCategory or SObject.MilkCategory) ||
            !input.Name.ContainsAnyOf("Large", "L.") || !ModEntry.Config.LargeProducsYieldQuantityOverQuality ||
            !ModEntry.Config.DairyArtisanMachines.Contains(producer.Name))
        {
            return;
        }

        var output = producer.heldObject.Value;
        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion harmony patches
}
