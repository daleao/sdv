namespace DaLion.Redux.Professions.Patches.Combat;

#region using directives

using DaLion.Redux.Professions.Events.GameLoop;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotBeginUsingPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotBeginUsingPatch"/> class.</summary>
    internal SlingshotBeginUsingPatch()
    {
        this.Target = this.RequireMethod<Slingshot>(nameof(Slingshot.beginUsing));
    }

    #region harmony patches

    /// <summary>Patch to trigger Desperado overcharge.</summary>
    [HarmonyPostfix]
    private static void SlingshotBeginUsingPostfix()
    {
        ModEntry.Events.Enable<DesperadoUpdateTickedEvent>();
    }

    #endregion harmony patches
}
