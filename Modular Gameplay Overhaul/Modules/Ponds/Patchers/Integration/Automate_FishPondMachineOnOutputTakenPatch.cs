namespace DaLion.Overhaul.Modules.Ponds.Patchers.Integration;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch specifies the mod in file name but not class to avoid breaking pattern.")]
internal sealed class FishPondMachineOnOutputTakenPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondMachineOnOutputTakenPatcher"/> class.</summary>
    internal FishPondMachineOnOutputTakenPatcher()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Buildings.FishPondMachine"
            .ToType()
            .RequireMethod("OnOutputTaken");
    }

    #region harmony patches

    /// <summary>Harvest produce from mod data until none are left.</summary>
    [HarmonyPrefix]
    private static bool FishPondMachineOnOutputTakenPrefix(object __instance, Item item)
    {
        FishPond? machine = null;
        try
        {
            machine = Reflector
                .GetUnboundPropertyGetter<object, FishPond>(__instance, "Machine")
                .Invoke(__instance);

            var produce = machine.Read(DataKeys.ItemsHeld).ParseList<string>(";");
            if (produce.Count == 0)
            {
                machine.output.Value = null;
            }
            else
            {
                var next = produce.First()!;
                var (index, stack, quality) = next.ParseTuple<int, int, int>()!.Value;
                SObject o;
                if (index == 812) // roe
                {
                    var split = Game1.objectInformation[machine.fishType.Value].SplitWithoutAllocation('/');
                    var c = machine.fishType.Value == 698
                        ? new Color(61, 55, 42)
                        : TailoringMenu.GetDyeColor(machine.GetFishObject()) ?? Color.Orange;
                    o = new ColoredObject(812, stack, c);
                    o.name = split[0].ToString() + " Roe";
                    o.preserve.Value = SObject.PreserveType.Roe;
                    o.preservedParentSheetIndex.Value = machine.fishType.Value;
                    o.Price += int.Parse(split[1]) / 2;
                    o.Quality = quality;
                }
                else
                {
                    o = new SObject(index, stack, quality: quality);
                }

                machine.output.Value = o;
                produce.Remove(next);
                machine.Write(DataKeys.ItemsHeld, string.Join(";", produce));
            }

            if (machine.Read<bool>(DataKeys.CheckedToday))
            {
                return false; // don't run original logic
            }

            var bonus = (int)(item is SObject obj
                ? obj.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
                : 0);

            Reflector
                .GetUnboundMethodDelegate<Func<object, Farmer>>(__instance, "GetOwner")
                .Invoke(__instance)
                .gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);

            machine.Write(DataKeys.CheckedToday, true.ToString());
            return false; // don't run original logic
        }
        catch (InvalidOperationException ex) when (machine is not null)
        {
            Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
            machine.Write(DataKeys.ItemsHeld, null);
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
