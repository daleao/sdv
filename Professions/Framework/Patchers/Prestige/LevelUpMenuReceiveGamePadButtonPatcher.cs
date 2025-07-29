namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuReceiveGamePadButtonPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuReceiveGamePadButtonPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuReceiveGamePadButtonPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.receiveGamePadButton));
    }

    #region harmony patches

    /// <summary>Patch to idiot-proof the level-up menu. </summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuUpdatePrefix(
        LevelUpMenu __instance,
        List<int> ___professionsToChoose,
        int ___currentLevel,
        int ___currentSkill,
        Buttons button)
    {
        try
        {
            if (!__instance.isProfessionChooser || !__instance.IsActive() || ___professionsToChoose.Count != 1)
            {
                return true; // run original logic
            }

            if (button is not (Buttons.Start or Buttons.A or Buttons.B))
            {
                return false; // don't run original logic
            }

            __instance.okButtonClicked();
            var player = Game1.player;
            var rootId = player.GetCurrentRootProfessionForSkill(Skill.FromValue(___currentSkill));
            switch (___currentLevel)
            {
                case 15:
                    if (player.professions.AddOrReplace(rootId + 100))
                    {
                        __instance.getImmediateProfessionPerk(rootId + 100);
                    }

                    break;
                case 20:
                    var branch =
                        player.GetCurrentBranchingProfessionForRoot(Profession.FromValue(rootId));
                    ___professionsToChoose.Add(branch);
                    if (player.professions.AddOrReplace(branch + 100))
                    {
                        __instance.getImmediateProfessionPerk(branch + 100);
                    }

                    break;
            }

            __instance.RemoveLevelFromLevelList();
            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
