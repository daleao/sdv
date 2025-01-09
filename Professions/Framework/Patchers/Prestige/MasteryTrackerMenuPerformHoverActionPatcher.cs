namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class MasteryTrackerMenuPerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MasteryTrackerMenuPerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MasteryTrackerMenuPerformHoverActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MasteryTrackerMenu>(nameof(MasteryTrackerMenu.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to prevent interaction during warning.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MasteryTrackerMenuPerformHoverActionPrefix()
    {
        return State.WarningBox is null;
    }

    #endregion harmony patches
}
