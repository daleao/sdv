﻿namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuRemoveImmediateProfessionPerkPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuRemoveImmediateProfessionPerkPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuRemoveImmediateProfessionPerkPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.removeImmediateProfessionPerk));
    }

    #region harmony patches

    /// <summary>Skip this method. Now handled by <see cref="Profession.OnRemoved"/>.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuRemoveImmediateProfessionPerkPrefix()
    {
        return false; // don't run original logic
    }

    #endregion harmony patches
}
