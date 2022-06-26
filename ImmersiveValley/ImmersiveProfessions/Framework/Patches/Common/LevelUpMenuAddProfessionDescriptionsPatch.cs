namespace DaLion.Stardew.Professions.Framework.Patches.Common;

#region using directives

using DaLion.Common;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuAddProfessionDescriptionsPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuAddProfessionDescriptionsPatch()
    {
        Target = RequireMethod<LevelUpMenu>("addProfessionDescriptions");
    }

    #region harmony patches

    /// <summary>Patch to apply modded profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuAddProfessionDescriptionsPrefix(List<string> descriptions,
        string professionName)
    {
        try
        {
            if (!Profession.TryFromName(professionName, out var profession) || (Skill)profession.Skill == Skill.Luck) return true; // run original logic

            descriptions.Add(profession.GetDisplayName(Game1.player.IsMale));

            var skillIndex = profession / 6;
            var currentLevel = Game1.player.GetUnmodifiedSkillLevel(skillIndex);
            var prestiged = Game1.player.HasProfession(profession, true) ||
                            Game1.activeClickableMenu is LevelUpMenu && currentLevel > 10;
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