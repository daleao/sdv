namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Infinity;

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

    /// <summary>Double stamina consumption when cursed.</summary>
    [HarmonyPrefix]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Preference for inner functions.")]
    private static void FarmerCurrentToolIndexPostfix(Farmer __instance, ref int value)
    {
        if (!__instance.Read<bool>(DataFields.Cursed) || __instance.Items[value] is not MeleeWeapon weapon ||
            weapon.InitialParentTileIndex == Constants.DarkSwordIndex || weapon.isScythe())
        {
            return;
        }

        var darkSword = __instance.Items.FirstOrDefault(item => item is MeleeWeapon
        {
            InitialParentTileIndex: Constants.DarkSwordIndex
        });
        if (darkSword is null)
        {
            Log.W($"Cursed farmer {__instance.Name} is not carrying the Dark Sword. The curse will be forcefully lifted.");
            __instance.Write(DataFields.Cursed, null);
            return;
        }

        if (Game1.random.NextDouble() > getCurseChance(darkSword.Read<int>(DataFields.CursePoints)))
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
