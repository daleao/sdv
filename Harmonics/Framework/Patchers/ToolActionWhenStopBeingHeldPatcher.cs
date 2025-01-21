namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenStopBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenStopBeingHeldPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal ToolActionWhenStopBeingHeldPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenStopBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset applied tool resonances.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void ToolActionWhenStopBeingHeldPostfix(Tool __instance, Farmer who)
    {
        if (!who.IsLocalPlayer || __instance is not MeleeWeapon weapon ||
            !State.ResonantBlades.Contains(weapon))
        {
            return;
        }

        foreach (var chord in State.ResonantChords.Values)
        {
            chord.QuenchAllForges(weapon);
        }

        State.ResonantBlades.Remove(weapon);
    }

    #endregion harmony patches
}
