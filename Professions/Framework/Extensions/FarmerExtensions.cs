namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Professions.Framework.Buffs;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
internal static class FarmerExtensions
{
    private static readonly Func<Vector2, GameLocation, bool> IsMineShaftTileSpawnable = (tile, location) =>
        location.isTileOnMap(tile) && !location.IsTileBlockedBy(tile, PipedSlime.COLLISION_MASK) &&
        (location as MineShaft)!.isTileOnClearAndSolidGround(tile);

    private static readonly Func<Vector2, GameLocation, bool> IsVolcanoTileSpawnable = (tile, location) =>
        location.isTileOnMap(tile) && !location.IsTileBlockedBy(tile, PipedSlime.COLLISION_MASK) &&
        (location as VolcanoDungeon)!.isTileOnClearAndSolidGround(tile);

    private static readonly Func<Vector2, GameLocation, bool> IsGenericTileSpawnable = (tile, location) =>
        location.isTileOnMap(tile) && !location.IsTileBlockedBy(tile, PipedSlime.COLLISION_MASK);

    /// <summary>Determines whether the <paramref name="farmer"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <paramref name="farmer"/> has the specified <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasProfession(this Farmer farmer, IProfession profession, bool prestiged = false)
    {
#if RELEASE
        if (prestiged && !profession.ParentSkill.CanGainPrestigeLevels())
        {
            return false;
        }
#endif
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
        return profession.GetBranchingProfessions.All(branch => farmer.professions.Contains(branch.Id));
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
    /// <param name="includeCustom">Whether to include <see cref="CustomProfession"/>s in the count.</param>
    /// <returns><see langword="true"/> only if the <paramref name="farmer"/> has all 30 vanilla professions, otherwise <see langword="false"/>.</returns>
    internal static bool HasAllProfessions(this Farmer farmer, bool includeCustom = false)
    {
        var allProfessions = Enumerable.Range(0, 30);
        if (!allProfessions.All(farmer.professions.Contains))
        {
            return false;
        }

        return !includeCustom || CustomProfession.List
            .Select(p => p.Id)
            .All(farmer.professions.Contains);
    }

    /// <summary>Determines whether this---or any <see cref="Farmer"/> instance in the game session, if allowed by the module's settings---has the specified <paramref name="profession"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="profession">The <see cref="IProfession"/> to check.</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if either <paramref name="farmer"/> has the specified <paramref name="profession"/>, or <see cref="ProfessionsConfig.LaxOwnershipRequirements"/> is enabled and at least one player in the game session has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasProfessionOrLax(
        this Farmer farmer, IProfession profession, bool prestiged = false)
    {
        return farmer.HasProfession(profession, prestiged) ||
               (Config.LaxOwnershipRequirements &&
                Game1.game1.DoesAnyPlayerHaveProfession(profession, prestiged: prestiged));
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
        var orderedProfessions = !farmer.IsLocalPlayer
            ? Data.Read(farmer, DataKeys.OrderedProfessions).ParseList<int>()
            : State.OrderedProfessions;

        var professions = orderedProfessions;
        return subset is null
            ? professions[^1]
            : professions.Intersect(subset).DefaultIfEmpty(-1).Last();
    }

