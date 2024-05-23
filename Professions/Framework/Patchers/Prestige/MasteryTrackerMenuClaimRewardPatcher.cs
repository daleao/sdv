namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Professions.Framework.UI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class MasteryTrackerMenuClaimRewardPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MasteryTrackerMenuClaimRewardPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MasteryTrackerMenuClaimRewardPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<MasteryTrackerMenu>("claimReward");
    }

    #region harmony patches

    /// <summary>Patch for post-Mastery unlocks.</summary>
    [HarmonyPostfix]
    private static void MasteryTrackerMenuPostfix(MasteryTrackerMenu __instance, int ___which)
    {
        var skill = Skill.FromValue(___which);
        if (skill == Skill.Combat && ShouldEnableLimitBreaks)
        {
            Game1.activeClickableMenu = new MasteryLimitSelectionPage();
        }
        else if (ShouldEnablePrestigeLevels)
        {
            //Game1.delayedActions.Add(new DelayedAction(
            //    350,
            //    () => Game1.drawObjectDialogue(I18n.Prestige_Mastery_Unlocked(skill.DisplayName))));
        }
    }

    #endregion harmony patches
}
