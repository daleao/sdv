namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Extensions;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotCheckForActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotCheckForActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CrabPotCheckForActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.checkForAction));
    }

    #region harmony patches

    /// <summary>Prevents remote item pickup when harvested by Hopper.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    private static bool CrabPotCheckForActionPrefix(
        ref CrabPot __instance,
        ref bool __result,
        ref int ___ignoreRemovalTimer,
        ref bool ___lidFlapping,
        ref float ___lidFlapTimer,
        ref Vector2 ___shake,
        ref float ___shakeTimer,
        Farmer who,
        bool justCheckingForActivity)
    {
        if (__instance.Location is not { } location || __instance.tileIndexToShow != 714 ||
            justCheckingForActivity || __instance.heldObject.Value is not { } held)
        {
            return true; // run original logic
        }

        var tileAbove = new Vector2(__instance.TileLocation.X, __instance.TileLocation.Y - 1f);
        if (location.Objects.TryGetValue(tileAbove, out var objAbove) != true ||
            objAbove is not Chest { SpecialChestType: Chest.SpecialChestTypes.AutoLoader } hopper)
        {
            return true; // run original logic
        }

        // this should always be false
        if (hopper.GetOwner() != who)
        {
            return true; // run original logic
        }

        var item = ItemRegistry.Create(held.QualifiedItemId, held.Stack, held.Quality);
        if (item is SObject && who.stats.Get("Book_Crabbing") != 0)
        {
            var r = Utility.CreateDaySaveRandom(
                Game1.uniqueIDForThisGame,
                Game1.stats.DaysPlayed * 77,
                (__instance.TileLocation.X * 777f) + __instance.TileLocation.Y);
            if (r.NextBool(0.25))
            {
                item.Stack++;
            }
        }

        if (hopper.addItem(item) is not null)
        {
            __result = false;
            return false; // don't run original logic
        }

        if (DataLoader.Fish(Game1.content).TryGetValue(held.ItemId, out var rawDataStr))
        {
            var rawData = rawDataStr.Split('/');
            var minFishSize = rawData.Length <= 5 ? 1 : Convert.ToInt32(rawData[5]);
            var maxFishSize = rawData.Length > 5 ? Convert.ToInt32(rawData[6]) : 10;
            who.caughtFish(held.QualifiedItemId, Game1.random.Next(minFishSize, maxFishSize + 1), from_fish_pond: false, held.Stack);
        }

        who.gainExperience(1, 5);
        __instance.bait.Value = null;
        __instance.heldObject.Value = null;
        __instance.readyForHarvest.Value = false;
        __instance.tileIndexToShow = 710;

        if (hopper.Items.FirstOrDefault(i => i?.Category == SObject.baitCategory) is SObject bait &&
            __instance.performObjectDropInAction(bait, false, who))
        {
            hopper.Items.ReduceId(bait.QualifiedItemId, 1);
        }

        if (!location.farmers.Any())
        {
            __result = false;
            return false; // don't run original logic
        }

        ___lidFlapping = true;
        ___lidFlapTimer = 60f;
        location.playSound("fishingRodBend");
        DelayedAction.playSoundAfterDelay("coin", 500, location);
        ___shake = Vector2.Zero;
        ___shakeTimer = 0f;
        ___ignoreRemovalTimer = 750;
        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
