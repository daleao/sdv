namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class ObjectPerformUseActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ObjectPerformUseActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ObjectPerformUseActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SObject>(nameof(SObject.performUseAction));
    }

    #region harmony patches

    /// <summary>Play Slime Flute.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool ObjectPerformUseActionPrefix(SObject __instance, GameLocation location)
    {
        var player = Game1.player;
        var normalGameplay = !Game1.eventUp && !Game1.isFestival() && !Game1.fadeToBlack
            && !player.swimming.Value && !player.bathingClothes.Value
            && !player.onBridge.Value;
        if (!normalGameplay || __instance.ItemId != SlimeFluteId)
        {
            return true; // run original logic
        }

        Game1.player.PlaySlimeFlute();
        return false; // don't run original logic
    }

    #endregion harmony patches
}
