namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using HarmonyLib;
using Shared.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatcher"/> class.</summary>
    internal ObjectPerformObjectDropInActionPatcher()
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Remember state before action.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static void ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject
            .Value is not null; // remember whether this machine was already holding an object
    }

    /// <summary>Tweaks golden and ostrich egg artisan products + gives flower memory to kegs.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(
        SObject __instance, bool __state, Item dropInItem, bool probe)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
        if (!ModEntry.Config.Tweex.LargeProducsYieldQuantityOverQuality || probe || __state ||
            __instance.heldObject.Value is not { } output || dropInItem is not SObject input)
        {
            return;
        }

        // large milk/eggs give double output at normal quality
        if (input.Category is SObject.EggCategory or SObject.MilkCategory && input.Name.ContainsAnyOf("Large", "L."))
        {
            output.Stack = 2;
            output.Quality = SObject.lowQuality;
        }
        else if (__instance.ParentSheetIndex == (int)Machine.MayonnaiseMachine)
        {
            switch (dropInItem.ParentSheetIndex)
            {
                // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
                case Constants.OstrichEggIndex when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                    output.Quality = SObject.lowQuality;
                    break;
                // golden mayonnaise keeps giving gives single output but keeps golden quality
                case Constants.GoldenEggIndex when !ModEntry.ModHelper.ModRegistry.IsLoaded(
                    "ughitsmegan.goldenmayoForProducerFrameworkMod"):
                    output.Stack = 1;
                    break;
            }
        }
    }

    #endregion harmony patches
}
