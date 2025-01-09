namespace DaLion.Core.Framework.Patchers;

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
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal DuggyUpdatePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Duggy>(nameof(Duggy.update), [typeof(GameTime), typeof(GameLocation)]);
    }

    #region harmony patches

    /// <summary>Allow Duggies to be stunned.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool DuggyUpdatePrefix(Duggy __instance)
    {
        return __instance.stunTime.Value <= 0;
    }

    #endregion harmony patches
}
