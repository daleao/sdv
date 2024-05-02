namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Professions.Configs;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directive

[UsedImplicitly]
internal sealed class GameLocationCanRespecPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationCanRespecPatcher"/> class.</summary>
    internal GameLocationCanRespecPatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.canRespec));
    }

    #region harmony patches

    /// <summary>Patch to change Statue of Uncertainty respec from (less than) 10 to (greater than) 10.</summary>
    [HarmonyPrefix]
    private static bool GameLocationCanRespecPrefix(ref bool __result, int skill_index)
    {
        try
        {
            if (ProfessionsModule.Config.Prestige.Mode == PrestigeConfig.PrestigeMode.Capped)
            {
                __result = false;
                return false; // don't run original logic
            }

            var p = ProfessionsModule.Config.Prestige.Mode is PrestigeConfig.PrestigeMode.Standard
                or PrestigeConfig.PrestigeMode.Challenge
                ? 10
                : 0;

            __result = Game1.player.GetUnmodifiedSkillLevel(skill_index) >= 5 + p &&
                       !Game1.player.newLevels.Contains(new Point(skill_index, 5 + p)) &&
                       !Game1.player.newLevels.Contains(new Point(skill_index, 10 + p));
            if (ProfessionsModule.Config.Prestige.Mode == PrestigeConfig.PrestigeMode.Streamlined)
            {
                __result = __result &&
                           !Game1.player.newLevels.Contains(new Point(skill_index, 5)) &&
                           !Game1.player.newLevels.Contains(new Point(skill_index, 10));
            }

            return false; // don't run original logic;
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
