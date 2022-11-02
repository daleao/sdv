namespace DaLion.Redux.Framework.Tweex.Patches;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformObjectDropInActionPatch"/> class.</summary>
    internal ObjectPerformObjectDropInActionPatch()
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
        if (probe || __state || dropInItem is not SObject input ||
            __instance.heldObject.Value is not { } output)
        {
            return;
        }

        // large milk/eggs give double output at normal quality
        switch (__instance.ParentSheetIndex)
        {
            case (int)Machine.Keg when input.ParentSheetIndex == Constants.HoneyIndex && input.preservedParentSheetIndex.Value > 0 &&
                              ModEntry.Config.Tweex.KegsRememberHoneyFlower:
                output.name = input.name.Split(" Honey")[0] + " Mead";
                output.honeyType.Value = (SObject.HoneyType)input.preservedParentSheetIndex.Value;
                output.preservedParentSheetIndex.Value =
                    input.preservedParentSheetIndex.Value;
                output.Price = input.Price * 2;
                break;
            case (int)Machine.CheesePress:
            case (int)Machine.MayonnaiseMachine:
                if (!ModEntry.Config.Tweex.LargeProducsYieldQuantityOverQuality)
                {
                    break;
                }

                if (input.Name.ContainsAnyOf("Large", "L."))
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

                break;
        }
    }

    #endregion harmony patches
}
