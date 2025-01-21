namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationUpdateEvenIfFarmerIsntHerePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationUpdateEvenIfFarmerIsntHerePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationUpdateEvenIfFarmerIsntHerePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.updateEvenIfFarmerIsntHere));
    }

    #region harmony patches

    /// <summary>Patch to make Hopper actually useful.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationUpdateEvenIfFarmerIsntHerePostfix(GameLocation __instance)
    {
        foreach (var chest in __instance.Objects.Values.OfType<Chest>())
        {
            if (chest.SpecialChestType == Chest.SpecialChestTypes.AutoLoader)
            {
                chest.mutex.Update(__instance);
            }
        }
    }

    #endregion harmony patches
}
