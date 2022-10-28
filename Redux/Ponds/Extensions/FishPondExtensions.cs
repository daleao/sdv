namespace DaLion.Redux.Ponds.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Redux.Core.Extensions;
using DaLion.Redux.Professions.Extensions;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
internal static class FishPondExtensions
{
    /// <summary>Determines whether the <paramref name="pond"/>'s population has been fully unlocked.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the last unlocked population gate matches the last gate in the <see cref="FishPondData"/>, otherwise <see langword="false"/>.</returns>
    internal static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var data = ModEntry.Reflector
            .GetUnboundFieldGetter<FishPond, FishPondData?>(pond, "_fishPondData")
            .Invoke(pond);
        return data?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= data.PopulationGates.Keys.Max();
    }

    /// <summary>Determines whether this <paramref name="pond"/> is infested with algae.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses any algae, otherwise <see langword="false"/>.</returns>
    internal static bool HasAlgae(this FishPond pond)
    {
        return pond.fishType.Value.IsAlgaeIndex();
    }

    /// <summary>Determines whether the <paramref name="pond"/>'s radioactivity is sufficient to enrich metallic nuclei.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="pond"/> houses a mutant or radioactive fish species, otherwise <see langword="false"/>.</returns>
    internal static bool IsRadioactive(this FishPond pond)
    {
        return pond.GetFishObject().IsRadioactiveFish() && pond.FishCount > 2;
    }

    /// <summary>Gets the number of days required to enrich a given <paramref name="metal"/> resource.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="metal">An ore or bar <see cref="SObject"/>.</param>
    /// <returns>The number of days required to enrich the nucleus of the metal.</returns>
    internal static int GetEnrichmentDuration(this FishPond pond, SObject metal)
    {
        var maxPopulation = pond.HasLegendaryFish()
            ? ModEntry.Config.Professions.LegendaryPondPopulationCap
            : 12;
        var populationFactor = pond.FishCount < maxPopulation / 2f
            ? 0f
            : maxPopulation / 2f / pond.FishCount;
        if (populationFactor == 0)
        {
            return 0;
        }

        var days = 0;
        if (metal.Name.Contains("Copper"))
        {
            days = 16;
        }
        else if (metal.Name.Contains("Iron"))
        {
            days = 8;
        }
        else if (metal.Name.Contains("Gold"))
        {
            days = 4;
        }
        else if (metal.Name.Contains("Iridium"))
        {
            days = 2;
        }

        if (metal.Name.Contains("Ore"))
        {
            days *= 3;
        }

        return (int)Math.Max(days * populationFactor, 1);
    }

    /// <summary>Gives the player fishing experience for harvesting the <paramref name="pond"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The player.</param>
    internal static void RewardExp(this FishPond pond, Farmer who)
    {
        if (pond.Read<bool>(DataFields.CheckedToday))
        {
            return;
        }

        var bonus = (int)(pond.output.Value is SObject obj
            ? obj.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>
    ///     Opens an <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the
    ///     <paramref name="pond"/>'s chum bucket.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> interacting with the <paramref name="pond"/>.</param>
    /// <returns>Always <see langword="true"/> (required by vanilla code).</returns>
    internal static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var held = pond.DeserializeObjectListData(DataFields.ItemsHeld);
        if (held.Count == 0)
        {
            if (who.addItemToInventoryBool(pond.output.Value))
            {
                Game1.playSound("coin");
                pond.output.Value = null;
            }
            else
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
            }
        }
        else
        {
            var inventory = new List<Item> { pond.output.Value };
            try
            {
                foreach (var h in held)
                {
                    if (h.ParentSheetIndex == Constants.RoeIndex)
                    {
                        var fishIndex = pond.fishType.Value;
                        var split = Game1.objectInformation[fishIndex].Split('/');
                        var c = fishIndex == 698
                            ? new Color(61, 55, 42)
                            : TailoringMenu.GetDyeColor(pond.GetFishObject()) ?? Color.Orange;
                        var o = new ColoredObject(Constants.RoeIndex, h.Stack, c);
                        o.name = split[0] + " Roe";
                        o.preserve.Value = SObject.PreserveType.Roe;
                        o.preservedParentSheetIndex.Value = fishIndex;
                        o.Price += Convert.ToInt32(split[1]) / 2;
                        o.Quality = ((SObject)h).Quality;
                        inventory.Add(o);
                    }
                    else
                    {
                        inventory.Add(h);
                    }
                }

                var menu = new ItemGrabMenu(inventory, pond).setEssential(false);
                menu.source = ItemGrabMenu.source_fishingChest;
                Game1.activeClickableMenu = menu;
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
                pond.Write(DataFields.ItemsHeld, null);
            }
        }

        pond.Write(DataFields.CheckedToday, true.ToString());
        return true; // expected by vanilla code
    }

    /// <summary>
    ///     Reads a serialized item list from the <paramref name="pond"/>'s <seealso cref="ModDataDictionary"/> and
    ///     returns a deserialized <see cref="List{T}"/> of <seealso cref="SObject"/>s.
    /// </summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="field">The data field.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Item"/>s which were encoded in <paramref name="field"/>.</returns>
    internal static List<Item> DeserializeObjectListData(this FishPond pond, string field)
    {
        return pond.Read(field)
            .ParseList<string>(";")
            .Select(s => s?.ParseTuple<int, int, int>())
            .WhereNotNull()
            .Select(t => new SObject(t.Item1, t.Item2, quality: t.Item3))
            .Cast<Item>()
            .ToList();
    }
}
