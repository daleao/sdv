namespace DaLion.Overhaul.Modules.Rings.Patchers;

#region using directives

using DaLion.Overhaul.Modules.Rings.Extensions;
using DaLion.Overhaul.Modules.Rings.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolActionWhenBeingHeldPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolActionWhenBeingHeldPatcher"/> class.</summary>
    internal ToolActionWhenBeingHeldPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.actionWhenBeingHeld));
    }

    #region harmony patches

    /// <summary>Reset applied arsenal resonances.</summary>
    [HarmonyPostfix]
    private static void ToolActionWhenBeingHeldPostfix(Tool __instance, Farmer who)
    {
        if (!ArsenalModule.IsEnabled)
        {
            return;
        }

        var chords = who.Get_ResonatingChords();
        for (var i = 0; i < chords.Count; i++)
        {
            var chord = chords[i];
            if (chord.Root is not null && __instance.CanResonateWith(chord.Root))
            {
                __instance.UpdateResonatingChord(chords[i]);
            }
        }
    }

    #endregion harmony patches
}
