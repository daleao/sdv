namespace DaLion.Overhaul.Modules.Professions.Patchers.Combat;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
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
        if (Game1.player.HasProfession(Profession.Desperado))
        {
            EventManager.Enable<DesperadoOverchargeUpdateTickedEvent>();
        }
    }

    #endregion harmony patches
}
