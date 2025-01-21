namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuGetImmediateProfessionPerkPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuGetImmediateProfessionPerkPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuGetImmediateProfessionPerkPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.getImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Skip this method. Now handled by <see cref="Profession.OnAdded"/>.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuGetImmediateProfessionPerkPrefix()
    {
        return false; // don't run original logic
    }

    #endregion harmony patches
}
