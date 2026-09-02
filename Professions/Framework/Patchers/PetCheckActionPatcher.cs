namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Characters;

#endregion using directives

[UsedImplicitly]
internal sealed class PetCheckActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PetCheckActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal PetCheckActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Pet>(nameof(Pet.checkAction));
    }

    #region harmony patches

    /// <summary>Patch to implement pet feeding.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PetCheckActionPostfix(Pet __instance, ref bool __result, Farmer who)
    {
        if (__result || who.ActiveObject is null)
        {
            return;
        }

        if (Data.ReadAs<bool>(__instance, DataKeys.WasSupplementedToday))
        {
            __instance.doEmote(40);
            return;
        }

        var isFavoredFeed = false;
        switch (__instance.petType.Value)
        {
            case "Cat":
                isFavoredFeed = who.ActiveObject.Category == SObject.FishCategory;
                break;
            case "Dog":
                isFavoredFeed = Lookups.CategoryByFeed.TryGetValue(who.ActiveObject.QualifiedItemId, out var category) &&
                    category.IsAnyOf(FeedCategoryRegistry.Fruits, FeedCategoryRegistry.Roots, FeedCategoryRegistry.Gourds);
                break;
        }

        if (!isFavoredFeed)
        {
            __instance.doEmote(36);
            return;
        }

        __instance.mutex.RequestLock(() =>
        {
            __instance.friendshipTowardFarmer.Value = Math.Min(1000, __instance.friendshipTowardFarmer.Value + 12);
            __instance.mutex.ReleaseLock();
        });

        Game1.playSound("give_gift");
        __instance.doEmote(20);
        __instance.playContentSound();
        who.reduceActiveItemByOne();
        Data.Write(__instance, DataKeys.WasSupplementedToday, "true".ToString());
        __result = true;
    }

    #endregion harmony patches
}
