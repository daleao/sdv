namespace DaLion.Stardew.FishPonds.Framework.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;

using Common.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly FieldInfo _FishPondData = typeof(FishPond).Field("_fishPondData");

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var fishPondData = (FishPondData) _FishPondData.GetValue(pond);
        return fishPondData?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= fishPondData.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    public static bool IsLegendaryPond(this FishPond pond)
    {
        return pond.GetFishObject().HasContextTag("fish_legendary");
    }

    /// <summary>Whether this pond is infested with algae.</summary>
    public static bool IsAlgaePond(this FishPond pond)
    {
        return pond.fishType.Value.IsAlgae();
    }

    /// <summary>Give the player fishing experience for harvesting the pond.</summary>
    /// <param name="who">The player.</param>
    public static void RewardExp(this FishPond pond, Farmer who)
    {
        if (pond.ReadDataAs<bool>("CheckedToday")) return;

        var bonus = (int) (pond.output.Value is SObject @object
            ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience((int) SkillType.Fishing, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>Opens a <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the pond's chum bucket.</summary>
    /// <returns>Always returns <c>True</c> (required by vanilla code).</returns>
    public static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var produce = pond.ReadData("ItemsHeld", null)?.ParseList<string>(";");
        if (produce is null)
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
            var items = new List<Item> {pond.output.Value};
            try
            {
                foreach (var p in produce)
                {
                    var (index, stack, quality) = p.ParseTuple<int, int, int>();
                    if (index == Constants.ROE_INDEX_I)
                    {
                        var split = Game1.objectInformation[pond.fishType.Value].Split('/');
                        var c = TailoringMenu.GetDyeColor(pond.GetFishObject()) ??
                                (pond.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
                        var o = new ColoredObject(Constants.ROE_INDEX_I, stack, c);
                        o.name = split[0] + " Roe";
                        o.preserve.Value = SObject.PreserveType.Roe;
                        o.preservedParentSheetIndex.Value = pond.fishType.Value;
                        o.Price += Convert.ToInt32(split[1]) / 2;
                        o.Quality = quality;
                        items.Add(o);
                    }
                    else
                    {
                        items.Add(new SObject(index, stack) {Quality = quality});
                    }
                }

                Game1.activeClickableMenu = new ItemGrabMenu(items, pond).setEssential(false);
                ((ItemGrabMenu) Game1.activeClickableMenu).source = ItemGrabMenu.source_fishingChest;
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
                pond.WriteData("ItemsHeld", null);
            }
        }

        pond.WriteData("CheckedToday", true.ToString());
        return true; // expected by vanilla code
    }
}