namespace DaLion.Stardew.Ponds.Framework.Patches.Integrations;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

using Common.Extensions;
using Common.Extensions.Reflection;
using Extensions;

using Object = StardewValley.Object;

#endregion using directives

internal class AutomatePatches
{
    private static MethodInfo _GetMachine;
    private static MethodInfo _GetOwner;

    /// <summary>Apply integration patches.</summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance.</param>
    internal static void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: "Pathoschild.Stardew.Automate.Framework.Machines.Buildings.FishPondMachine".ToType().RequireMethod("OnOutputTaken"),
            prefix: new(typeof(AutomatePatches).RequireMethod(nameof(FishPondMachineOnOutputTakenPrefix)))
        );
    }

    #region harmony patches

    /// <summary>Harvest produce from mod data until none are left.</summary>
    private static bool FishPondMachineOnOutputTakenPrefix(object __instance, Item item)
    {
        FishPond machine = null;
        try
        {
            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
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
                Object o;
                if (index == 812) // roe
                {
                    var split = Game1.objectInformation[machine.fishType.Value].Split('/');
                    var c = machine.fishType.Value == 698
                        ? new(61, 55, 42)
                        : TailoringMenu.GetDyeColor(machine.GetFishObject()) ?? Color.Orange;
                    o = new ColoredObject(812, stack, c);
                    o.name = split[0] + " Roe";
                    o.preserve.Value = Object.PreserveType.Roe;
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

            if (machine.ReadDataAs<bool>("CheckedToday")) return false; // don't run original logic

            var bonus = (int) (item is Object @object
                ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
                : 0);

            _GetOwner ??= __instance.GetType().RequireMethod("GetOwner");
            ((Farmer) _GetOwner.Invoke(__instance, null))?.gainExperience((int) SkillType.Fishing,
                FishPond.HARVEST_BASE_EXP + bonus);

            machine.WriteData("CheckedToday", true.ToString());
            return false; // don't run original logic
        }
        catch (InvalidOperationException ex) when (machine is not null)
        {
            ModEntry.Log($"ItemsHeld data is invalid. {ex}\nThe data will be reset", LogLevel.Warn);
            machine.WriteData("ItemsHeld", null);
            return true; // default to original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}