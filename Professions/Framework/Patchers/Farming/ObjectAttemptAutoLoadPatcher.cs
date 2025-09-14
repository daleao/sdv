namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Inventories;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectAttemptAutoLoadPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectAttemptAutoLoadPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectAttemptAutoLoadPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target =
            this.RequireMethod<SObject>(nameof(SObject.AttemptAutoLoad), [typeof(IInventory), typeof(Farmer)]);
    }

    #region harmony patches

    /// <summary>Patch for Industrialist production speed bonus.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ObjectAttemptAutoLoadPostfix(SObject __instance, bool __result, Farmer who)
    {
        if (!__instance.IsArtisanMachine() || !__result || !who.HasProfession(Profession.Artisan, true))
        {
            return;
        }

        if (__instance is Cask cask)
        {
            cask.daysToMature.Value -= cask.daysToMature.Value / 4;
        }
        else
        {
            __instance.MinutesUntilReady -= __instance.MinutesUntilReady / 4;
        }
    }

    #endregion harmony patches
}
