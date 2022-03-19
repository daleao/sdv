namespace DaLion.Stardew.Tweaks.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class ObjectPerformObjectDropInActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal ObjectPerformObjectDropInActionPatch()
    {
        Original = RequireMethod<SObject>(nameof(SObject.performObjectDropInAction));
    }

    #region harmony patches

    // <summary>Remember state before action.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool ObjectPerformObjectDropInActionPrefix(SObject __instance, ref bool __state)
    {
        __state = __instance.heldObject.Value !=
                  null; // remember whether this machine was already holding an object
        return true; // run original logic
    }

    /// <summary>Tweaks golden and ostrich egg artisan products + gives flower memory to kegs.</summary>
    [HarmonyPostfix]
    private static void ObjectPerformObjectDropInActionPostfix(SObject __instance, bool __state, Item dropInItem,
        bool probe, Farmer who)
    {
        // if there was an object inside before running the original method, or if the machine is still empty after running the original method, then do nothing
        if (__state || __instance.heldObject.Value is null || probe || dropInItem is not SObject dropIn) return;
        
        // kegs remember honey flower type
        if (__instance.name == "Keg" && dropIn.ParentSheetIndex == 340 &&
            dropIn.preservedParentSheetIndex.Value > 0 && ModEntry.Config.KegsRememberHoneyFlower)
        {
            __instance.heldObject.Value.name = dropIn.name.Split(" Honey")[0] + " Mead";
            __instance.heldObject.Value.preservedParentSheetIndex.Value =
                dropIn.preservedParentSheetIndex.Value;
            __instance.heldObject.Value.Price = dropIn.Price * 2;
        }
        // large milk/eggs give double output at normal quality
        else if (dropInItem.Name.ContainsAnyOf("Large", "L.") && ModEntry.Config.LargeProducsYieldQuantityOverQuality)
        {
            __instance.heldObject.Value.Stack = 2;
            __instance.heldObject.Value.Quality = SObject.lowQuality;
        }
        else if (ModEntry.Config.LargeProducsYieldQuantityOverQuality) switch (dropInItem.ParentSheetIndex)
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