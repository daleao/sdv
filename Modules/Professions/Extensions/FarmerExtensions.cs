// ReSharper disable PossibleLossOfFraction
namespace DaLion.Overhaul.Modules.Professions.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Events.Player.Warped;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.Buildings;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    /// <summary>Determines whether the <paramref name="farmer"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasProfession(this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        if (prestiged && !(profession is Profession ||
                           (profession is SCProfession custom && ((SCSkill)custom.Skill).CanPrestige)))
        {
            return false;
        }

        return farmer.professions.Contains(profession.Id + (prestiged ? 100 : 0));
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has acquired both level ten professions branching from the
    ///     specified level five <paramref name="profession"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has both tier-two professions which branch from <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasAllProfessionsBranchingFrom(this Farmer farmer, IProfession profession)
    {
        return profession.BranchingProfessions.All(branch => farmer.professions.Contains(branch.Id));
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has all six professions in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has all four professions belonging to the <paramref name="skill"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasAllProfessionsInSkill(this Farmer farmer, ISkill skill)
    {
        return skill.ProfessionIds.All(farmer.professions.Contains);
    }

    /// <summary>Determines whether the <paramref name="farmer"/> has all available professions (vanilla + modded).</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="includeCustom">Whether to include <see cref="SCProfession"/>s in the count.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has all 30 vanilla professions, otherwise <see langword="false"/>.</returns>
    internal static bool HasAllProfessions(this Farmer farmer, bool includeCustom = false)
    {
        var allProfessions = Enumerable.Range(0, 30);
        if (!allProfessions.All(farmer.professions.Contains))
        {
            return false;
        }

        if (includeCustom && !SCProfession.List
                .Select(p => p.Id)
                .All(farmer.professions.Contains))
        {
            return false;
        }

        return true;
    }

    /// <summary>Determines whether this or, if allowed by the module's settings, any <see cref="Farmer"/> instance in the current game session has the specified <paramref name="profession"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if either <paramref name="farmer"/> has the specified <paramref name="profession"/>, or <see cref="Config.LaxOwnershipRequirements"/> is enabled and at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasProfessionOrLax(
        this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        return farmer.HasProfession(profession, prestiged) ||
               (ProfessionsModule.Config.LaxOwnershipRequirements &&
                Game1.game1.DoesAnyPlayerHaveProfession(profession, out _, prestiged));
    }

    /// <summary>
    ///     Gets the most recent tier-one profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    internal static int GetCurrentBranchForSkill(this Farmer farmer, ISkill skill)
    {
        var branch = farmer.professions
            .Intersect(skill.TierOneProfessionIds.Concat(skill.TierOneProfessionIds.Select(id => id + 100)))
            .DefaultIfEmpty(-1)
            .Last();

        return farmer.professions.Contains(branch - 100) ? branch - 100 : branch;
    }

    /// <summary>
    ///     Gets the most recent tier-two profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="branch"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="branch">The branch (level 5 <see cref="IProfession"/>) to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    internal static int GetCurrentProfessionForBranch(this Farmer farmer, IProfession branch)
    {
        var current = farmer.professions
            .Intersect(branch.BranchingProfessions
                .Select(p => p.Id)
                .Concat(branch.BranchingProfessions
                    .Select(p => p.Id + 100)))
            .DefaultIfEmpty(-1)
            .Last();

        var allPrestiged = Profession.GetRange(true).Concat(SCProfession.GetAllIds(true)).ToHashSet();
        return allPrestiged.Contains(current) ? current - 100 : current;
    }

    /// <summary>Gets all the <paramref name="farmer"/>'s professions associated with a specific <paramref name="skill"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the result.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="IProfession"/>s in <paramref name="skill"/>.</returns>
    internal static IProfession[] GetProfessionsForSkill(
        this Farmer farmer, ISkill skill, bool excludeTierOneProfessions = false)
    {
        return farmer.professions
            .Intersect(excludeTierOneProfessions ? skill.TierTwoProfessionIds : skill.ProfessionIds)
            .Select<int, IProfession>(id =>
                SCSkill.Loaded.ContainsKey(skill.StringId)
                    ? SCProfession.Loaded[id]
                    : Profession.FromValue(id)).ToArray();
    }

    /// <summary>
    ///     Gets the professions which the <paramref name="farmer"/> is missing in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <param name="excludeTierOneProfessions">Whether to exclude level 5 professions from the count.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of un-obtained <see cref="IProfession"/>s.</returns>
    internal static IProfession[] GetMissingProfessionsInSkill(
        this Farmer farmer, ISkill skill, bool excludeTierOneProfessions = false)
    {
        return excludeTierOneProfessions
            ? skill.Professions.Where(p => p.Level == 10 && !farmer.professions.Contains(p.Id)).ToArray()
            : skill.Professions.Where(p => !farmer.professions.Contains(p.Id)).ToArray();
    }

    /// <summary>
    ///     Gets the last acquired profession by the <paramref name="farmer"/> in the specified subset, or simply the
    ///     last acquired profession if no subset is specified.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="subset">An array of profession ids.</param>
    /// <returns>The last acquired profession, or -1 if none was found.</returns>
    internal static int GetMostRecentProfession(this Farmer farmer, IEnumerable<int>? subset = null)
    {
        return subset is null
            ? farmer.professions[^1]
            : farmer.professions.Intersect(subset).DefaultIfEmpty(-1).Last();
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/>'s registered <see cref="IUltimate"/> is valid, or whether they
    ///     should be assigned one based on their professions.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void RevalidateUltimate(this Farmer farmer)
    {
        var currentIndex = farmer.Read(DataKeys.UltimateIndex, -1);
        if (currentIndex > 0 && !ProfessionsModule.Config.EnableLimitBreaks)
        {
            Log.W(
                $"[PRFS]: {farmer.Name} has non-null Limit Break but Limit Breaks are not enabled. The registered Limit Break will be reset.");
            farmer.Write(DataKeys.UltimateIndex, null);
            return;
        }

        var newIndex = currentIndex;
        switch (currentIndex)
        {
            case < 0 when farmer.professions.Any(p => p is >= 26 and < 30):
            {
                Log.W(
                    $"[PRFS]: {farmer.Name} is eligible for a Limit Break but is not currently registered to any. The registered Limit Break will be set to a default value.");
                newIndex = farmer.professions.First(p => p is >= 26 and < 30);
                break;
            }

            case >= 0 when !farmer.professions.Contains(currentIndex):
            {
                Log.W($"[PRFS]: {farmer.Name} is registered to Limit Break index {currentIndex} but is missing the corresponding profession. The registered Limit Break will be reset.");
                newIndex = farmer.professions.Any(p => p is >= 26 and < 30)
                    ? farmer.professions.First(p => p is >= 26 and < 30)
                    : -1;
                break;
            }
        }

        if (newIndex != currentIndex)
        {
            farmer.Write(DataKeys.UltimateIndex, newIndex.ToString());
            currentIndex = newIndex;
        }

        if (currentIndex > 0)
        {
            EventManager.Enable<UltimateWarpedEvent>();
        }
    }

    /// <summary>Gets the <see cref="Ultimate"/> options which are not currently registered.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Ultimate"/>s, excluding the currently registered.</returns>
    internal static IEnumerable<Ultimate> GetUnchosenUltimates(this Farmer farmer)
    {
        var chosen = farmer.Get_Ultimate();
        return new[] { 26, 27, 28, 29 }
            .Intersect(farmer.professions)
            .Except(chosen?.Value.Collect() ?? Enumerable.Empty<int>())
            .Select(Ultimate.FromValue);
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has caught the fish with specified <paramref name="index"/>
    ///     at max size.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="index">The fish's index.</param>
    /// <returns><see langword="true"/> if the fish with the specified <paramref name="index"/> has been caught with max size, otherwise <see langword="false"/>.</returns>
    internal static bool HasCaughtMaxSized(this Farmer farmer, int index)
    {
        if (!farmer.fishCaught.ContainsKey(index) || farmer.fishCaught[index][1] <= 0)
        {
            return false;
        }

        var fishData = Game1.content
            .Load<Dictionary<int, string>>("Data\\Fish")
            .Where(p => !p.Key.IsIn(152, 153, 157) && !p.Value.Contains("trap"))
            .ToDictionary(p => p.Key, p => p.Value);

        if (!fishData.TryGetValue(index, out var specificFishData))
        {
            return false;
        }

        var dataFields = specificFishData.SplitWithoutAllocation('/');
        return farmer.fishCaught[index][1] >= int.Parse(dataFields[4]);
    }

    /// <summary>Gets the price bonus applied to animal produce sold by <see cref="Profession.Producer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    internal static float GetProducerPriceBonus(this Farmer farmer)
    {
        var sum = 0f;
        var buildings = Game1.getFarm().buildings;
        for (var i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if ((building.IsOwnedBy(farmer) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                !building.isUnderConstruction() && building.buildingType.Contains("Deluxe") &&
                building.indoors.Value is AnimalHouse house && house.isFull())
            {
                sum += 0.05f;
            }
        }

        return sum;
    }

    /// <summary>Gets the price bonus applied to fish sold by <see cref="Profession.Angler"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    internal static float GetAnglerPriceBonus(this Farmer farmer)
    {
        var fishData = Game1.content
            .Load<Dictionary<int, string>>("Data\\Fish");
        var bonus = 0f;
        var isPrestiged = farmer.HasProfession(Profession.Angler, true);
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (key.IsAlgaeIndex() || !fishData.TryGetValue(key, out var specificFishData))
            {
                continue;
            }

            var dataFields = specificFishData.SplitWithoutAllocation('/');
            if (dataFields[2] == "trap" || !int.TryParse(dataFields[4], out var maxSize))
            {
                continue;
            }

            if (Sets.LegendaryFishes.Contains(dataFields[0].ToString()))
            {
                bonus += 0.025f;
            }
            else if (value[1] >= maxSize && isPrestiged)
            {
                bonus += 0.01f;
            }
            else
            {
                bonus += 0.005f;
            }
        }

        return Math.Min(bonus, ProfessionsModule.Config.AnglerPriceBonusCeiling);
    }

    /// <summary>Gets the bonus "catching" bar build rate for <see cref="Profession.Aquarist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> catching height.</returns>
    internal static float GetAquaristCatchingHandicap(this Farmer farmer)
    {
        HashSet<int> fishTypes = new();
        var buildings = Game1.getFarm().buildings;
        for (var i = 0; i < buildings.Count; i++)
        {
            var building = buildings[i];
            if (building is FishPond pond &&
                (pond.IsOwnedBy(Game1.player) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                !pond.isUnderConstruction() && pond.fishType.Value > 0)
            {
                fishTypes.Add(pond.fishType.Value);
            }
        }

        return Math.Min(Math.Max(fishTypes.Count, ProfessionsModule.Config.AquaristFishPondCeiling) * 0.000165f, 0.002f);
    }

    /// <summary>Gets the price bonus applied to all items sold by <see cref="Profession.Conservationist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for general items.</returns>
    internal static float GetConservationistPriceMultiplier(this Farmer farmer)
    {
        return 1f + farmer.Read<float>(DataKeys.ConservationistActiveTaxDeduction);
    }

    /// <summary>Gets the quality of items foraged by <see cref="Profession.Ecologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    internal static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = farmer.Read<uint>(DataKeys.EcologistItemsForaged);
        return itemsForaged < ProfessionsModule.Config.ForagesNeededForBestQuality
            ? itemsForaged < ProfessionsModule.Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Gets the quality of minerals collected by <see cref="Profession.Gemologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    internal static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = farmer.Read<uint>(DataKeys.GemologistMineralsCollected);
        return mineralsCollected < ProfessionsModule.Config.MineralsNeededForBestQuality
            ? mineralsCollected < ProfessionsModule.Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Applies <see cref="Profession.Spelunker"/> effects following interaction with a ladder or sink hole.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void AddSpelunkerMomentum(this Farmer farmer)
    {
        ProfessionsModule.State.SpelunkerLadderStreak++;
        EventManager.Enable<SpelunkerUpdateTickedEvent>();
        if (!farmer.HasProfession(Profession.Spelunker, true))
        {
            return;
        }

        farmer.health = Math.Min(farmer.health + (int)(farmer.maxHealth * 0.05f), farmer.maxHealth);
        farmer.Stamina = Math.Min(farmer.Stamina + (farmer.MaxStamina * 0.025f), farmer.MaxStamina);
    }

    /// <summary>Enumerates the <see cref="GreenSlime"/>s currently inhabiting owned <see cref="SlimeHutch"/>es.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="GreenSlime"/>s currently inhabiting owned <see cref="SlimeHutch"/>es.</returns>
    internal static IEnumerable<GreenSlime> GetRaisedSlimes(this Farmer farmer)
    {
        return Game1.getFarm().buildings
            .Where(b => (b.IsOwnedBy(farmer) || ProfessionsModule.Config.LaxOwnershipRequirements) &&
                        b.indoors.Value is SlimeHutch && !b.isUnderConstruction())
            .SelectMany(b => b.indoors.Value.characters.OfType<GreenSlime>());
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> is currently using the <see cref="Profession.Poacher"/>
    ///     <see cref="Ultimate"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if <see cref="Ultimate.PoacherAmbush"/> is active.</returns>
    internal static bool IsInAmbush(this Farmer farmer)
    {
        return farmer.Get_Ultimate() is Ambush { IsActive: true };
    }
}
