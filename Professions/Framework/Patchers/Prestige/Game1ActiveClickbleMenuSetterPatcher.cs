namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1ActiveClickbleMenuSetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1ActiveClickbleMenuSetterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Game1ActiveClickbleMenuSetterPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequirePropertySetter<Game1>(nameof(Game1.activeClickableMenu));
    }

    #region harmony patches

    /// <summary>Reload profession sprites on level-up.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Game1ActiveClickbleMenuSetterPostfix(IClickableMenu? value)
    {
        if (value is LevelUpMenu { isProfessionChooser: true } levelup)
        {
            var level = Reflector.GetUnboundFieldGetter<LevelUpMenu, int>("currentLevel").Invoke(levelup);
            if (level > 10)
            {
                ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
            }
        }
    }

    #endregion harmony patches
}
