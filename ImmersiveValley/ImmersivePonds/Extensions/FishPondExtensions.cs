namespace DaLion.Stardew.Ponds.Extensions;

#region using directives

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Reflection;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.FishPond;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

/// <summary>Extensions for the <see cref="FishPond"/> class.</summary>
public static class FishPondExtensions
{
    private static readonly Func<FishPond, FishPondData?> _GetFishPondData = typeof(FishPond).RequireField("_fishPondData")
        .CompileUnboundFieldGetterDelegate<Func<FishPond, FishPondData?>>();

    /// <summary>Whether the instance's population has been fully unlocked.</summary>
    public static bool HasUnlockedFinalPopulationGate(this FishPond pond)
    {
        var data = _GetFishPondData(pond);
        return data?.PopulationGates is null ||
               pond.lastUnlockedPopulationGate.Value >= data.PopulationGates.Keys.Max();
    }

    /// <summary>Whether a legendary fish lives in this pond.</summary>
    public static bool IsLegendaryPond(this FishPond pond) =>
        pond.GetFishObject().HasContextTag("fish_legendary");

    /// <summary>Whether this pond is infested with algae.</summary>
    public static bool IsAlgaePond(this FishPond pond) =>
        pond.fishType.Value.IsAlgae();

    /// <summary>Give the player fishing experience for harvesting the pond.</summary>
    /// <param name="who">The player.</param>
    public static void RewardExp(this FishPond pond, Farmer who)
    {
        if (ModDataIO.ReadDataAs<bool>(pond, "CheckedToday")) return;

        var bonus = (int)(pond.output.Value is SObject @object
            ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
            : 0);
        who.gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);
    }

    /// <summary>Opens a <see cref="ItemGrabMenu"/> instance to allow retrieve multiple items from the pond's chum bucket.</summary>
    /// <returns>Always returns <see langword="true"> (required by vanilla code).</returns>
    public static bool OpenChumBucketMenu(this FishPond pond, Farmer who)
    {
        var held = ModDataIO.ReadData(pond, "ItemsHeld").ParseList<string>(";");
        if (held?.Any() != true)
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
            var produce = new List<Item> { pond.output.Value };
            try
            {
                foreach (var p in held)
                {
                    var (index, stack, quality) = p.ParseTuple<int, int, int>();
                    if (index == Constants.ROE_INDEX_I)
                    {
                        var split = Game1.objectInformation[pond.fishType.Value].Split('/');
                        var c = pond.fishType.Value == 698 ? new(61, 55, 42) : TailoringMenu.GetDyeColor(pond.GetFishObject()) ?? Color.Orange;
                        var o = new ColoredObject(Constants.ROE_INDEX_I, stack, c);
                        o.name = split[0] + " Roe";
                        o.preserve.Value = SObject.PreserveType.Roe;
                        o.preservedParentSheetIndex.Value = pond.fishType.Value;
                        o.Price += Convert.ToInt32(split[1]) / 2;
                        o.Quality = quality;
                        produce.Add(o);
                    }
                    else
                    {
                        produce.Add(new SObject(index, stack) { Quality = quality });
                    }
                }

                Game1.activeClickableMenu = new ItemGrabMenu(produce, pond).setEssential(false);
                ((ItemGrabMenu)Game1.activeClickableMenu).source = ItemGrabMenu.source_fishingChest;
            }
            catch (InvalidOperationException ex)
            {
                Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
                ModDataIO.WriteData(pond, "ItemsHeld", null);
            }
        }

        ModDataIO.WriteData(pond, "CheckedToday", true.ToString());
        return true; // expected by vanilla code
    }
}