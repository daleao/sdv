namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class SlingshotBeginUsingPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SlingshotBeginUsingPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SlingshotBeginUsingPatcher(Harmonizer harmonizer)
        : base(harmonizer)
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
