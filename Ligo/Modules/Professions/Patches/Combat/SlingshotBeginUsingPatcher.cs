namespace DaLion.Ligo.Modules.Professions.Patches.Combat;

#region using directives

using DaLion.Ligo.Modules.Professions.Events.GameLoop;
using HarmonyLib;
using Shared.Harmony;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotBeginUsingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotBeginUsingPatcher"/> class.</summary>
    internal SlingshotBeginUsingPatcher()
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
