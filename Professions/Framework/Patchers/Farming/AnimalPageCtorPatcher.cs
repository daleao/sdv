namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class AnimalPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="AnimalPageCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal AnimalPageCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = typeof(AnimalPage).RequireConstructor(4);
    }

    #region harmony patches

    /// <summary>Patch to increase width of Animal Page menu for Crop Feed icon.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void AnimalPageCtorPrefix(ref int width)
    {
        if (Game1.player.HasProfession(Profession.Rancher))
        {
            width += 76;
        }
    }

    #endregion harmony patches
}
