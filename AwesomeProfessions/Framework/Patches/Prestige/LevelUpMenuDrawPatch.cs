using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class LevelUpMenuDrawPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuDrawPatch()
    {
        Original = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.draw), new[] {typeof(SpriteBatch)});
    }

    #region harmony patches

    /// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
    [HarmonyPrefix]
    private static bool LevelUpMenuDrawPrefix(LevelUpMenu __instance, int ___currentSkill, int ___currentLevel)
    {
        if (__instance.isProfessionChooser && ___currentLevel == 10)
            __instance.height += 16;

        return true; // run original logic
    }

    /// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> LevelUpMenuDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: string chooseProfession = Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
        /// To: string chooseProfession = GetChooseProfessionText(this);

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Stloc_1)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Ldsfld)
                )
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Callvirt)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuDrawPatch).MethodNamed(nameof(GetChooseProfessionText)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching level up menu choose profession text. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        /// Injected: DrawSubroutine(this, b);
        /// Before: else if (!isProfessionChooser)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(LevelUpMenu).Field(nameof(LevelUpMenu.isProfessionChooser)))
                )
                .Advance()
                .GetOperand(out var isNotProfessionChooser)
                .FindLabel((Label) isNotProfessionChooser)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuDrawPatch).MethodNamed(nameof(DrawSubroutine)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching level up menu prestige ribbon draw. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static string GetChooseProfessionText(LevelUpMenu menu)
    {
        var currentLevel = ModEntry.ModHelper.Reflection.GetField<int>(menu, "currentLevel").GetValue();
        return currentLevel > 10
            ? ModEntry.ModHelper.Translation.Get("prestige.levelup.prestige")
            : Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");
    }

    private static void DrawSubroutine(LevelUpMenu menu, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige || !menu.isProfessionChooser) return;

        var currentLevel = ModEntry.ModHelper.Reflection.GetField<int>(menu, "currentLevel").GetValue();
        if (currentLevel > 10) return;

        var professionsToChoose = ModEntry.ModHelper.Reflection.GetField<List<int>>(menu, "professionsToChoose")
            .GetValue();
        var leftProfession = professionsToChoose[0];
        var rightProfession = professionsToChoose[1];

        if (Game1.player.professions.Contains(leftProfession) &&
            Game1.player.HasAllProfessionsInBranch(leftProfession))
        {
            var selectionArea = new Rectangle(menu.xPositionOnScreen + 32, menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.ModHelper.Translation.Get(leftProfession % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }

        if (Game1.player.professions.Contains(rightProfession) &&
            Game1.player.HasAllProfessionsInBranch(rightProfession))
        {
            var selectionArea = new Rectangle(menu.xPositionOnScreen + menu.width / 2 + 8,
                menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.ModHelper.Translation.Get(leftProfession % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion private methods
}