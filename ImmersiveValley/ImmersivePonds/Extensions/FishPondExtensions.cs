namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using Common;
using Common.Extensions;
using Common.Extensions.Collections;
using Common.Extensions.Reflection;
using Common.ModData;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly Lazy<Func<FishPond, FishPondData?>> _GetFishPondData = new(() =>
        typeof(FishPond).RequireField("_fishPondData").CompileUnboundFieldGetterDelegate<FishPond, FishPondData?>());

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var data = _GetFishPondData.Value(pond);
        return data?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= data.PopulationGates.Keys.Max();
    }

    /// <summary>Whether this pond is infested with algae.</summary>
    public static bool HasAlgae(this FishPond pond) =>
        pond.fishType.Value.IsAlgaeIndex();

    /// <summary>Whether a radioactive fish lives in this pond.</summary>
    public static bool HasRadioactiveFish(this FishPond pond) =>
        pond.GetFishObject().IsRadioactiveFish();

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    public static bool HasLegendaryFish(this FishPond pond) =>
        pond.GetFishObject().IsLegendary();

    /// <summary>Get the number of days required to enrich a given metallic resource.<summary>
    public static int GetEnrichmentDuration(this FishPond pond, SObject metallic)
    {
        var maxPopulation = pond.HasLegendaryFish()
            ? ModEntry.ProfessionsAPI?.GetConfigs().LegendaryPondPopulationCap ?? 12
            : 12;
        var populationFactor = pond.FishCount < maxPopulation / 2f
            ? 0f
            : maxPopulation / 2f / pond.FishCount;
        if (populationFactor == 0) return 0;

        var days = 0;
        if (metallic.Name.Contains("Copper")) days = 16;
        else if (metallic.Name.Contains("Iron")) days = 8;
        else if (metallic.Name.Contains("Gold")) days = 4;
        else if (metallic.Name.Contains("Iridium")) days = 2;

        if (metallic.IsNonRadioactiveIngot()) days *= 3;

        return (int)Math.Max(days * populationFactor, 1);
    }

    /// <summary>Give the player fishing experience for harvesting the pond.</summary>
    /// <param name="who">The player.</param>
    public static void RewardExp(this FishPond pond, Farmer who)
    {
        if (ModDataIO.Read<bool>(pond, "CheckedToday")) return;

        var bonus = (int)(pond.output.Value is SObject @object
            ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>Opens a <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the pond's chum bucket.</summary>
    /// <returns>Always returns <see langword="true"> (required by vanilla code).</returns>
    public static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var held = pond.DeserializeObjectListData("ItemsHeld");
        if (held.Count <= 0)
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
                    if (h.ParentSheetIndex == Constants.ROE_INDEX_I)
                    {
                        var fishIndex = pond.fishType.Value;
                        var split = Game1.objectInformation[fishIndex].Split('/');
                        var c = fishIndex == 698
                            ? new(61, 55, 42)
                            : TailoringMenu.GetDyeColor(pond.GetFishObject()) ?? Color.Orange;
                        var o = new ColoredObject(Constants.ROE_INDEX_I, h.Stack, c);
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
                ModDataIO.Write(pond, "ItemsHeld", null);
            }
        }

        ModDataIO.Write(pond, "CheckedToday", true.ToString());
        return true; // expected by vanilla code
    }

    /// <summary>Read a serialized item list from the fish pond's mod data and return a deserialized list of <see cref="SObject"/>.</summary>
    /// <param name="pond">The <see cref="FishPond"/>.</param>
    /// <param name="field">The data field.</param>
    internal static List<Item> DeserializeObjectListData(this FishPond pond, string field) =>
        ModDataIO.Read(pond, field)
            .ParseList<string>(";")?
            .Select(s => s?.ParseTuple<int, int, int>())
            .WhereNotNull()
            .Select(t => new SObject(t.Item1, t.Item2, quality: t.Item3))
            .Cast<Item>()
            .ToList() ?? new List<Item>();
}