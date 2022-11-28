namespace DaLion.Ligo.Modules.Core.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
[Debug]
internal class DuggyUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="DuggyUpdatePatcher"/> class.</summary>
    internal DuggyUpdatePatcher()
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
