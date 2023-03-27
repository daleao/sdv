namespace DaLion.Overhaul.Modules.Weapons.Patchers.Infinity;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCurrentToolIndexSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCurrentToolIndexSetterPatcher"/> class.</summary>
    internal FarmerCurrentToolIndexSetterPatcher()
    {
        this.Target = this.RequirePropertySetter<Farmer>(nameof(Farmer.CurrentToolIndex));
    }

    #region harmony patches

    /// <summary>Auto-equip cursed weapon.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, ref int value)
    {
        if (!__instance.Read<bool>(DataKeys.Cursed) ||
            value < 0 || value >= __instance.Items.Count ||
            __instance.Items[value] is not MeleeWeapon weapon ||
            weapon.InitialParentTileIndex == ItemIDs.DarkSword || weapon.isScythe())
        {
            return;
        }

        var darkSword = __instance.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: ItemIDs.DarkSword
        });
        if (darkSword is null)
        {
            Log.W($"Cursed farmer {__instance.Name} is not carrying the Dark Sword. The curse will be forcefully lifted.");
            __instance.Write(DataKeys.Cursed, null);
            return;
        }

        if (Game1.random.NextDouble() > getCurseChance(darkSword.Read<int>(DataKeys.CursePoints)))
        {
            return;
        }

        value = __instance.Items.IndexOf(darkSword);

        double getCurseChance(int x)
        {
            return (0.0008888889 * x) + (0.000002222222 * x * x);
        }
    }

    #endregion harmony patches
}
