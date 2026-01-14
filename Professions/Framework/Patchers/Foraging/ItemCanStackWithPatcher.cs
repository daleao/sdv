namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using System.Reflection;
using System.Reflection.Emit;
using DaLion.Shared.Data;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class ItemCanStackWithPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ItemCanStackWithPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ItemCanStackWithPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Item>(nameof(Item.canStackWith));
    }

    #region harmony patches

    /// <summary>Patch to increase Arborist non-fruit tree growth odds.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ItemCanStackWithPostfix(Item __instance, bool __result)
    {
    }

    #endregion harmony patches
}
