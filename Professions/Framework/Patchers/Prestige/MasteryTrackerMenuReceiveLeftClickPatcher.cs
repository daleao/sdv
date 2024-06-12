namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Professions.Framework.UI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class MasteryTrackerMenuReceiveLeftClickPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MasteryTrackerMenuReceiveLeftClickPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MasteryTrackerMenuReceiveLeftClickPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MasteryTrackerMenu>(nameof(MasteryTrackerMenu.receiveLeftClick));
    }

    #region harmony patches

    /// <summary>Patch for Mastery warnings and unlocks.</summary>
    [HarmonyPrefix]
    private static bool MasteryTrackerMenuReceiveLeftClickPrefix(
        MasteryTrackerMenu __instance,
        bool ___canClaim,
        float ___destroyTimer,
        float ___pressedButtonTimer,
        int x,
        int y,
        int ___which)
    {
        if (___which == -1)
        {
            return true;
        }

        if (State.WarningBox is not null)
        {
            return false; // don't run original logic
        }

        if (___destroyTimer > 0f || __instance.mainButton is null || !__instance.mainButton.containsPoint(x, y) ||
            ___pressedButtonTimer > 0f || !___canClaim ||
            Game1.player.HasAllProfessionsInSkill(Skill.FromValue(___which)))
        {
            return true; // run original logic
        }

        State.WarningBox = new MasteryWarningBox(Game1.currentLocation, __instance);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
