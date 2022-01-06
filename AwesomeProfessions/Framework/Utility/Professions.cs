using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using TheLion.Stardew.Common.Classes;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

// ReSharper disable PossibleLossOfFraction

namespace TheLion.Stardew.Professions.Framework.Utility;

/// <summary>Holds common methods and properties related to specific professions.</summary>
public static class Professions
{
    #region look-up table

    public static BiMap<string, int> IndexByName { get; } = new()
    {
        // farming
        { "Rancher", Farmer.rancher }, // 0
        { "Breeder", Farmer.butcher }, // 2 (coopmaster)
        { "Producer", Farmer.shepherd }, // 3

        { "Harvester", Farmer.tiller }, // 1
        { "Artisan", Farmer.artisan }, // 4
        { "Agriculturist", Farmer.agriculturist }, // 5

        // fishing
        { "Fisher", Farmer.fisher }, // 6
        { "Angler", Farmer.angler }, // 8
        { "Aquarist", Farmer.pirate }, // 9

        { "Trapper", Farmer.trapper }, // 7
        { "Luremaster", Farmer.baitmaster }, // 10
        { "Conservationist", Farmer.mariner }, // 11
        /// Note: the vanilla game code has mariner and baitmaster IDs mixed up; i.e. effectively mariner is 10 and luremaster is 11.
        /// Since we are completely replacing both professions, we take the opportunity to fix this inconsistency.

        // foraging
        { "Lumberjack", Farmer.forester }, // 12
        { "Arborist", Farmer.lumberjack }, // 14
        { "Tapper", Farmer.tapper }, // 15

        { "Forager", Farmer.gatherer }, // 13
        { "Ecologist", Farmer.botanist }, // 16
        { "Scavenger", Farmer.tracker }, // 17

        // mining
        { "Miner", Farmer.miner }, // 18
        { "Spelunker", Farmer.blacksmith }, // 20
        { "Prospector", Farmer.burrower }, // 21 (prospector)

        { "Blaster", Farmer.geologist }, // 19
        { "Demolitionist", Farmer.excavator }, // 22
        { "Gemologist", Farmer.gemologist }, // 23

        // combat
        { "Fighter", Farmer.fighter }, // 24
        { "Brute", Farmer.brute }, // 26
        { "Poacher", Farmer.defender }, // 27

        { "Rascal", Farmer.scout }, // 25
        { "Piper", Farmer.acrobat }, // 28
        { "Desperado", Farmer.desperado } // 29
    };

    #endregion look-up table

    #region public methods

    /// <summary>Get the index of a given profession by name.</summary>
    /// <param name="professionName">Case-sensitive profession name.</param>
    public static int IndexOf(string professionName)
    {
        if (IndexByName.Forward.TryGetValue(professionName, out var professionIndex)) return professionIndex;
        throw new ArgumentException($"Profession {professionName} does not exist.");
    }

    /// <summary>Get the name of a given profession by index.</summary>
    /// <param name="professionIndex">The index of the profession.</param>
    public static string NameOf(int professionIndex)
    {
        if (IndexByName.Reverse.TryGetValue(professionIndex, out var professionName)) return professionName;
        throw new IndexOutOfRangeException($"Index {professionIndex} is not a valid profession index.");
    }

    /// <summary>Affects the price of produce sold by Producer.</summary>
    /// <param name="who">The player.</param>
    public static float GetProducerPriceBonus(Farmer who)
    {
        return Game1.getFarm().buildings.Where(b =>
            (b.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer) &&
            b.buildingType.Contains("Deluxe") && ((AnimalHouse)b.indoors.Value).isFull()).Sum(_ => 0.05f);
    }