    /// <summary>
    ///     Gets the most recent tier-one profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="skill"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="skill">The <see cref="ISkill"/> to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    internal static int GetCurrentRootProfessionForSkill(this Farmer farmer, ISkill skill)
    {
        var orderedProfessions = !farmer.IsLocalPlayer
            ? Data.Read(farmer, DataKeys.OrderedProfessions).ParseList<int>()
            : State.OrderedProfessions;

        var professions = orderedProfessions;
        var roots = skill.TierOneProfessionIds.ToHashSet();
        for (var i = professions.Count - 1; i >= 0; i--)
        {
            var profession = professions[i];
            if (roots.Contains(profession))
            {
                return profession;
            }

            if (roots.Contains(profession - 100))
            {
                return profession - 100;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Gets the most recent tier-two profession acquired by the <paramref name="farmer"/> in the specified
    ///     <paramref name="root"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="root">The root (level 5 <see cref="IProfession"/>) to check.</param>
    /// <returns>The last acquired profession index, or -1 if none was found.</returns>
    internal static int GetCurrentBranchingProfessionForRoot(this Farmer farmer, IProfession root)
    {
        var orderedProfessions = !farmer.IsLocalPlayer
            ? Data.Read(farmer, DataKeys.OrderedProfessions).ParseList<int>()
            : State.OrderedProfessions;

        var professions = orderedProfessions;
        var branches = root.GetBranchingProfessions
            .Select(p => p.Id)
            .ToHashSet();
        for (var i = professions.Count - 1; i >= 0; i--)
        {
            var profession = professions[i];
            if (branches.Contains(profession))
            {
                return profession;
            }

            if (branches.Contains(profession - 100))
            {
                return profession - 100;
            }
        }

        return -1;
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
            .Select<int, IProfession?>(id =>
                Profession.TryFromValue(id, out var vanilla)
                    ? vanilla
                    : CustomProfession.Loaded.GetValueOrDefault(id))
            .WhereNotNull()
            .ToArray();
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> has caught the fish with specified <paramref name="fishId"/>
    ///     at max size.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="fishId">The fish's id.</param>
    /// <returns><see langword="true"/> if the fish with the specified <paramref name="fishId"/> has been caught with max size, otherwise <see langword="false"/>.</returns>
    internal static bool HasCaughtMaxSized(this Farmer farmer, string fishId)
    {
        var qid = $"(O){fishId}";
        if (!farmer.fishCaught.ContainsKey(qid) || farmer.fishCaught[qid][1] <= 0)
        {
            return false;
        }

        var fishData = DataLoader.Fish(Game1.content)
            .Where(p => !p.Key.IsAlgaeId())
            .ToDictionary(p => p.Key, p => p.Value);
        if (!fishData.TryGetValue(fishId, out var specificFishData))
        {
            return false;
        }

        var rawSplit = specificFishData.SplitWithoutAllocation('/');
        return specificFishData.Contains("trap")
            ? farmer.fishCaught[qid][1] >= int.Parse(rawSplit[6])
            : farmer.fishCaught[qid][1] > int.Parse(rawSplit[4]);
    }

    /// <summary>Gets the price bonus applied to animal produce sold by <see cref="Profession.Producer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for animal products.</returns>
    internal static float GetProducerSaleBonus(this Farmer farmer)
    {
        var sum = 0f;
        Utility.ForEachBuilding(b =>
        {
            if (b.IsOwnedByOrLax(farmer) && b.buildingType.Value.ContainsAnyOf("Deluxe", "Premium") &&
                b.indoors.Value is AnimalHouse house && house.isFull())
            {
                sum += 0.05f;
            }

            return true; // continue enumeration
        });

        return sum;
    }

    /// <summary>Gets the price bonus applied to fish sold by <see cref="Profession.Angler"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> multiplier for fish prices.</returns>
    internal static float GetAnglerSaleBonus(this Farmer farmer)
    {
        var fishData = DataLoader.Fish(Game1.content);
        var bonus = 0f;
        foreach (var (key, value) in farmer.fishCaught.Pairs)
        {
            if (key.Split("(O)") is not { Length: 2 } split)
            {
                continue;
            }

            var id = split[1];
            if (id.IsAlgaeId() || !fishData.TryGetValue(id, out var specificFishData))
            {
                continue;
            }

            var dataFields = specificFishData.SplitWithoutAllocation('/');
            if (dataFields[2] == "trap" || !int.TryParse(dataFields[4], out var maxSize))
            {
                continue;
            }

            var singleBonus = Config.AnglerPriceBonusRate;
            if (singleBonus <= 0f)
            {
                singleBonus = Config.AnglerPriceBonusCeiling / fishData.Count;
            }

            if (id.IsBossFishId())
            {
                bonus += singleBonus * 5f;
            }
            else if (value[1] > maxSize)
            {
                bonus += singleBonus;
            }
        }

        return Math.Min(bonus, Config.AnglerPriceBonusCeiling);
    }

    /// <summary>Gets the bonus "catching" bar build rate for <see cref="Profession.Aquarist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="float"/> catching height.</returns>
    internal static float GetAquaristCatchingHandicap(this Farmer farmer)
    {
        HashSet<string> fishTypes = [];
        Utility.ForEachBuilding(b =>
        {
            if (b is FishPond pond && pond.IsOwnedByOrLax(Game1.player) && !string.IsNullOrEmpty(pond.fishType.Value))
            {
                fishTypes.Add(pond.fishType.Value);
            }

            return true; // continue enumeration
        });

        return Math.Min(Math.Min(fishTypes.Count, Config.AquaristFishPondCeiling) * 0.000165f, 0.00198f);
    }

    /// <summary>Gets the quality of items foraged by <see cref="Profession.Ecologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    internal static int GetEcologistForageQuality(this Farmer farmer)
    {
        var itemsForaged = Data.Read(farmer, DataKeys.EcologistVarietiesForaged)
            .ParseList<string>()
            .ToHashSet()
            .Count;
        return itemsForaged < Config.ForagesNeededForBestQuality
            ? itemsForaged < Config.ForagesNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>Applies bonus edibility to an item gathered by <see cref="Profession.Ecologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="object">The <see cref="SObject"/>>.</param>
    internal static void ApplyEcologistEdibility(this Farmer farmer, SObject @object)
    {
        if (@object.Edibility <= 0)
        {
            return;
        }

        @object.Edibility = farmer.HasProfession(Profession.Ecologist, true)
            ? @object.Edibility * 2
            : (int)(@object.Edibility * 1.5f);
    }

    /// <summary>Applies one of the Prestiged <see cref="Profession.Ecologist"/> food buffs to the <paramref name="farmer"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <param name="buffIndex">The buff index.</param>
    internal static void ApplyPrestigedEcologistBuff(this Farmer farmer, int buffIndex)
    {
        Buff? buff = buffIndex switch
        {
            0 => new EcologistFarmingBuff(),
            1 => new EcologistFishingBuff(),
            2 => new EcologistMiningBuff(),
            3 => new EcologistLuckLevelBuff(),
            4 => new EcologistForagingBuff(),
            5 => new EcologistMaxStaminaBuff(),
            6 => new EcologistMagneticRadiusBuff(),
            7 => new EcologistSpeedBuff(),
            8 => new EcologistDefenseBuff(),
            9 => new EcologistAttackBuff(),
            _ => null,
        };

        farmer.applyBuff(buff);
    }

    /// <summary>Gets the quality of minerals collected by <see cref="Profession.Gemologist"/>.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="SObject"/> quality level.</returns>
    internal static int GetGemologistMineralQuality(this Farmer farmer)
    {
        var mineralsCollected = Data.Read(farmer, DataKeys.GemologistMineralsStudied)
            .ParseList<string>()
            .ToHashSet()
            .Count;
        return mineralsCollected < Config.MineralsNeededForBestQuality
            ? mineralsCollected < Config.MineralsNeededForBestQuality / 2
                ? SObject.medQuality
                : SObject.highQuality
            : SObject.bestQuality;
    }

    /// <summary>
    ///     Determines whether the <paramref name="farmer"/> is currently using the <see cref="Profession.Poacher"/>
    ///     <see cref="LimitBreak"/>.
    /// </summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if <see cref="PoacherAmbush"/> is active.</returns>
    internal static bool IsAmbushing(this Farmer farmer)
    {
        return farmer.Get_LimitBreak() is PoacherAmbush { IsActive: true };
    }

    /// <summary>Gets a list with all <see cref="GreenSlime"/>s currently inhabiting the farm and owned <see cref="SlimeHutch"/>es.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="GreenSlime"/>s currently inhabiting the farm and owned <see cref="SlimeHutch"/>es.</returns>
    internal static List<GreenSlime> GetRaisedSlimes(this Farmer farmer)
    {
        var raised = Game1.getFarm().characters.OfType<GreenSlime>().ToList();
        Utility.ForEachBuilding(b =>
        {
            if (b.IsOwnedByOrLax(farmer) && b.indoors.Value is SlimeHutch)
            {
                raised.AddRange(b.indoors.Value.characters.OfType<GreenSlime>());
            }

            return true; // continue enumeration
        });

        return raised;
    }

    /// <summary>Counts the <see cref="GreenSlime"/>s currently inhabiting the farm and owned <see cref="SlimeHutch"/>es.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The total number of <see cref="GreenSlime"/>s currently inhabiting the farm and owned <see cref="SlimeHutch"/>es.</returns>
    internal static int CountRaisedSlimes(this Farmer farmer)
    {
        //var count = Game1.getFarm().characters.OfType<GreenSlime>().Count();
        var count = 0;
        Utility.ForEachBuilding(b =>
        {
            if (b.IsOwnedByOrLax(farmer) && b.indoors.Value is SlimeHutch)
            {
                count += b.indoors.Value.characters.OfType<GreenSlime>().Count();
            }

            return true; // continue enumeration
        });

        return count;
    }

    /// <summary>Enumerates all <see cref="PipedSlime"/>s currently under the <paramref name="farmer"/>'s influence.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of all <see cref="PipedSlime"/>s currently under the <paramref name="farmer"/>'s influence.</returns>
    internal static IEnumerable<PipedSlime> GetPipedSlimes(this Farmer farmer)
    {
        return GreenSlime_Piped.Values
            .Where(pair => ReferenceEquals(pair.Value.Piper, farmer))
            .Select(pair => pair.Value)
            .ToList();
    }

    /// <summary>Counts the <see cref="PipedSlime"/>s currently under the <paramref name="farmer"/>'s influence.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns>The total number of <see cref="PipedSlime"/>s currently under the <paramref name="farmer"/>'s influence.</returns>
    internal static int CountPipedSlimes(this Farmer farmer)
    {
        return GreenSlime_Piped.Values.Count(pair => ReferenceEquals(pair.Value.Piper, farmer));
    }

    /// <summary>Implements the Slime Flute logic for summoning minions.</summary>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    internal static void PlaySlimeFlute(this Farmer farmer)
    {
        if (!farmer.HasProfession(Profession.Piper))
        {
            Game1.showRedMessage(I18n.Piper_Useflute_CantPlay());
            return;
        }

        var inDangerZone = farmer.currentLocation.IsEnemyArea();
        if (!inDangerZone)
        {
            Game1.showRedMessage(I18n.Piper_Useflute_NotHere());
            return;
        }

        if (State.SlimeFluteCooldown > 0 && !Config.ModKey.IsDown())
        {
            Game1.playSound("cancel");
            return;
        }

        farmer.faceDirection(Game1.down);
        Game1.MusicDuckTimer = 2000f;
        Game1.playSound("horse_flute");
        Game1.player.FarmerSprite.animateOnce(
        [
                new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
                new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
                new FarmerSprite.AnimationFrame(100, 200, secondaryArm: true, flip: false),
                new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false),
                new FarmerSprite.AnimationFrame(98, 400, secondaryArm: true, flip: false),
                new FarmerSprite.AnimationFrame(99, 200, secondaryArm: true, flip: false)
        ]);

        Game1.player.freezePause = 1500;

        DelayedAction.functionAfterDelay(
            () =>
            {
                var action = Config.ModKey.IsDown() ? "dismiss" : "spawn";

                var numberRaised = farmer.CountRaisedSlimes();
                if (numberRaised == 0)
                {
                    Game1.showRedMessage(I18n.Piper_Useflute_NothingHappened());
                    return;
                }

                var luck = Game1.player.DailyLuck;
                var baseNumberToSpawn = 1 + (numberRaised / 5);

                int[] offsets = [-2, -1, 0, 1, 2];
                float[] weights = [1f, 3f, 8f, 8f, 3f, 1f], skewedWeights = new float[weights.Length];
                const float luckFactor = 6f;
                for (var i = 0; i < weights.Length; i++)
                {
                    skewedWeights[i] = (float)(weights[i] * Math.Exp(luck * luckFactor * offsets[i]));
                }

                var totalWeight = skewedWeights.Sum();
                var roll = (float)Game1.random.NextDouble() * totalWeight;
                var rolledOffset = 0;
                for (var i = 0; i < weights.Length; i++)
                {
                    roll -= weights[i];
                    if (roll <= 0f)
                    {
                        rolledOffset = offsets[i];
                        break;
                    }
                }

                var numberToSpawn = baseNumberToSpawn + rolledOffset;

                if (!Game1.IsMasterGame)
                {
                    Broadcaster.MessageHost(numberToSpawn.ToString(), $"PiperMinions_{action}");
                }
                else
                {
                    farmer.SpawnMinions(numberToSpawn);
                }

                farmer.CharmMinions();
            },
            1500);

        State.SlimeFluteCooldown = 60;
    }

    /// <summary>Charms nearby <see cref="GreenSlime"/>s.</summary>
    /// <param name="piper">The <see cref="Farmer"/>.</param>
    internal static void CharmMinions(this Farmer piper)
    {
        foreach (var slime in piper.currentLocation.characters.OfType<GreenSlime>())
        {
            if (slime.IsCharacterWithinThreshold(piper) && !slime.IsPiped())
            {
                slime.Set_Piped(piper, PipedSlime.PipingSource.Charmed);
            }
        }
    }

    /// <summary>Spawns a <see cref="GreenSlime"/> minion next to the <paramref name="piper"/>.</summary>
    /// <param name="piper">The <see cref="Farmer"/>.</param>
    /// <param name="numberToSpawn">The number of minions to spawn.</param>
    /// <returns>The <see cref="Vector2"/> tiles where minions were spawned.</returns>
    internal static IEnumerable<Vector2> SpawnMinions(this Farmer piper, int numberToSpawn)
    {
        if (piper.GetPipedSlimes().Count() >= numberToSpawn)
        {
            yield break;
        }

        var location = piper.currentLocation;
        var r = Random.Shared;
        var raisedSlimes = piper.GetRaisedSlimes();
        for (var i = 0; i < numberToSpawn; i++)
        {
            var condition = location switch
            {
                MineShaft => IsMineShaftTileSpawnable,
                VolcanoDungeon => IsVolcanoTileSpawnable,
                _ => IsGenericTileSpawnable,
            };

            var spawnTile = piper.ChooseFromFourtyEightNeighboringTiles(condition, location);
            GreenSlime spawn;
            if (!raisedSlimes.TryChoose(out var chosen, r))
            {
                spawn = new GreenSlime(spawnTile * Game1.tileSize, 20)
                {
                    currentLocation = location,
                };

                goto setPiped;
            }

            int minAttack = int.MinValue, minDefense = int.MinValue, minHealth = int.MinValue;
            int maxAttack = int.MaxValue, maxDefense = int.MaxValue, maxHealth = int.MaxValue;
            foreach (var slime in raisedSlimes)
            {
                if (slime.DamageToFarmer < minAttack)
                {
                    minAttack = slime.DamageToFarmer;
                }

                if (slime.DamageToFarmer > maxAttack)
                {
                    maxAttack = slime.DamageToFarmer;
                }

                if (slime.resilience.Value < minDefense)
                {
                    minDefense = slime.resilience.Value;
                }

                if (slime.resilience.Value > maxDefense)
                {
                    maxDefense = slime.resilience.Value;
                }

                if (slime.MaxHealth < minHealth)
                {
                    minHealth = slime.MaxHealth;
                }

                if (slime.MaxHealth > maxHealth)
                {
                    maxHealth = slime.MaxHealth;
                }
            }

            spawn = new GreenSlime(spawnTile * Game1.tileSize, chosen.color.Value)
            {
                currentLocation = location,
                MaxHealth = r.Next(minHealth, maxHealth + 1),
                DamageToFarmer = r.Next(minAttack, maxAttack + 1),
                resilience = { Value = r.Next(minDefense, maxDefense + 1) },
            };

            spawn.Health = spawn.MaxHealth;
            switch (chosen.Name)
            {
                case "Tiger Slime":
                    spawn.makeTigerSlime();
                    break;
                case "Gold Slime":
                    spawn.MakeGoldSlime();
                    break;
                default:
                {
                    if (chosen.prismatic.Value)
                    {
                        spawn.prismatic.Value = true;
                        spawn.Name = "Prismatic Slime";
                        spawn.MaxHealth *= 2;
                        spawn.DamageToFarmer *= 2;
                        spawn.resilience.Value *= 2;
                    }

                    break;
                }
            }

        setPiped:
            var piped = spawn.Set_Piped(piper, PipedSlime.PipingSource.Summoned);

            DelayedAction.functionAfterDelay(
                () =>
                {
                    location.characters.Add(spawn);
                    for (var j = 0; j < 8; j++)
                    {
                        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                        .broadcastSprites(
                            piper.currentLocation,
                            new TemporaryAnimatedSprite(
                                10,
                                new Vector2(
                                    spawnTile.X + Utility.RandomFloat(-1f, 1f),
                                    spawnTile.Y + Utility.RandomFloat(-1f, 0f)) * 64f,
                                Color.LimeGreen,
                                8,
                                flipped: false,
                                50f)
                            {
                                layerDepth = 1f,
                                motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f)),
                            });
                    }
                },
                500 + Game1.random.Next(-200, 200));

            yield return spawnTile;
        }
    }

    /// <summary>Dismisses active <see cref="GreenSlime"/> minions assigned to the <paramref name="piper"/>.</summary>
    /// <param name="piper">The <see cref="Farmer"/>.</param>
    internal static void DismissMinions(this Farmer piper)
    {
        var pipedSlimes = GreenSlime_Piped.PipedSlimes.ToList();
        for (var i = pipedSlimes.Count - 1; i > 0; i--)
        {
            var slime = pipedSlimes[i];
            var piped = slime.Get_Piped();
            if (piped?.HasHat ?? false)
            {
                continue;
            }

            var (tileX, tileY) = slime.Tile;
            DelayedAction.functionAfterDelay(
                () =>
                {
                    for (var j = 0; j < 8; j++)
                    {
                        Reflector.GetStaticFieldGetter<Multiplayer>(typeof(Game1), "multiplayer").Invoke()
                        .broadcastSprites(
                            piper.currentLocation,
                            new TemporaryAnimatedSprite(
                                10,
                                new Vector2(
                                    tileX + Utility.RandomFloat(-1f, 1f),
                                    tileY + Utility.RandomFloat(-1f, 0f)) * 64f,
                                Color.LimeGreen,
                                8,
                                flipped: false,
                                50f)
                            {
                                layerDepth = 1f,
                                motion = new Vector2(Utility.RandomFloat(-0.5f, 0.5f), Utility.RandomFloat(-0.5f, 0.5f)),
                            });
                    }

                    piper.currentLocation.characters.Remove(slime);
                },
                500 + Game1.random.Next(-200, 200));

            slime.Set_Piped(null, PipedSlime.PipingSource.None);
            pipedSlimes.RemoveAt(i);
        }
    }
}
