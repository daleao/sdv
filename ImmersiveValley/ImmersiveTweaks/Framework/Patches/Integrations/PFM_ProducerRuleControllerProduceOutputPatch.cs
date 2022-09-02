namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Attributes;
using Common.Extensions;
using Common.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

[UsedImplicitly, RequiresMod("Digus.ProducerFrameworkMod")]
internal sealed class ProducerRuleControllerProduceOutputPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ProducerRuleControllerProduceOutputPatch()
    {
        Target = "ProducerFrameworkMod.Controllers.ProducerRuleController".ToType().RequireMethod("ProduceOutput");
        Postfix!.before = new[] { "DaLion.ImmersiveProfessions" };
    }

    #region harmony patches

    /// <summary>Replaces large egg and milk output quality with quantity for PFM machines.</summary>
    [HarmonyPostfix, HarmonyBefore("DaLion.ImmersiveProfessions")]
    private static void ProducerRuleControllerProduceOutputPostfix(SObject producer, SObject? input,
        bool probe)
    {
        if (probe || input is null || input.Category is not (SObject.EggCategory or SObject.MilkCategory) ||
            !input.Name.ContainsAnyOf("Large", "L.") || !ModEntry.Config.LargeProducsYieldQuantityOverQuality ||
            !ModEntry.Config.DairyArtisanMachines.Contains(producer.Name)) return;

        var output = producer.heldObject.Value;
        output.Stack = 2;
        output.Quality = SObject.lowQuality;
    }

    #endregion harmony patches
}