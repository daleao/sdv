namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Core.Extensions;
using DaLion.Overhaul.Modules.Rings.Events;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCanBeDamagedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCanBeDamagedPatcher"/> class.</summary>
    internal FarmerCanBeDamagedPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.CanBeDamaged));
    }

    #region harmony patches

    /// <summary>Ring of Yoba rebalance.</summary>
    [HarmonyPrefix]
    private static bool FarmerCanBeDamagedPostfix(Farmer __instance, ref bool __result)
    {
        __result = !__instance.temporarilyInvincible && !__instance.isEating && !Game1.fadeToBlack &&
               (!Game1.buffsDisplay.hasBuff(21) || RingsModule.Config.RebalancedRings);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
