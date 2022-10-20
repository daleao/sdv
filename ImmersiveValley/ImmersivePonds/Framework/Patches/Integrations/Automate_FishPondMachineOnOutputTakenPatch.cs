namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using System.Linq;
using System.Reflection;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using StardewValley.Menus;
using StardewValley.Objects;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class FishPondMachineOnOutputTakenPatch : HarmonyPatch
{
    private static Func<object, FishPond>? _getMachine;
    private static Func<object, Farmer>? _getOwner;

    /// <summary>Initializes a new instance of the <see cref="FishPondMachineOnOutputTakenPatch"/> class.</summary>
    internal FishPondMachineOnOutputTakenPatch()
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
            _getMachine ??= __instance
                .GetType()
                .RequirePropertyGetter("Machine")
                .CompileUnboundDelegate<Func<object, FishPond>>();
            machine = _getMachine(__instance);

            var produce = machine.Read(DataFields.ItemsHeld).ParseList<string>(";");
            if (produce.Count <= 0)
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
                    var split = Game1.objectInformation[machine.fishType.Value].Split('/');
                    var c = machine.fishType.Value == 698
                        ? new Color(61, 55, 42)
                        : TailoringMenu.GetDyeColor(machine.GetFishObject()) ?? Color.Orange;
                    o = new ColoredObject(812, stack, c);
                    o.name = split[0] + " Roe";
                    o.preserve.Value = SObject.PreserveType.Roe;
                    o.preservedParentSheetIndex.Value = machine.fishType.Value;
                    o.Price += Convert.ToInt32(split[1]) / 2;
                    o.Quality = quality;
                }
                else
                {
                    o = new SObject(index, stack, quality: quality);
                }

                machine.output.Value = o;
                produce.Remove(next);
                machine.Write(DataFields.ItemsHeld, string.Join(";", produce));
            }

            if (machine.Read<bool>(DataFields.CheckedToday))
            {
                return false; // don't run original logic
            }

            var bonus = (int)(item is SObject obj
                ? obj.sellToStorePrice() * FishPond.HARVEST_OUTPUT_EXP_MULTIPLIER
                : 0);

            _getOwner ??= __instance
                .GetType()
                .RequireMethod("GetOwner")
                .CompileUnboundDelegate<Func<object, Farmer>>();
            _getOwner(__instance).gainExperience(Farmer.fishingSkill, FishPond.HARVEST_BASE_EXP + bonus);

            machine.Write(DataFields.CheckedToday, true.ToString());
            return false; // don't run original logic
        }
        catch (InvalidOperationException ex) when (machine is not null)
        {
            Log.W($"ItemsHeld data is invalid. {ex}\nThe data will be reset");
            machine.Write(DataFields.ItemsHeld, null);
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
