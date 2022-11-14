namespace DaLion.Ligo.Modules.Core.Patches;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Debug]
internal class DuggyUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="DuggyUpdatePatch"/> class.</summary>
    internal DuggyUpdatePatch()
    {
        this.Target = this.RequireMethod<Duggy>(nameof(Duggy.update), new[] { typeof(GameTime), typeof(GameLocation) });
    }

    #region harmony patches

    /// <summary>Allow Duggies to be stunned.</summary>
    [HarmonyPrefix]
    private static bool DuggyUpdatePrefix(Duggy __instance)
    {
        return __instance.stunTime <= 0;
    }

    #endregion harmony patches
}
