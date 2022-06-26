namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

using Common;
using Common.Data;
using Common.Extensions;
using Common.Extensions.Reflection;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondMachineOnOutputTakenPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, FishPond>? _GetMachine;
    private static Func<object, Farmer>? _GetOwner;

    /// <summary>Construct an instance.</summary>
    internal FishPondMachineOnOutputTakenPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Buildings.FishPondMachine".ToType()
                .RequireMethod("OnOutputTaken");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Harvest produce from mod data until none are left.</summary>
    [HarmonyPrefix]
    private static bool FishPondMachineOnOutputTakenPrefix(object __instance, Item item)
    {
        FishPond? machine = null;
        try
        {
            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CompileUnboundDelegate<Func<object, FishPond>>();
            machine = _GetMachine(__instance);
            
            var produce = ModDataIO.ReadData(machine, "ItemsHeld").ParseList<string>(";");
            if (produce is null)
            {
                machine.output.Value = null;
            }
            else
            {
                var next = produce.First();
                var (index, stack, quality) = next.ParseTuple<int, int, int>();
                StardewValley.Object o;
                if (index == 812) // roe
                {
                    var split = Game1.objectInformation[machine.fishType.Value].Split('/');
                    var c = machine.fishType.Value == 698
                        ? new(61, 55, 42)
                        : TailoringMenu.GetDyeColor(machine.GetFishObject()) ?? Color.Orange;
                    o = new ColoredObject(812, stack, c);
                    o.name = split[0] + " Roe";
                    o.preserve.Value = StardewValley.Object.PreserveType.Roe;
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
                ModDataIO.WriteData(machine, "ItemsHeld", string.Join(";", produce));
            }

            if (ModDataIO.ReadDataAs<bool>(machine, "CheckedToday")) return false; // don't run original logic

            var bonus = (int) (item is StardewValley.Object @object
                ? @object.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
                : 0);

            _GetOwner ??= __instance.GetType().RequireMethod("GetOwner").CompileUnboundDelegate<Func<object, Farmer>>();
            _GetOwner(__instance).gainExperience(Farmer.fishingSkill,
                FishPond.HARVEST_BASE_EXP + bonus);

            ModDataIO.WriteData(machine, "CheckedToday", true.ToString());
            return false; // don't run original logic
        }
        catch (InvalidOperationException ex) when (machine is not null)
        {
            Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
            ModDataIO.WriteData(machine, "ItemsHeld", null);
            return true; // default to original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}