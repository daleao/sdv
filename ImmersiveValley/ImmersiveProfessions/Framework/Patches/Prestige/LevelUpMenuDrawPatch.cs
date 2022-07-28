namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

#endregion using directives

[UsedImplicitly]
internal sealed class LevelUpMenuDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static readonly Lazy<Func<LevelUpMenu, List<int>>> _GetProfessionsToChoose = new(() =>
        typeof(LevelUpMenu).RequireField("professionsToChoose")
            .CompileUnboundFieldGetterDelegate<LevelUpMenu, List<int>>());

    /// <summary>Construct an instance.</summary>
    internal LevelUpMenuDrawPatch()
    {
        Target = RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to increase the height of Level Up Menu to fit longer profession descriptions.</summary>
    [HarmonyPrefix]
    private static void LevelUpMenuDrawPrefix(LevelUpMenu __instance, int ___currentLevel)
    {
        if (__instance.isProfessionChooser && ___currentLevel == 10)
            __instance.height += 16;
    }

    /// <summary>Patch to draw Prestige tooltip during profession selection.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? LevelUpMenuDrawTranspiler(IEnumerable<CodeInstruction> instructions,
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
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuDrawPatch).RequireMethod(nameof(GetChooseProfessionText)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up menu choose profession text.\nHelper returned {ex}");
            return null;
        }

        /// Injected: DrawSubroutine(this, b);
        /// Before: else if (!isProfessionChooser)

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldfld,
                        typeof(LevelUpMenu).RequireField(nameof(LevelUpMenu.isProfessionChooser)))
                )
                .Advance()
                .GetOperand(out var isNotProfessionChooser)
                .FindLabel((Label)isNotProfessionChooser)
                .Retreat()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(LevelUpMenu).RequireField("currentLevel")),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(LevelUpMenuDrawPatch).RequireMethod(nameof(DrawSubroutine)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching level up menu prestige tooltip draw.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    internal static string GetChooseProfessionText(int currentLevel) =>
        currentLevel > 10
            ? ModEntry.i18n.Get("prestige.levelup.prestige")
            : Game1.content.LoadString("Strings\\UI:LevelUp_ChooseProfession");

    private static void DrawSubroutine(LevelUpMenu menu, int currentLevel, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige || !menu.isProfessionChooser || currentLevel > 10) return;

        var professionsToChoose = _GetProfessionsToChoose.Value(menu);
        if (!Profession.TryFromValue(professionsToChoose[0], out var leftProfession) ||
            !Profession.TryFromValue(professionsToChoose[1], out var rightProfession)) return;

        Rectangle selectionArea;
        if (Game1.player.HasProfession(leftProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(leftProfession))
        {
            selectionArea = new(menu.xPositionOnScreen + 32, menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.i18n.Get(leftProfession % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }

        if (Game1.player.HasProfession(rightProfession) &&
            Game1.player.HasAllProfessionsBranchingFrom(rightProfession))
        {
            selectionArea = new(menu.xPositionOnScreen + menu.width / 2 + 8,
                menu.yPositionOnScreen + 232,
                menu.width / 2 - 40, menu.height - 264);
            b.Draw(Game1.staminaRect, selectionArea, new(Color.Black, 0.3f));

            if (selectionArea.Contains(Game1.getMouseX(), Game1.getMouseY()))
            {
                var hoverText = ModEntry.i18n.Get(leftProfession % 6 <= 1
                    ? "prestige.levelup.tooltip:5"
                    : "prestige.levelup.tooltip:10");
                IClickableMenu.drawHoverText(b, hoverText, Game1.smallFont);
            }
        }
    }

    #endregion injected subroutines
}