    /// <summary>Affects the price of fish sold by Angler.</summary>
    /// <param name="who">The player.</param>
    public static float GetAnglerPriceBonus(Farmer who)
    {
        var fishData = Game1.content.Load<Dictionary<int, string>>(PathUtilities.NormalizeAssetName("Data/Fish"))
            .Where(p => !p.Key.IsAnyOf(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        var multiplier = 0f;
        foreach (var (key, value) in who.fishCaught.Pairs)
        {
            if (!fishData.TryGetValue(key, out var specificFishData)) continue;

            var dataFields = specificFishData.Split('/');
            if (Objects.LegendaryFishNames.Contains(dataFields[0]))
                multiplier += 0.05f;
            else if (value[1] >= Convert.ToInt32(dataFields[4]))
                multiplier += 0.01f;
        }

        return multiplier;
    }

    /// <summary>Affects the decay of the "catching" bar for Aquarist.</summary>
    public static float GetAquaristBonusCatchingBarSpeed(Farmer who)
    {
        var fishTypes = Game1.getFarm().buildings
            .Where(b => (b.owner.Value == who.UniqueMultiplayerID || !Context.IsMultiplayer) && b is FishPond pond && pond.fishType.Value > 0)
            .Cast<FishPond>()
            .Select(pond => pond.fishType.Value);

        return fishTypes.Distinct().Count() * 0.000165f;
    }

    /// <summary>Affects the price all items sold by Conservationist.</summary>
    public static float GetConservationistPriceMultiplier()
    {
        return 1f + ModData.ReadAs<float>("ActiveTaxBonusPercent");
    }

    /// <summary>Affects the price of animals sold by Breeder.</summary>
    /// <param name="a">Farm animal instance.</param>
    public static double GetProducerAdjustedFriendship(FarmAnimal a)
    {
        return Math.Pow(Math.Sqrt(2) * a.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }

    /// <summary>Affects the quality of items foraged by Ecologist.</summary>
    public static int GetEcologistForageQuality()
    {
        var itemsForaged = ModData.ReadAs<uint>("ItemsForaged");
        return itemsForaged < ModEntry.Config.ForagesNeededForBestQuality
            ? itemsForaged < ModEntry.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Affects the quality of minerals collected by Gemologist.</summary>
    public static int GetGemologistMineralQuality()
    {
        var mineralsCollected = ModData.ReadAs<uint>("MineralsCollected");
        return mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ModEntry.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Affects that chance that a ladder or shaft will spawn for Spelunker.</summary>
    public static double GetSpelunkerBonusLadderDownChance()
    {
        return ModEntry.State.Value.SpelunkerLadderStreak * 0.005;
    }

    /// <summary>Affects the raw damage dealt by Brute.</summary>
    /// <param name="who">The player.</param>
    public static float GetBruteBonusDamageMultiplier(Farmer who)
    {
        return 1.15f +
               (who.IsLocalPlayer && ModEntry.State.Value.IsSuperModeActive && ModEntry.State.Value.SuperModeIndex == IndexOf("Brute")
                   ? (who.HasPrestigedProfession("Fighter") ? 0.2f : 0.1f) + 0.15f + who.attackIncreaseModifier + // double fighter, brute and ring bonuses
                     (who.CurrentTool is not null
                         ? who.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f // double enchants
                         : 0f)
                     + ModEntry.State.Value.SuperModeGaugeMaxValue / 10 * 0.005f // apply the maximum fury bonus
                   : ModEntry.State.Value.SuperModeGaugeValue / 10 * 0.005f);
    }

    /// <summary>Affects the cooldown of special moves performed by prestiged Brute.</summary>
    /// <param name="who">The player.</param>
    public static float GetPrestigedBruteCooldownReduction(Farmer who)
    {
        return 1f - who.attackIncreaseModifier + (who.CurrentTool is not null
            ? who.CurrentTool.GetEnchantmentLevel<RubyEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>Affecsts the power of critical strikes performed by Poacher.</summary>
    public static float GetPoacherCritDamageMultiplier()
    {
        return ModEntry.State.Value.IsSuperModeActive
            ? 1f + ModEntry.State.Value.SuperModeGaugeMaxValue / 10 * 0.04f // apply the maximum cold blood bonus
            : 1f + ModEntry.State.Value.SuperModeGaugeValue / 10 * 0.04f;
    }

    /// <summary>Affects the cooldown special moves performed by prestiged Poacher.</summary>
    /// <param name="who">The player.</param>
    public static float GetPrestigedPoacherCooldownReduction(Farmer who)
    {
        return 1f - who.critChanceModifier + who.critPowerModifier + (who.CurrentTool is not null
            ? who.CurrentTool.GetEnchantmentLevel<AquamarineEnchantment>() +
              who.CurrentTool.GetEnchantmentLevel<JadeEnchantment>() * 0.1f
            : 0f);
    }

    /// <summary>Affects the damage of projectiles fired by Rascal.</summary>
    /// <param name="travelTime">Projectile's travel time.</param>
    public static float GetRascalBonusDamageForTravelTime(int travelTime)
    {
        const int MAX_TRAVEL_TIME_I = 800;
        if (travelTime > MAX_TRAVEL_TIME_I) return 1.5f;
        return 1f + 0.5f / MAX_TRAVEL_TIME_I * travelTime;
    }

    /// <summary>Affects the chance to shoot twice consecutively for Desperado.</summary>
    /// <param name="who">The player.</param>
    public static float GetDesperadoDoubleStrafeChance(Farmer who)
    {
        var healthPercent = (double)who.health / who.maxHealth;
        return (float)Math.Min(2 / (healthPercent + 1.5) - 0.75, 0.5f);
    }

    /// <summary>Affects projectile velocity, knockback, hitbox size and pierce chance for Desperado.</summary>
    public static float GetDesperadoBulletPower()
    {
        return ModEntry.State.Value.IsSuperModeActive
            ? 1f
            : 1f + ModEntry.State.Value.SuperModeGaugeValue / 10 * 0.01f;
    }

    /// <summary>Affects the maximum number of bonus Slimes that can be attracted by Piper.</summary>
    public static int GetPiperSlimeSpawnAttempts()
    {
        return ModEntry.State.Value.IsSuperModeActive
            ? ModEntry.State.Value.SuperModeGaugeMaxValue / 50 + 1
            : ModEntry.State.Value.SuperModeGaugeValue / 50 + 1;
    }

    /// <summary>Get the localized pronoun for the currently registered Super Mode buff.</summary>
    public static string GetBuffPronoun()
    {
        switch (LocalizedContentManager.CurrentLanguageCode)
        {
            case LocalizedContentManager.LanguageCode.es:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite.female");

            case LocalizedContentManager.LanguageCode.fr:
            case LocalizedContentManager.LanguageCode.pt:
                return ModEntry.ModHelper.Translation.Get("pronoun.definite" +
                                                          (ModEntry.State.Value.SuperModeIndex == IndexOf("Poacher")
                                                              ? ".male"
                                                              : ".female"));

            default:
                return string.Empty;
        }
    }

    #endregion public methods
}