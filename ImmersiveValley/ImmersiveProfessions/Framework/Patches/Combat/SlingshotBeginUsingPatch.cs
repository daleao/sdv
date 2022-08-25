namespace DaLion.Stardew.Professions.Framework.Patches;

#region using directives

using Events.GameLoop;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotBeginUsingPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SlingshotBeginUsingPatch()
    {
        Target = RequireMethod<Slingshot>(nameof(Slingshot.beginUsing));
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