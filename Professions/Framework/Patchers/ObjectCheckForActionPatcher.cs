namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectCheckForActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ObjectCheckForActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SObject>("CheckForActionOnMachine");
    }

    #region harmony patches

    /// <summary>Patch to remember object state.</summary>
    [HarmonyPrefix]
    private static bool ObjectCheckForActionPrefix(SObject __instance, out bool __state, bool justCheckingForActivity)
    {
        __state = __instance.heldObject.Value is not null && !justCheckingForActivity;
        return true; // run original logic
    }

    /// <summary>Patch to increment Ecologist counter for Mushroom Box + retrieve Prestiged Artisan input.</summary>
    [HarmonyPostfix]
    private static void ObjectCheckForActionPostfix(SObject __instance, bool __state, Farmer who)
    {
        if (!__state || __instance.heldObject.Value is not { } held)
        {
            return;
        }

        if (__instance.QualifiedItemId is not QualifiedBigCraftableIds.MushroomBox ||
            !who.HasProfession(Profession.Ecologist))
        {
            return;
        }

        Data.AppendToEcologistItemsForaged(held.ItemId, who);
        held.Quality = who.GetEcologistForageQuality();
    }

    #endregion harmony patches
}
