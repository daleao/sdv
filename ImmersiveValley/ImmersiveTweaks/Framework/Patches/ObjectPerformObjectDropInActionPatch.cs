namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Common.Extensions;
using Common.Harmony;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformObjectDropInActionPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformObjectDropInActionPatch()
    {
        Target = RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    /// <summary>Remember state before action.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static void ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
    }

    /// <summary>Tweaks golden and ostrich egg artisan products + gives flower memory to kegs.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
        bool probe)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
        if (probe || __state || __instance.name != "Mayonnaise Machine" || __instance.heldObject.Value is null || dropInItem is not SObject ||
            !ModEntry.Config.LargeProducsYieldQuantityOverQuality) return;

        // large milk/eggs give double output at normal quality
        if (dropInItem.Name.ContainsAnyOf("Large", "L."))
        {
            __instance.heldObject.Value.Stack = 2;
            __instance.heldObject.Value.Quality = SObject.lowQuality;
        }
        else switch (dropInItem.ParentSheetIndex)
        {
            // ostrich mayonnaise keeps giving x10 output but doesn't respect input quality without Artisan
            case 289 when !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.ostrichmayoForProducerFrameworkMod"):
                __instance.heldObject.Value.Quality = SObject.lowQuality;
                break;
            // golden mayonnaise keeps giving gives single output but keeps golden quality
            case 928 when !ModEntry.ModHelper.ModRegistry.IsLoaded("ughitsmegan.goldenmayoForProducerFrameworkMod"):
                __instance.heldObject.Value.Stack = 1;
                break;
        }
    }

    #endregion harmony patches
}