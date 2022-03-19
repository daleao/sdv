namespace DaLion.Stardew.FishPonds.Automate;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

using Pathoschild.Stardew.Automate;
using Common.Extensions;
using Framework.Extensions;

using SObject = StardewValley.Object;

#endregion using directives

/// <summary>The mod entry point.</summary>
public class ModEntry : Mod
{
    internal static Action<string, LogLevel> Log { get; private set; }

    private static MethodInfo _GetMachine;
    private static MethodInfo _GetOwner;

    /// <summary>The mod entry point, called after the mod is first loaded.</summary>
    /// <param name="helper">Provides simplified APIs for writing mods.</param>
    public override void Entry(IModHelper helper)
    {
        // store reference to logger
        Log = Monitor.Log;

        // register events
        Helper.Events.GameLoop.DayStarted += OnDayStarted;

        // apply harmony patch
        var target = 
        new Harmony(ModManifest.UniqueID).Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Buildings.FishPondMachine".ToType()
                .MethodNamed("OnOutputTaken"),
            prefix: new(typeof(ModEntry).MethodNamed(nameof(FishPondMachineOnOutputTakenPrefix)))
        );
    }

    /// <summary>Raised after a new in-game day starts, or after connecting to a multiplayer world.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnDayStarted(object sender, DayStartedEventArgs e)
    {
        foreach (var pond in Game1.getFarm().buildings.OfType<FishPond>().Where(p =>
                     (p.owner.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer) &&
                     !p.isUnderConstruction()))
            pond.WriteData("HarvestedToday", false.ToString());
    }

    /// <summary>Harvest produce from mod data until none are left.</summary>
    private static bool FishPondMachineOnOutputTakenPrefix(IMachine __instance, Item item)
    {
        FishPond machine = null;
        try
        {
            _GetMachine ??= __instance.GetType().PropertyGetter("Machine");
            machine = (FishPond) _GetMachine.Invoke(__instance, null);
            if (machine is null) return true; // run original logic

            var produce = machine.ReadData("ItemsHeld", null)?.ParseList<string>(";");
            if (produce is null)
            {
                machine.output.Value = null;
            }
            else
            {
                var next = produce.First();
                var (index, stack, quality) = next.ParseTuple<int, int, int>();
                SObject o;
                if (index == 812) // roe
                {
                    var split = Game1.objectInformation[machine.fishType.Value].Split('/');
                    var c = TailoringMenu.GetDyeColor(machine.GetFishObject()) ??
                            (machine.fishType.Value == 698 ? new(61, 55, 42) : Color.Orange);
                    o = new ColoredObject(812, stack, c);
                    o.name = split[0] + " Roe";
                    o.preserve.Value = SObject.PreserveType.Roe;
                    o.preservedParentSheetIndex.Value = machine.fishType.Value;
                    o.Price += Convert.ToInt32(split[1]) / 2;
                    o.Quality = quality;
                }
                else
                {
                    o = new(index, stack) {Quality = quality};
                }

                machine.output.Value = o;
                produce.Remove(next);
                machine.WriteData("ItemsHeld", string.Join(";", produce));
            }

            if (machine.ReadDataAs<bool>("HarvestedToday")) return false; // don't run original logic

            var bonus = (int) (item is SObject @object
                ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
                : 0);

            _GetOwner ??= __instance.GetType().MethodNamed("GetOwner");
            ((Farmer) _GetOwner.Invoke(__instance, null))?.gainExperience((int) SkillType.Fishing,
                FishPond.HARVEST_BASE_EXP + bonus);

            machine.WriteData("HarvestedToday", true.ToString());
            return false; // don't run original logic
        }
        catch (InvalidOperationException ex) when (machine is not null)
        {
            Log($"ItemsHeld data is invalid. {ex}\nThe data will be reset", LogLevel.Warn);
            machine.WriteData("ItemsHeld", null);
            return true; // default to original logic
        }
        catch (Exception ex)
        {
            Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }
}