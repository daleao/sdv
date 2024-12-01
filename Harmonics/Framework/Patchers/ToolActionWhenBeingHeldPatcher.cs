namespace DaLion.Harmonics.Framework.Patchers;

#region using directives

using DaLion.Harmonics.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenBeingHeldPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal ToolActionWhenBeingHeldPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset applied tool resonances.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenBeingHeldPostfix(Tool __instance, Farmer who)
    {
        if (__instance is not MeleeWeapon weapon)
        {
            return;
        }

        foreach (var chord in State.ResonantChords.Values)
        {
            chord.ResonateAllForges(weapon);
        }
    }

    #endregion harmony patches
}
