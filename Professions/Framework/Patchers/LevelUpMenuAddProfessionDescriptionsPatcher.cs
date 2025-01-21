namespace DaLion.Professions.Framework.Patchers;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuAddProfessionDescriptionsPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuAddProfessionDescriptionsPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuAddProfessionDescriptionsPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LevelUpMenu>("addProfessionDescriptions");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession descriptions.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuAddProfessionDescriptionsPrefix(
        List<string> descriptions, string professionName)
    {
        if (!Profession.TryFromName(professionName, true, out var profession))
        {
            return true; // run original logic
        }

        try
        {
            //var currentLevel = profession.ParentSkill.CurrentLevel;
            var prestiged = Game1.player.HasProfession(profession, true) ||
                            (Game1.activeClickableMenu is LevelUpMenu menu && Reflector
                                .GetUnboundFieldGetter<LevelUpMenu, int>("currentLevel").Invoke(menu) > 10);
            descriptions.Add(profession.GetTitle(prestiged));
            descriptions.AddRange(profession.GetDescription(prestiged).Split('\n'));

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
