namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;
using FarmerExtensions = DaLion.Professions.Framework.Extensions.FarmerExtensions;

#endregion using directives

[UsedImplicitly]
internal sealed class BobberBarCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="BobberBarCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal BobberBarCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<BobberBar>(
            typeof(string),
            typeof(float),
            typeof(bool),
            typeof(List<string>),
            typeof(string),
            typeof(bool),
            typeof(string),
            typeof(bool));
    }

    #region harmony patches

    /// <summary>Patch for double effectiveness of Deluxe Bait.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void BobberBarCtorPostfix(BobberBar __instance, string baitID)
    {
        if (baitID == "(O)DeluxeBait" && Game1.player.HasProfession(Profession.Fisher))
        {
            __instance.bobberBarHeight += 12;
        }
    }

    #endregion harmony patches
}
