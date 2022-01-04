using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Common;

[UsedImplicitly]
internal class LevelUpMenuAddProfessionDescriptionsPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuAddProfessionDescriptionsPatch()
    {
        Original = RequireMethod<LevelUpMenu>("addProfessionDescriptions");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions,
        string professionName)
    {
        try
        {
            if (!Utility.Professions.IndexByName.Contains(professionName)) return true; // run original logic

            descriptions.Add(ModEntry.ModHelper.Translation.Get(professionName + ".name." +
                                                                (Game1.player.IsMale ? "male" : "female")));

            var professionIndex = Utility.Professions.IndexOf(professionName);
            var skillIndex = professionIndex / 6;
            var currentLevel = Game1.player.GetUnmodifiedSkillLevel(skillIndex);
            descriptions.AddRange(ModEntry.ModHelper.Translation
                .Get(professionName + ".desc" +
                     (Game1.player.HasPrestigedProfession(professionName) ||
                      Game1.activeClickableMenu is LevelUpMenu && currentLevel > 10
                         ? ".prestiged"
                         : string.Empty)).ToString()
                .Split('\n'));

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}